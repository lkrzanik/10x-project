using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Services.ClimateImport;

public interface IClimateImportPersistenceAdapter
{
    Task PersistValidRowsAsync(CsvImportResult importResult, CancellationToken cancellationToken = default);
}
