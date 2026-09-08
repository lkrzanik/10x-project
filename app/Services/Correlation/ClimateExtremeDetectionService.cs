using _10xPV.Models.Correlation;

namespace _10xPV.Services.Correlation;

public sealed class ClimateExtremeDetectionService : IClimateExtremeDetectionService
{
    private readonly ExtremeDetectionOptions _options;

    public ClimateExtremeDetectionService(ExtremeDetectionOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options;
    }

    public IReadOnlyList<ClimateExtreme> Detect(IEnumerable<ClimateExtremeInput> readings)
    {
        ArgumentNullException.ThrowIfNull(readings);

        var extremes = new List<ClimateExtreme>();
        foreach (var reading in readings)
        {
            if (!reading.Value.HasValue)
            {
                continue;
            }

            var (minimum, maximum) = _options.GetThresholds(reading.Parameter);
            var value = reading.Value.Value;

            if (value < minimum)
            {
                extremes.Add(new ClimateExtreme(
                    reading.Timestamp,
                    reading.Parameter,
                    value,
                    ClimateExtremeDirection.BelowMinimum,
                    minimum,
                    reading.Source));
            }
            else if (value > maximum)
            {
                extremes.Add(new ClimateExtreme(
                    reading.Timestamp,
                    reading.Parameter,
                    value,
                    ClimateExtremeDirection.AboveMaximum,
                    maximum,
                    reading.Source));
            }
        }

        return extremes
            .OrderBy(extreme => extreme.Timestamp)
            .ThenBy(extreme => extreme.Parameter)
            .ThenBy(extreme => extreme.Source)
            .ToArray();
    }
}
