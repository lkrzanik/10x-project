namespace _10xPV.Models.Correlation;

public sealed record ClimateCorrelationMetadata(
    int InterpolatedCount,
    int DroppedCount,
    int OutOfRangeCount);
