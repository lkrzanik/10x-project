using _10xPV.Models;

namespace _10xPV.Models.ClimateImport;

public class ClimateDataFilterViewModel
{
    public DataSource DataSource { get; init; } = DataSource.Sensor;

    public DateOnly? From { get; init; }

    public DateOnly? To { get; init; }

    public int Page { get; init; } = 1;

    public int PageSize { get; init; } = 50;
}