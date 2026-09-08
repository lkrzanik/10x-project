namespace _10xPV.Models.Correlation;

public class ClimateExtremeViewModel
{
    public DateOnly? From { get; set; }
    public DateOnly? To { get; set; }
    public IReadOnlyList<ClimateExtreme> Extremes { get; set; } = [];
    public bool HasResults => Extremes.Count > 0;
}
