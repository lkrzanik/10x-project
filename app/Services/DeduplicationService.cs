using Microsoft.EntityFrameworkCore;
using _10xPV.Data;
using _10xPV.Models;

namespace _10xPV.Services;

public class DeduplicationService : IDeduplicationService
{
    private readonly AppDbContext _db;

    public DeduplicationService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsAsync(DateTimeOffset timestamp, DataSource source)
    {
        return source switch
        {
            DataSource.Sensor => await _db.SensorReadings.AnyAsync(r => r.Timestamp == timestamp),
            DataSource.Weather => await _db.WeatherReadings.AnyAsync(r => r.Timestamp == timestamp),
            _ => throw new ArgumentOutOfRangeException(nameof(source))
        };
    }
}