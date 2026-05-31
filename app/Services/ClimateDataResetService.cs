using _10xPV.Data;

namespace _10xPV.Services;

public class ClimateDataResetService : IClimateDataResetService
{
    private readonly AppDbContext _db;

    public ClimateDataResetService(AppDbContext db)
    {
        _db = db;
    }

    public async Task DeleteAllAsync()
    {
        _db.SensorReadings.RemoveRange(_db.SensorReadings);
        _db.WeatherReadings.RemoveRange(_db.WeatherReadings);
        await _db.SaveChangesAsync();
    }
}
