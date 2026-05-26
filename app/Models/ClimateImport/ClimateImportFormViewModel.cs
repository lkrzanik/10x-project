using _10xPV.Services.Csv.Contracts;
using Microsoft.AspNetCore.Http;

namespace _10xPV.Models.ClimateImport;

public class ClimateImportFormViewModel
{
    public IFormFile? File { get; set; }

    public CsvSchemaType? SchemaType { get; set; }
}