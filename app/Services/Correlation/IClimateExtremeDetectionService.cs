using _10xPV.Models.Correlation;

namespace _10xPV.Services.Correlation;

public interface IClimateExtremeDetectionService
{
    IReadOnlyList<ClimateExtreme> Detect(IEnumerable<ClimateExtremeInput> readings);
}
