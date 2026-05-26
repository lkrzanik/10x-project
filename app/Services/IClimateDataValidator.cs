using _10xPV.Models;

namespace _10xPV.Services;

public interface IClimateDataValidator
{
    ClimateValidationResult Validate(SensorReading reading);
    ClimateValidationResult Validate(WeatherReading reading);
}