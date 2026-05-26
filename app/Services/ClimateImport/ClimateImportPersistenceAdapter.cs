using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Services.Csv.Contracts;
using Microsoft.EntityFrameworkCore;

namespace _10xPV.Services.ClimateImport;

public sealed class ClimateImportPersistenceAdapter : IClimateImportPersistenceAdapter
{
    private readonly AppDbContext _dbContext;

    public ClimateImportPersistenceAdapter(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task PersistValidRowsAsync(CsvImportResult importResult, CancellationToken cancellationToken = default)
    {
        if (importResult.ValidRows == 0)
        {
            return;
        }

        switch (importResult.SchemaType)
        {
            case CsvSchemaType.Sensor:
                await PersistSensorRowsAsync(importResult.SensorRows, cancellationToken);
                break;
            case CsvSchemaType.Weather:
                await PersistWeatherRowsAsync(importResult.WeatherRows, cancellationToken);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(importResult.SchemaType), importResult.SchemaType, "Nieobsługiwany typ schematu CSV.");
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task PersistSensorRowsAsync(IReadOnlyList<SensorCsvRow> rows, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return;
        }

        var sourceTimestamps = rows
            .Select(row => ToUtcOffset(row.Timestamp))
            .Distinct()
            .ToArray();

        var existingTimestamps = await _dbContext.SensorReadings
            .AsNoTracking()
            .Where(reading => sourceTimestamps.Contains(reading.Timestamp))
            .Select(reading => reading.Timestamp)
            .ToListAsync(cancellationToken);

        var existingSet = existingTimestamps.ToHashSet();
        var seenInBatch = new HashSet<DateTimeOffset>();

        var entitiesToInsert = rows
            .Select(row => new
            {
                Timestamp = ToUtcOffset(row.Timestamp),
                row.Temperature,
                row.Humidity
            })
            .Where(item => !existingSet.Contains(item.Timestamp) && seenInBatch.Add(item.Timestamp))
            .Select(item => new SensorReading
            {
                Id = Guid.NewGuid(),
                Timestamp = item.Timestamp,
                Temperature = item.Temperature,
                Humidity = item.Humidity
            })
            .ToArray();

        if (entitiesToInsert.Length == 0)
        {
            return;
        }

        await _dbContext.SensorReadings.AddRangeAsync(entitiesToInsert, cancellationToken);
    }

    private async Task PersistWeatherRowsAsync(IReadOnlyList<WeatherCsvRow> rows, CancellationToken cancellationToken)
    {
        if (rows.Count == 0)
        {
            return;
        }

        var sourceTimestamps = rows
            .Select(row => ToUtcOffset(row.Timestamp))
            .Distinct()
            .ToArray();

        var existingTimestamps = await _dbContext.WeatherReadings
            .AsNoTracking()
            .Where(reading => sourceTimestamps.Contains(reading.Timestamp))
            .Select(reading => reading.Timestamp)
            .ToListAsync(cancellationToken);

        var existingSet = existingTimestamps.ToHashSet();
        var seenInBatch = new HashSet<DateTimeOffset>();

        var entitiesToInsert = rows
            .Select(row => new
            {
                Timestamp = ToUtcOffset(row.Timestamp),
                row.Temperature,
                row.Humidity,
                row.CloudCover
            })
            .Where(item => !existingSet.Contains(item.Timestamp) && seenInBatch.Add(item.Timestamp))
            .Select(item => new WeatherReading
            {
                Id = Guid.NewGuid(),
                Timestamp = item.Timestamp,
                Temperature = item.Temperature,
                Humidity = item.Humidity,
                CloudCover = item.CloudCover
            })
            .ToArray();

        if (entitiesToInsert.Length == 0)
        {
            return;
        }

        await _dbContext.WeatherReadings.AddRangeAsync(entitiesToInsert, cancellationToken);
    }

    private static DateTimeOffset ToUtcOffset(DateTime timestamp)
    {
        var normalizedTimestamp = timestamp.Kind switch
        {
            DateTimeKind.Utc => timestamp,
            DateTimeKind.Local => timestamp.ToUniversalTime(),
            _ => DateTime.SpecifyKind(timestamp, DateTimeKind.Utc)
        };

        return new DateTimeOffset(normalizedTimestamp, TimeSpan.Zero);
    }
}