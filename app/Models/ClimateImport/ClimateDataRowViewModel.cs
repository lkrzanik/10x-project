namespace _10xPV.Models.ClimateImport;

public class ClimateDataRowViewModel
{
    public DateTimeOffset Timestamp { get; init; }

    public double? Temperature { get; init; }

    public double? Humidity { get; init; }

    public double? CloudCover { get; init; }
}