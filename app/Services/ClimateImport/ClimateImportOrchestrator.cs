using _10xPV.Services.Csv;
using _10xPV.Services.Csv.Contracts;

namespace _10xPV.Services.ClimateImport;

public sealed class ClimateImportOrchestrator : IClimateImportOrchestrator
{
    private readonly ICsvImportService _csvImportService;
    private readonly IClimateImportPersistenceAdapter _persistenceAdapter;

    public ClimateImportOrchestrator(ICsvImportService csvImportService, IClimateImportPersistenceAdapter persistenceAdapter)
    {
        _csvImportService = csvImportService;
        _persistenceAdapter = persistenceAdapter;
    }

    public async Task<CsvImportResult> ImportAsync(
        Stream csvStream,
        CsvSchemaType schemaType,
        CancellationToken cancellationToken = default)
    {
        var importResult = await _csvImportService.ImportAsync(csvStream, schemaType, cancellationToken);
        await _persistenceAdapter.PersistValidRowsAsync(importResult, cancellationToken);

        return importResult;
    }
}
