namespace _10xPV.Models.ClimateImport;

public class ClimateDataPageViewModel
{
    public ClimateDataFilterViewModel Filter { get; init; } = new();

    public IReadOnlyList<ClimateDataRowViewModel> Rows { get; init; } = [];

    public int TotalCount { get; init; }

    public int CurrentPage { get; init; } = 1;

    public int TotalPages { get; init; }

    public int PageSize { get; init; } = 50;

    public bool HasPreviousPage { get; init; }

    public bool HasNextPage { get; init; }
}