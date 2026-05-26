using _10xPV.Controllers;
using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Models.ClimateImport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

namespace _10xPV.Tests.Controllers;

public class ClimateDataControllerTests
{
    [Fact]
    public async Task Index_DefaultQuery_ReturnsSensorRowsWithPaginationMetadata()
    {
        await using var dbContext = CreateDbContext();
        dbContext.SensorReadings.AddRange(
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero), Temperature = 10.0, Humidity = 50.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero), Temperature = 11.0, Humidity = 51.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 3, 0, 0, 0, TimeSpan.Zero), Temperature = 12.0, Humidity = 52.0 });

        dbContext.WeatherReadings.Add(
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 1, 4, 0, 0, 0, TimeSpan.Zero), Temperature = 20.0, Humidity = 60.0, CloudCover = 70.0 });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        Assert.Equal(DataSource.Sensor, model.Filter.DataSource);
        Assert.Equal(3, model.TotalCount);
        Assert.Equal(1, model.CurrentPage);
        Assert.Equal(1, model.TotalPages);
        Assert.Equal(50, model.PageSize);
        Assert.False(model.HasPreviousPage);
        Assert.False(model.HasNextPage);
        Assert.Equal(3, model.Rows.Count);
        Assert.All(model.Rows, row => Assert.Null(row.CloudCover));
    }

    [Fact]
    public async Task Index_WithDateRange_ReturnsOnlyRowsWithinInclusiveBounds()
    {
        await using var dbContext = CreateDbContext();
        dbContext.SensorReadings.AddRange(
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 2, 1, 10, 0, 0, TimeSpan.Zero), Temperature = 7.0, Humidity = 41.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 2, 2, 11, 0, 0, TimeSpan.Zero), Temperature = 8.0, Humidity = 42.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 2, 3, 12, 0, 0, TimeSpan.Zero), Temperature = 9.0, Humidity = 43.0 });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Index(
            dataSource: DataSource.Sensor,
            from: new DateOnly(2026, 2, 2),
            to: new DateOnly(2026, 2, 2),
            page: 1,
            pageSize: 50);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        var row = Assert.Single(model.Rows);
        Assert.Equal(new DateTimeOffset(2026, 2, 2, 11, 0, 0, TimeSpan.Zero), row.Timestamp);
    }

    [Fact]
    public async Task Index_WhenRequestedPageExceedsTotalPages_NormalizesToLastPage()
    {
        await using var dbContext = CreateDbContext();

        var weatherRows = Enumerable.Range(1, 75)
            .Select(index => new WeatherReading
            {
                Id = Guid.NewGuid(),
                Timestamp = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero).AddHours(index),
                Temperature = 15.0 + index,
                Humidity = 50.0 + index,
                CloudCover = 20.0 + index
            })
            .ToArray();

        dbContext.WeatherReadings.AddRange(weatherRows);
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Index(
            dataSource: DataSource.Weather,
            page: 10,
            pageSize: 50);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        Assert.Equal(2, model.CurrentPage);
        Assert.Equal(2, model.TotalPages);
        Assert.Equal(75, model.TotalCount);
        Assert.Equal(25, model.Rows.Count);
        Assert.True(model.HasPreviousPage);
        Assert.False(model.HasNextPage);
        Assert.All(model.Rows, row => Assert.NotNull(row.CloudCover));
    }

    [Fact]
    public async Task Index_WhenFromDateIsAfterToDate_ReturnsValidationErrorWithoutThrowing()
    {
        await using var dbContext = CreateDbContext();
        var sut = CreateSut(dbContext);

        var result = await sut.Index(
            dataSource: DataSource.Sensor,
            from: new DateOnly(2026, 4, 10),
            to: new DateOnly(2026, 4, 1),
            page: 1,
            pageSize: 50);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        Assert.False(sut.ModelState.IsValid);
        var error = Assert.Single(sut.ModelState[string.Empty]!.Errors);
        Assert.Equal("Data początkowa nie może być późniejsza niż data końcowa.", error.ErrorMessage);
        Assert.Empty(model.Rows);
        Assert.Equal(0, model.TotalCount);
    }

    [Fact]
    public async Task Index_WhenPageIsLessThanOne_NormalizesToFirstPage()
    {
        await using var dbContext = CreateDbContext();

        dbContext.SensorReadings.AddRange(
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 4, 1, 10, 0, 0, TimeSpan.Zero), Temperature = 10.0, Humidity = 50.0 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 4, 1, 11, 0, 0, TimeSpan.Zero), Temperature = 11.0, Humidity = 51.0 });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Index(
            dataSource: DataSource.Sensor,
            page: 0,
            pageSize: 50);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        Assert.Equal(1, model.CurrentPage);
        Assert.Equal(1, model.TotalPages);
        Assert.Equal(2, model.Rows.Count);
    }

    [Fact]
    public async Task Index_WithOnlyToDate_AppliesUpperBoundFilter()
    {
        await using var dbContext = CreateDbContext();

        dbContext.WeatherReadings.AddRange(
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 5, 1, 8, 0, 0, TimeSpan.Zero), Temperature = 18.0, Humidity = 60.0, CloudCover = 30.0 },
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = new DateTimeOffset(2026, 5, 2, 8, 0, 0, TimeSpan.Zero), Temperature = 19.0, Humidity = 61.0, CloudCover = 40.0 });
        await dbContext.SaveChangesAsync();

        var sut = CreateSut(dbContext);

        var result = await sut.Index(
            dataSource: DataSource.Weather,
            to: new DateOnly(2026, 5, 1),
            page: 1,
            pageSize: 50);

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateDataPageViewModel>(viewResult.Model);

        var row = Assert.Single(model.Rows);
        Assert.Equal(new DateTimeOffset(2026, 5, 1, 8, 0, 0, TimeSpan.Zero), row.Timestamp);
    }

    private static ClimateDataController CreateSut(AppDbContext dbContext)
    {
        return new ClimateDataController(dbContext, NullLogger<ClimateDataController>.Instance);
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new AppDbContext(options);
    }
}