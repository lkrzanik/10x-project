namespace _10xPV.Models.Correlation;

public sealed record ClimateCorrelationResult(
    IReadOnlyList<ClimateCorrelationAlignedPoint> Points,
    ClimateCorrelationMetadata Metadata);
