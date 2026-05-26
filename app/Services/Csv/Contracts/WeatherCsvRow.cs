namespace _10xPV.Services.Csv.Contracts;

public sealed record WeatherCsvRow(
    DateTime Timestamp,
    double Temperature,
    double Humidity,
    double CloudCover);