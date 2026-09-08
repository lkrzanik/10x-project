namespace _10xPV.Models.Correlation;

public class CorrelationViewModel
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public string Parameter { get; set; } = "Temperature";
    public IReadOnlyList<ClimateCorrelationAlignedPoint> SensorPoints { get; set; } = [];
    public IReadOnlyList<ClimateCorrelationAlignedPoint> WeatherPoints { get; set; } = [];
    public ClimateCorrelationMetadata? SensorMetadata { get; set; }
    public ClimateCorrelationMetadata? WeatherMetadata { get; set; }
    public int TotalAlignedPoints { get; set; }
    public bool HasResults { get; set; }
    public IReadOnlyList<ClimateExtreme> Extremes { get; set; } = [];
    public bool HasExtremes => Extremes.Count > 0;
}
