using _10xPV.Models.Correlation;
using MathNet.Numerics.Interpolation;

namespace _10xPV.Services.Correlation;

public class ClimateCorrelationService : IClimateCorrelationService
{
    public ClimateCorrelationResult AlignToReferenceTimeline(
        IEnumerable<ClimateSeriesPoint> sourceSeries,
        IEnumerable<DateTimeOffset> referenceTimeline)
    {
        ArgumentNullException.ThrowIfNull(sourceSeries);
        ArgumentNullException.ThrowIfNull(referenceTimeline);

        var sourceList = sourceSeries.ToList();
        if (sourceList.Count == 0)
        {
            return new ClimateCorrelationResult(
                Points: [],
                Metadata: new ClimateCorrelationMetadata(
                    InterpolatedCount: 0,
                    DroppedCount: 0,
                    OutOfRangeCount: 0));
        }

        var referenceList = referenceTimeline.ToList();

        var nonNullPoints = sourceList
            .Where(point => point.Value.HasValue)
            .OrderBy(point => point.Timestamp)
            .ToList();

        var droppedForNull = sourceList.Count - nonNullPoints.Count;

        var normalizedPoints = new List<ClimateSeriesPoint>(nonNullPoints.Count);
        var droppedDuplicates = 0;

        DateTimeOffset? previousTimestamp = null;
        foreach (var point in nonNullPoints)
        {
            if (previousTimestamp.HasValue && point.Timestamp == previousTimestamp.Value)
            {
                droppedDuplicates++;
                continue;
            }

            normalizedPoints.Add(point);
            previousTimestamp = point.Timestamp;
        }

        var droppedCount = droppedForNull + droppedDuplicates;

        if (normalizedPoints.Count == 0)
        {
            return new ClimateCorrelationResult(
                Points: [],
                Metadata: new ClimateCorrelationMetadata(
                    InterpolatedCount: 0,
                    DroppedCount: droppedCount,
                    OutOfRangeCount: 0));
        }

        var exactPoints = normalizedPoints.ToDictionary(point => point.Timestamp, point => point.Value!.Value);

        var minTimestamp = normalizedPoints[0].Timestamp;
        var maxTimestamp = normalizedPoints[^1].Timestamp;

        var alignedPoints = new List<ClimateCorrelationAlignedPoint>(referenceList.Count);
        var interpolatedCount = 0;
        var outOfRangeCount = 0;

        if (normalizedPoints.Count == 1)
        {
            var singlePoint = normalizedPoints[0];

            foreach (var timestamp in referenceList)
            {
                if (timestamp == singlePoint.Timestamp)
                {
                    alignedPoints.Add(new ClimateCorrelationAlignedPoint(timestamp, singlePoint.Value, IsInterpolated: false));
                    continue;
                }

                alignedPoints.Add(new ClimateCorrelationAlignedPoint(timestamp, Value: null, IsInterpolated: false));
                outOfRangeCount++;
            }

            return new ClimateCorrelationResult(
                Points: alignedPoints,
                Metadata: new ClimateCorrelationMetadata(
                    InterpolatedCount: interpolatedCount,
                    DroppedCount: droppedCount,
                    OutOfRangeCount: outOfRangeCount));
        }

        var xValues = normalizedPoints.Select(point => ToAxis(point.Timestamp)).ToArray();
        var yValues = normalizedPoints.Select(point => point.Value!.Value).ToArray();
        var spline = LinearSpline.InterpolateSorted(xValues, yValues);

        foreach (var timestamp in referenceList)
        {
            if (exactPoints.TryGetValue(timestamp, out var exactValue))
            {
                alignedPoints.Add(new ClimateCorrelationAlignedPoint(timestamp, exactValue, IsInterpolated: false));
                continue;
            }

            if (timestamp < minTimestamp || timestamp > maxTimestamp)
            {
                alignedPoints.Add(new ClimateCorrelationAlignedPoint(timestamp, Value: null, IsInterpolated: false));
                outOfRangeCount++;
                continue;
            }

            var interpolatedValue = spline.Interpolate(ToAxis(timestamp));
            alignedPoints.Add(new ClimateCorrelationAlignedPoint(timestamp, interpolatedValue, IsInterpolated: true));
            interpolatedCount++;
        }

        return new ClimateCorrelationResult(
            Points: alignedPoints,
            Metadata: new ClimateCorrelationMetadata(
                InterpolatedCount: interpolatedCount,
                DroppedCount: droppedCount,
                OutOfRangeCount: outOfRangeCount));
    }

    private static double ToAxis(DateTimeOffset timestamp)
        => timestamp.ToUnixTimeMilliseconds();
}
