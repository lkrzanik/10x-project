namespace _10xPV.Services.Csv.Contracts;

public sealed record WeatherCsvRow(
    DateTime Timestamp,
    double TemperatureC,
    double WindSpeedMs,
    double IrradianceWm2);