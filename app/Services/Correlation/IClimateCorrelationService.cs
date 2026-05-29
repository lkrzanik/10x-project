using _10xPV.Models.Correlation;

namespace _10xPV.Services.Correlation;

public interface IClimateCorrelationService
{
    ClimateCorrelationResult AlignToReferenceTimeline(
        IEnumerable<ClimateSeriesPoint> sourceSeries,
        IEnumerable<DateTimeOffset> referenceTimeline);
}
