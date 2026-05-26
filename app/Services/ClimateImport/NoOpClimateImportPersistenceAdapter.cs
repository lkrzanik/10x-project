using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Services.ClimateImport;

public sealed class NoOpClimateImportPersistenceAdapter : IClimateImportPersistenceAdapter
{
    public Task PersistValidRowsAsync(CsvImportResult importResult, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
