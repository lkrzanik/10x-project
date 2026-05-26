using System.Text;
using _10xPV.Services.Csv;
using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Tests.Services.Csv;

public class CsvImportServiceTests
{
    private readonly CsvImportService _sut = new();

    [Fact]
    public async Task ImportAsync_SensorValidFile_ReturnsSingleValidSensorRow()
    {
        const string csv = "Timestamp,SensorId,Value,Unit\n2026-01-15T10:30:00,SEN-01,123.45,kWh\n";

        await using var stream = CreateUtf8Stream(csv);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Sensor);

        Assert.Equal(CsvSchemaType.Sensor, result.SchemaType);
        Assert.Equal(1, result.TotalRows);
        Assert.Equal(1, result.ValidRows);
        Assert.Equal(0, result.InvalidRows);
        Assert.Empty(result.Errors);
        Assert.Single(result.SensorRows);
        Assert.Empty(result.WeatherRows);

        var row = result.SensorRows[0];
        Assert.Equal(new DateTime(2026, 1, 15, 10, 30, 0), row.Timestamp);
        Assert.Equal("SEN-01", row.SensorId);
        Assert.Equal(123.45, row.Value, 5);
        Assert.Equal("kWh", row.Unit);
    }

    [Fact]
    public async Task ImportAsync_WeatherValidFile_ReturnsSingleValidWeatherRow()
    {
        const string csv = "Timestamp,TemperatureC,WindSpeedMs,IrradianceWm2\n2026-02-01,12.3,4.5,800\n";

        await using var stream = CreateUtf8Stream(csv);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Weather);

        Assert.Equal(CsvSchemaType.Weather, result.SchemaType);
        Assert.Equal(1, result.TotalRows);
        Assert.Equal(1, result.ValidRows);
        Assert.Equal(0, result.InvalidRows);
        Assert.Empty(result.Errors);
        Assert.Single(result.WeatherRows);
        Assert.Empty(result.SensorRows);

        var row = result.WeatherRows[0];
        Assert.Equal(new DateTime(2026, 2, 1), row.Timestamp);
        Assert.Equal(12.3, row.TemperatureC, 5);
        Assert.Equal(4.5, row.WindSpeedMs, 5);
        Assert.Equal(800, row.IrradianceWm2, 5);
    }

    [Fact]
    public async Task ImportAsync_InvalidHeaders_ReturnsHeaderInvalidError()
    {
        const string csv = "Timestamp,WrongField,Value,Unit\n2026-01-15T10:30:00,SEN-01,123.45,kWh\n";

        await using var stream = CreateUtf8Stream(csv);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Sensor);

        Assert.Equal(0, result.TotalRows);
        Assert.Equal(0, result.ValidRows);
        Assert.Equal(0, result.InvalidRows);
        Assert.Empty(result.SensorRows);
        Assert.Empty(result.WeatherRows);

        var error = Assert.Single(result.Errors);
        Assert.Equal(1, error.LineNumber);
        Assert.Null(error.Field);
        Assert.Equal(CsvErrorCodes.HeaderInvalid, error.Code);
    }

    [Fact]
    public async Task ImportAsync_MixedSensorRows_CollectsErrorsAndContinues()
    {
        const string csv =
            "Timestamp,SensorId,Value,Unit\n" +
            "2026-01-15T10:30:00,SEN-01,123.45,kWh\n" +
            "invalid-date,SEN-02,44.1,kWh\n" +
            "2026-01-15T11:00:00,SEN-03,NaN,kWh\n";

        await using var stream = CreateUtf8Stream(csv);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Sensor);

        Assert.Equal(3, result.TotalRows);
        Assert.Equal(1, result.ValidRows);
        Assert.Equal(2, result.InvalidRows);
        Assert.Single(result.SensorRows);
        Assert.Equal(2, result.Errors.Count);

        Assert.Contains(result.Errors, error =>
            error.LineNumber == 3 &&
            error.Field == "Timestamp" &&
            error.Code == CsvErrorCodes.RowDateTimeInvalid);

        Assert.Contains(result.Errors, error =>
            error.LineNumber == 4 &&
            error.Field == "Value" &&
            error.Code == CsvErrorCodes.RowNumberNanOrInf);
    }

    [Fact]
    public async Task ImportAsync_OnlyHeaderAndEmptyRows_ReturnsZeroRowsWithoutErrors()
    {
        const string csv = "Timestamp,SensorId,Value,Unit\n\n   \n";

        await using var stream = CreateUtf8Stream(csv);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Sensor);

        Assert.Equal(0, result.TotalRows);
        Assert.Equal(0, result.ValidRows);
        Assert.Equal(0, result.InvalidRows);
        Assert.Empty(result.SensorRows);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task ImportAsync_EmptyFile_ReturnsEmptyFileError()
    {
        await using var stream = CreateUtf8Stream(string.Empty);
        var result = await _sut.ImportAsync(stream, CsvSchemaType.Sensor);

        Assert.Equal(0, result.TotalRows);
        Assert.Equal(0, result.ValidRows);
        Assert.Equal(0, result.InvalidRows);
        Assert.Empty(result.SensorRows);
        Assert.Empty(result.WeatherRows);

        var error = Assert.Single(result.Errors);
        Assert.Equal(CsvErrorCodes.EmptyFile, error.Code);
        Assert.Equal(1, error.LineNumber);
        Assert.Null(error.Field);
    }

    private static MemoryStream CreateUtf8Stream(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }
}
