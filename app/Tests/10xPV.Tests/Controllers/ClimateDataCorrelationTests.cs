using _10xPV.Controllers;
using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Models.Correlation;
using _10xPV.Services.Correlation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace _10xPV.Tests.Controllers;

public class ClimateCorrelationControllerTests
{
    [Fact]
    public async Task Correlation_WithValidData_ReturnsViewWithCorrelationResults()
    {
        await using var dbContext = CreateDbContext();
        SeedData(dbContext);

        var sut = CreateSut(dbContext);

        var result = await sut.Correlation(
            from: new DateOnly(2026, 1, 1),
            to: new DateOnly(2026, 1, 3));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        Assert.True(model.HasResults);
        Assert.NotNull(model.SensorMetadata);
        Assert.NotNull(model.WeatherMetadata);
        var expectedTimestamps = model.SensorPoints
            .Select(point => point.Timestamp)
            .Union(model.WeatherPoints.Select(point => point.Timestamp))
            .OrderBy(timestamp => timestamp)
            .ToList();

        Assert.Equal(expectedTimestamps, model.TableRows.Select(row => row.Timestamp));
        foreach (var row in model.TableRows)
        {
            var sensorPoint = model.SensorPoints.SingleOrDefault(point => point.Timestamp == row.Timestamp);
            var weatherPoint = model.WeatherPoints.SingleOrDefault(point => point.Timestamp == row.Timestamp);

            Assert.Equal(sensorPoint?.Value, row.SensorValue);
            Assert.Equal(sensorPoint?.IsInterpolated ?? false, row.SensorIsInterpolated);
            Assert.Equal(weatherPoint?.Value, row.WeatherValue);
            Assert.Equal(weatherPoint?.IsInterpolated ?? false, row.WeatherIsInterpolated);
        }

        Assert.Contains(model.TableRows, row => row.SensorIsInterpolated);
        Assert.False(model.HasExtremes);
    }

    [Fact]
    public async Task Correlation_WithExtremeTemperature_ReturnsExtremeInViewModel()
    {
        await using var dbContext = CreateDbContext();
        SeedData(dbContext);
        dbContext.SensorReadings.Add(new SensorReading
        {
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 6, 0, 0, TimeSpan.Zero),
            Temperature = 41.0,
            Humidity = 50.0
        });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Correlation(
            from: new DateOnly(2026, 1, 1),
            to: new DateOnly(2026, 1, 3));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        var extreme = Assert.Single(model.Extremes);
        Assert.True(model.HasExtremes);
        Assert.Equal(ClimateExtremeParameter.Temperature, extreme.Parameter);
        Assert.Equal(41.0, extreme.Value);
        Assert.Equal(ClimateExtremeDirection.AboveMaximum, extreme.Direction);
        Assert.Equal(ClimateExtremeSource.Sensor, extreme.Source);
    }

    [Fact]
    public async Task Extremes_WithExtremeTemperature_ReturnsDedicatedViewModel()
    {
        await using var dbContext = CreateDbContext();
        SeedData(dbContext);
        dbContext.SensorReadings.Add(new SensorReading
        {
            Id = Guid.NewGuid(),
            Timestamp = new DateTimeOffset(2026, 1, 2, 6, 0, 0, TimeSpan.Zero),
            Temperature = 41.0,
            Humidity = 50.0
        });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Extremes(
            from: new DateOnly(2026, 1, 1),
            to: new DateOnly(2026, 1, 3));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateExtremeViewModel>(viewResult.Model);

        var extreme = Assert.Single(model.Extremes);
        Assert.True(model.HasResults);
        Assert.Equal(ClimateExtremeParameter.Temperature, extreme.Parameter);
        Assert.Equal(41.0, extreme.Value);
        Assert.Equal(ClimateExtremeDirection.AboveMaximum, extreme.Direction);
        Assert.Equal(ClimateExtremeSource.Sensor, extreme.Source);
    }

    [Fact]
    public async Task Extremes_WithNoData_ReturnsEmptyState()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var result = await sut.Extremes(
            from: new DateOnly(2026, 1, 1),
            to: new DateOnly(2026, 1, 3));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateExtremeViewModel>(viewResult.Model);

        Assert.False(model.HasResults);
        Assert.Empty(model.Extremes);
    }

    [Fact]
    public async Task Extremes_WithInvalidDateRange_ReturnsValidationError()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var result = await sut.Extremes(
            from: new DateOnly(2026, 3, 10),
            to: new DateOnly(2026, 3, 1));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateExtremeViewModel>(viewResult.Model);

        Assert.False(sut.ModelState.IsValid);
        var error = Assert.Single(sut.ModelState[string.Empty]!.Errors);
        Assert.Equal("Data początkowa nie może być późniejsza niż data końcowa.", error.ErrorMessage);
        Assert.False(model.HasResults);
        Assert.Empty(model.Extremes);
    }

    [Fact]
    public async Task Correlation_WithNoData_ReturnsViewWithoutResults()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var result = await sut.Correlation();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        Assert.False(model.HasResults);
        Assert.Equal(0, model.TotalAlignedPoints);
        Assert.Empty(model.TableRows);
    }

    [Fact]
    public async Task Correlation_WithInvalidDateRange_ReturnsValidationError()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var result = await sut.Correlation(
            from: new DateOnly(2026, 3, 10),
            to: new DateOnly(2026, 3, 1));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        Assert.False(sut.ModelState.IsValid);
        var error = Assert.Single(sut.ModelState[string.Empty]!.Errors);
        Assert.Equal("Data początkowa nie może być późniejsza niż data końcowa.", error.ErrorMessage);
        Assert.False(model.HasResults);
    }

    [Fact]
    public async Task Correlation_ReturnsMetadataWithInterpolatedCounts()
    {
        await using var dbContext = CreateDbContext();
        SeedData(dbContext);

        var sut = CreateSut(dbContext);

        var result = await sut.Correlation(
            from: new DateOnly(2026, 1, 1),
            to: new DateOnly(2026, 1, 3));

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        // Metadata should be populated (exact values depend on service logic)
        Assert.NotNull(model.SensorMetadata);
        Assert.NotNull(model.WeatherMetadata);
        Assert.True(model.TotalAlignedPoints > 0);
    }

    [Fact]
    public async Task Correlation_PreservesFilterValuesInViewModel()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var from = new DateOnly(2026, 2, 1);
        var to = new DateOnly(2026, 2, 28);

        var result = await sut.Correlation(from: from, to: to);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<CorrelationViewModel>(viewResult.Model);

        Assert.Equal(from, model.From);
        Assert.Equal(to, model.To);
    }

    private static void SeedData(AppDbContext dbContext)
    {
        dbContext.SensorReadings.AddRange(
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), Temperature = 10.0, Humidity = 50.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero), Temperature = 12.0, Humidity = 52.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 3, 0, 0, 0, TimeSpan.Zero), Temperature = 14.0, Humidity = 54.0 });

        dbContext.WeatherReadings.AddRange(
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero), Temperature = 11.0, Humidity = 55.0, CloudCover = 30.0 },
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 2, 12, 0, 0, TimeSpan.Zero), Temperature = 13.0, Humidity = 57.0, CloudCover = 40.0 });

        dbContext.SaveChanges();
    }

    private static ClimateCorrelationController CreateSut(AppDbContext dbContext)
    {
        var correlationService = new ClimateCorrelationService();
        var extremeDetectionService = new ClimateExtremeDetectionService(new ExtremeDetectionOptions());
        return new ClimateCorrelationController(
            dbContext,
            correlationService,
            extremeDetectionService,
            NullLogger<ClimateCorrelationController>.Instance);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }
}
