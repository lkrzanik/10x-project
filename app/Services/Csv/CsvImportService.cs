using System.Globalization;
using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Services.Csv;

public sealed class CsvImportService : ICsvImportService
{
    private static readonly string[] SupportedDateFormats = ["yyyy-MM-dd", "yyyy-MM-ddTHH:mm:ss"];

    private static readonly string[] SensorHeaders =
    [
        "Timestamp",
        "SensorId",
        "Value",
        "Unit"
    ];

    private static readonly string[] WeatherHeaders =
    [
        "Timestamp",
        "TemperatureC",
        "WindSpeedMs",
        "IrradianceWm2"
    ];

    public async Task<CsvImportResult> ImportAsync(
        Stream csvStream,
        CsvSchemaType schemaType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(csvStream);

        if (csvStream.CanSeek)
        {
            csvStream.Seek(0, SeekOrigin.Begin);
        }

        using var reader = new StreamReader(csvStream, detectEncodingFromByteOrderMarks: true, leaveOpen: true);

        var firstLine = await reader.ReadLineAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(firstLine))
        {
            return ResultWithSingleGlobalError(
                schemaType,
                CsvErrorCodes.EmptyFile,
                "Plik CSV jest pusty.");
        }

        var headerValues = ParseCsvLine(firstLine)
            .Select(value => value.Trim())
            .ToArray();

        if (headerValues.All(string.IsNullOrWhiteSpace))
        {
            return ResultWithSingleGlobalError(
                schemaType,
                CsvErrorCodes.HeaderMissing,
                "Brak wiersza nagłówków CSV.");
        }

        var expectedHeaders = GetExpectedHeaders(schemaType);
        if (!HeadersMatch(headerValues, expectedHeaders))
        {
            var expected = string.Join(", ", expectedHeaders);
            return ResultWithSingleGlobalError(
                schemaType,
                CsvErrorCodes.HeaderInvalid,
                $"Niepoprawne nagłówki CSV. Oczekiwano: {expected}.");
        }

        var sensorRows = new List<SensorCsvRow>();
        var weatherRows = new List<WeatherCsvRow>();
        var errors = new List<CsvRowError>();

        var totalRows = 0;
        var validRows = 0;
        var invalidRows = 0;
        var lineNumber = 1;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            if (line is null)
            {
                break;
            }

            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            totalRows++;
            var columns = ParseCsvLine(line);

            var rowErrors = schemaType switch
            {
                CsvSchemaType.Sensor => ValidateAndMapSensorRow(lineNumber, columns, sensorRows),
                CsvSchemaType.Weather => ValidateAndMapWeatherRow(lineNumber, columns, weatherRows),
                _ =>
                [
                    new CsvRowError(
                        lineNumber,
                        null,
                        CsvErrorCodes.HeaderInvalid,
                        "Nieobsługiwany typ schematu CSV.")
                ]
            };

            if (rowErrors.Count == 0)
            {
                validRows++;
                continue;
            }

            invalidRows++;
            errors.AddRange(rowErrors);
        }

        return new CsvImportResult(
            schemaType,
            sensorRows,
            weatherRows,
            errors,
            totalRows,
            validRows,
            invalidRows);
    }

    private static List<CsvRowError> ValidateAndMapSensorRow(
        int lineNumber,
        IReadOnlyList<string> columns,
        ICollection<SensorCsvRow> target)
    {
        var errors = new List<CsvRowError>();

        var timestampRaw = GetColumn(columns, 0);
        var sensorIdRaw = GetColumn(columns, 1);
        var valueRaw = GetColumn(columns, 2);
        var unitRaw = GetColumn(columns, 3);

        var timestamp = ParseRequiredDateTime(lineNumber, "Timestamp", timestampRaw, errors);
        var sensorId = ParseRequiredText(lineNumber, "SensorId", sensorIdRaw, errors);
        var value = ParseRequiredNumber(lineNumber, "Value", valueRaw, errors);
        var unit = ParseRequiredText(lineNumber, "Unit", unitRaw, errors);

        if (errors.Count == 0)
        {
            target.Add(new SensorCsvRow(timestamp!.Value, sensorId!, value!.Value, unit!));
        }

        return errors;
    }

    private static List<CsvRowError> ValidateAndMapWeatherRow(
        int lineNumber,
        IReadOnlyList<string> columns,
        ICollection<WeatherCsvRow> target)
    {
        var errors = new List<CsvRowError>();

        var timestampRaw = GetColumn(columns, 0);
        var temperatureRaw = GetColumn(columns, 1);
        var windSpeedRaw = GetColumn(columns, 2);
        var irradianceRaw = GetColumn(columns, 3);

        var timestamp = ParseRequiredDateTime(lineNumber, "Timestamp", timestampRaw, errors);
        var temperature = ParseRequiredNumber(lineNumber, "TemperatureC", temperatureRaw, errors);
        var windSpeed = ParseRequiredNumber(lineNumber, "WindSpeedMs", windSpeedRaw, errors);
        var irradiance = ParseRequiredNumber(lineNumber, "IrradianceWm2", irradianceRaw, errors);

        if (errors.Count == 0)
        {
            target.Add(new WeatherCsvRow(
                timestamp!.Value,
                temperature!.Value,
                windSpeed!.Value,
                irradiance!.Value));
        }

        return errors;
    }

    private static DateTime? ParseRequiredDateTime(
        int lineNumber,
        string field,
        string? rawValue,
        ICollection<CsvRowError> errors)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowFieldRequired,
                $"Pole '{field}' jest wymagane."));
            return null;
        }

        if (!DateTime.TryParseExact(
                rawValue.Trim(),
                SupportedDateFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var parsed))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowDateTimeInvalid,
                $"Pole '{field}' zawiera niepoprawny format daty i czasu."));
            return null;
        }

        return parsed;
    }

    private static double? ParseRequiredNumber(
        int lineNumber,
        string field,
        string? rawValue,
        ICollection<CsvRowError> errors)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowFieldRequired,
                $"Pole '{field}' jest wymagane."));
            return null;
        }

        if (!double.TryParse(
                rawValue.Trim(),
                NumberStyles.Float | NumberStyles.AllowThousands,
                CultureInfo.InvariantCulture,
                out var parsed))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowNumberInvalid,
                $"Pole '{field}' zawiera niepoprawną wartość liczbową."));
            return null;
        }

        if (double.IsNaN(parsed) || double.IsInfinity(parsed))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowNumberNanOrInf,
                $"Pole '{field}' nie może być NaN ani Infinity."));
            return null;
        }

        return parsed;
    }

    private static string? ParseRequiredText(
        int lineNumber,
        string field,
        string? rawValue,
        ICollection<CsvRowError> errors)
    {
        if (string.IsNullOrWhiteSpace(rawValue))
        {
            errors.Add(new CsvRowError(
                lineNumber,
                field,
                CsvErrorCodes.RowFieldRequired,
                $"Pole '{field}' jest wymagane."));
            return null;
        }

        return rawValue.Trim();
    }

    private static string? GetColumn(IReadOnlyList<string> columns, int index)
    {
        if (index >= columns.Count)
        {
            return null;
        }

        return columns[index];
    }

    private static string[] GetExpectedHeaders(CsvSchemaType schemaType) => schemaType switch
    {
        CsvSchemaType.Sensor => SensorHeaders,
        CsvSchemaType.Weather => WeatherHeaders,
        _ => []
    };

    private static bool HeadersMatch(IReadOnlyList<string> actual, IReadOnlyList<string> expected)
    {
        if (actual.Count != expected.Count)
        {
            return false;
        }

        for (var i = 0; i < expected.Count; i++)
        {
            if (!string.Equals(actual[i], expected[i], StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static CsvImportResult ResultWithSingleGlobalError(
        CsvSchemaType schemaType,
        string code,
        string message)
    {
        var error = new CsvRowError(1, null, code, message);
        return new CsvImportResult(schemaType, [], [], [error], 0, 0, 0);
    }

    private static string[] ParseCsvLine(string line)
    {
        if (string.IsNullOrEmpty(line))
        {
            return [];
        }

        var values = new List<string>();
        var current = new System.Text.StringBuilder();
        var insideQuotes = false;

        for (var i = 0; i < line.Length; i++)
        {
            var ch = line[i];

            if (ch == '"')
            {
                if (insideQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                    continue;
                }

                insideQuotes = !insideQuotes;
                continue;
            }

            if (ch == ',' && !insideQuotes)
            {
                values.Add(current.ToString());
                current.Clear();
                continue;
            }

            current.Append(ch);
        }

        values.Add(current.ToString());
        return values.ToArray();
    }
}
