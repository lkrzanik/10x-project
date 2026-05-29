namespace _10xPV.Models.Correlation;

public sealed record ClimateCorrelationAlignedPoint(
    DateTimeOffset Timestamp,
    double? Value,
    bool IsInterpolated);
