namespace _10xPV.Services.Csv.Contracts;

public sealed record SensorCsvRow(
    DateTime Timestamp,
    string SensorId,
    double Value,
    string Unit);