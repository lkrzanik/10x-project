namespace _10xPV.Services.Csv.Contracts;

public sealed record SensorCsvRow(
    DateTime Timestamp,
    double Temperature,
    double Humidity);