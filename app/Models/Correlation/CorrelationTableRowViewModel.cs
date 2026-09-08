namespace _10xPV.Models.Correlation;

public sealed record CorrelationTableRowViewModel(
    DateTimeOffset Timestamp,
    double? SensorValue,
    bool SensorIsInterpolated,
    double? WeatherValue,
    bool WeatherIsInterpolated);