namespace _10xPV.Models.Correlation;

public enum ClimateExtremeParameter
{
    Temperature,
    Humidity,
    CloudCover
}

public enum ClimateExtremeDirection
{
    BelowMinimum,
    AboveMaximum
}

public enum ClimateExtremeSource
{
    Sensor,
    Weather
}

public sealed record ClimateExtremeInput(
    DateTimeOffset Timestamp,
    ClimateExtremeParameter Parameter,
    double? Value,
    ClimateExtremeSource Source);

public sealed record ClimateExtreme(
    DateTimeOffset Timestamp,
    ClimateExtremeParameter Parameter,
    double Value,
    ClimateExtremeDirection Direction,
    double Threshold,
    ClimateExtremeSource Source);
