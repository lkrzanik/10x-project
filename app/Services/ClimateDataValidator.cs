using _10xPV.Models;

namespace _10xPV.Services;

public class ClimateDataValidator : IClimateDataValidator
{
    public ClimateValidationResult Validate(SensorReading reading)
    {
        var errors = new List<string>();

        ValidateTimestamp(reading.Timestamp, errors);
        ValidateRange(reading.Temperature, -60, 60, "Temperature", errors);
        ValidateRange(reading.Humidity, 0, 100, "Humidity", errors);

        return errors.Count == 0
            ? ClimateValidationResult.Success()
            : ClimateValidationResult.Failure(errors);
    }

    public ClimateValidationResult Validate(WeatherReading reading)
    {
        var errors = new List<string>();

        ValidateTimestamp(reading.Timestamp, errors);
        ValidateRange(reading.Temperature, -60, 60, "Temperature", errors);
        ValidateRange(reading.Humidity, 0, 100, "Humidity", errors);
        ValidateRange(reading.CloudCover, 0, 100, "CloudCover", errors);

        return errors.Count == 0
            ? ClimateValidationResult.Success()
            : ClimateValidationResult.Failure(errors);
    }

    private static void ValidateTimestamp(DateTimeOffset timestamp, List<string> errors)
    {
        if (timestamp.Offset != TimeSpan.Zero)
            errors.Add("Timestamp must be UTC.");

        if (timestamp > DateTimeOffset.UtcNow)
            errors.Add("Timestamp cannot be in the future.");
    }

    private static void ValidateRange(double? value, double min, double max, string field, List<string> errors)
    {
        if (value.HasValue && (value.Value < min || value.Value > max))
            errors.Add($"{field} must be between {min} and {max}.");
    }
}