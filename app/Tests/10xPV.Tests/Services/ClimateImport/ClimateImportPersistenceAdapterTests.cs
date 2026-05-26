using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Services.ClimateImport;
using _10xPV.Services.Csv.Contracts;
using Microsoft.EntityFrameworkCore;

namespace _10xPV.Tests.Services.ClimateImport;

public class ClimateImportPersistenceAdapterTests
{
    [Fact]
    public async Task PersistValidRowsAsync_SensorRows_PersistsOnlyUniqueTimestamps()
    {
        await using var dbContext = CreateDbContext();
        dbContext.SensorReadings.Add(new SensorReading
        {
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.Zero),
            Temperature = 10.0,
            Humidity = 40.0
        });
        await dbContext.SaveChangesAsync();

        var importResult = new CsvImportResult(
            CsvSchemaType.Sensor,
            [
                new SensorCsvRow(new DateTime(2026, 5, 10, 10, 0, 0), 11.2, 41.1),
                new SensorCsvRow(new DateTime(2026, 5, 10, 11, 0, 0), 12.3, 42.2),
                new SensorCsvRow(new DateTime(2026, 5, 10, 11, 0, 0), 99.0, 99.0)
            ],
            [],
            [],
            TotalRows: 3,
            ValidRows: 3,
            InvalidRows: 0);

        var sut = new ClimateImportPersistenceAdapter(dbContext);

        await sut.PersistValidRowsAsync(importResult);

        var saved = await dbContext.SensorReadings
            .AsNoTracking()
            .OrderBy(row => row.Timestamp)
            .ToListAsync();

        Assert.Equal(2, saved.Count);
        Assert.Equal(new DateTimeOffset(2026, 5, 10, 10, 0, 0, TimeSpan.Zero), saved[0].Timestamp);
        Assert.Equal(10.0, saved[0].Temperature);
        Assert.Equal(40.0, saved[0].Humidity);
        Assert.Equal(new DateTimeOffset(2026, 5, 10, 11, 0, 0, TimeSpan.Zero), saved[1].Timestamp);
        Assert.True(saved[1].Temperature.HasValue);
        Assert.True(saved[1].Humidity.HasValue);
        var temperature = saved[1].Temperature.GetValueOrDefault();
        var humidity = saved[1].Humidity.GetValueOrDefault();
        Assert.Equal(12.3, temperature, 5);
        Assert.Equal(42.2, humidity, 5);
    }

    [Fact]
    public async Task PersistValidRowsAsync_WeatherRows_PersistsRowsAndSkipsDuplicatesAcrossImports()
    {
        await using var dbContext = CreateDbContext();
        var sut = new ClimateImportPersistenceAdapter(dbContext);

        var firstImport = new CsvImportResult(
            CsvSchemaType.Weather,
            [],
            [
                new WeatherCsvRow(new DateTime(2026, 6, 1, 9, 0, 0), 19.5, 61.0, 70.0),
                new WeatherCsvRow(new DateTime(2026, 6, 1, 10, 0, 0), 20.0, 62.0, 65.0)
            ],
            [],
            TotalRows: 2,
            ValidRows: 2,
            InvalidRows: 0);

        var secondImport = new CsvImportResult(
            CsvSchemaType.Weather,
            [],
            [
                new WeatherCsvRow(new DateTime(2026, 6, 1, 10, 0, 0), 21.0, 63.0, 50.0),
                new WeatherCsvRow(new DateTime(2026, 6, 1, 11, 0, 0), 22.0, 64.0, 45.0)
            ],
            [],
            TotalRows: 2,
            ValidRows: 2,
            InvalidRows: 0);

        await sut.PersistValidRowsAsync(firstImport);
        await sut.PersistValidRowsAsync(secondImport);

        var saved = await dbContext.WeatherReadings
            .AsNoTracking()
            .OrderBy(row => row.Timestamp)
            .ToListAsync();

        Assert.Equal(3, saved.Count);
        Assert.Equal(new DateTimeOffset(2026, 6, 1, 9, 0, 0, TimeSpan.Zero), saved[0].Timestamp);
        Assert.Equal(new DateTimeOffset(2026, 6, 1, 10, 0, 0, TimeSpan.Zero), saved[1].Timestamp);
        Assert.Equal(new DateTimeOffset(2026, 6, 1, 11, 0, 0, TimeSpan.Zero), saved[2].Timestamp);

        Assert.True(saved[1].Temperature.HasValue);
        Assert.True(saved[1].Humidity.HasValue);
        Assert.True(saved[1].CloudCover.HasValue);
        var temperature = saved[1].Temperature.GetValueOrDefault();
        var humidity = saved[1].Humidity.GetValueOrDefault();
        var cloudCover = saved[1].CloudCover.GetValueOrDefault();
        Assert.Equal(20.0, temperature, 5);
        Assert.Equal(62.0, humidity, 5);
        Assert.Equal(65.0, cloudCover, 5);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }
}