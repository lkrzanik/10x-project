using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Services;
using Microsoft.EntityFrameworkCore;

namespace _10xPV.Tests.Services;

public class ClimateDataResetServiceTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task DeleteAllAsync_RemovesAllSensorAndWeatherReadings()
    {
        // Arrange
        await using var db = CreateInMemoryContext();
        db.SensorReadings.AddRange(
            new SensorReading { Id = Guid.NewGuid(), Timestamp = DateTimeOffset.UtcNow, Temperature = 20 },
            new SensorReading { Id = Guid.NewGuid(), Timestamp = DateTimeOffset.UtcNow, Temperature = 22 }
        );
        db.WeatherReadings.AddRange(
            new WeatherReading { Id = Guid.NewGuid(), Timestamp = DateTimeOffset.UtcNow, Temperature = 18 }
        );
        await db.SaveChangesAsync();

        var service = new ClimateDataResetService(db);

        // Act
        await service.DeleteAllAsync();

        // Assert
        Assert.Empty(await db.SensorReadings.ToListAsync());
        Assert.Empty(await db.WeatherReadings.ToListAsync());
    }

    [Fact]
    public async Task DeleteAllAsync_WorksWhenTablesAreEmpty()
    {
        // Arrange
        await using var db = CreateInMemoryContext();
        var service = new ClimateDataResetService(db);

        // Act
        await service.DeleteAllAsync();

        // Assert
        Assert.Empty(await db.SensorReadings.ToListAsync());
        Assert.Empty(await db.WeatherReadings.ToListAsync());
    }
}
