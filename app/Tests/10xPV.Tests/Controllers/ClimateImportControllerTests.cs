using _10xPV.Controllers;
using _10xPV.Models.ClimateImport;
using _10xPV.Services.Csv;
using _10xPV.Services.Csv.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace _10xPV.Tests.Controllers;

public class ClimateImportControllerTests
{
    [Fact]
    public void Index_Get_ReturnsViewWithFormModel()
    {
        var sut = CreateSut();

        var result = sut.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.NotNull(model.Form);
    }

    [Fact]
    public async Task Index_Post_WithoutFile_ReturnsValidationError()
    {
        var sut = CreateSut();
        var model = new ClimateImportFormViewModel
        {
            SchemaType = CsvSchemaType.Sensor,
            File = null
        };

        var result = await sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(sut.ModelState.IsValid);
        Assert.True(sut.ModelState.ContainsKey(nameof(ClimateImportFormViewModel.File)));

        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.Same(model, pageModel.Form);
    }

    [Fact]
    public async Task Index_Post_WithoutSchema_ReturnsValidationError()
    {
        var sut = CreateSut();
        var file = CreateFormFile("import.csv", "Timestamp,Temperature,Humidity\n2026-01-01,21.2,55.0\n");
        var model = new ClimateImportFormViewModel
        {
            SchemaType = null,
            File = file
        };

        var result = await sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(sut.ModelState.IsValid);
        Assert.True(sut.ModelState.ContainsKey(nameof(ClimateImportFormViewModel.SchemaType)));

        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.Same(model, pageModel.Form);
    }

    [Fact]
    public async Task Index_Post_ValidForm_MapsImportSummaryAndErrors()
    {
        var importResult = new CsvImportResult(
            CsvSchemaType.Sensor,
            [],
            [],
            [new CsvRowError(3, "Temperature", "ROW_NUMBER_INVALID", "Niepoprawna wartość liczby.")],
            TotalRows: 2,
            ValidRows: 1,
            InvalidRows: 1);

        var sut = CreateSut((_, _, _) => Task.FromResult(importResult));
        var model = new ClimateImportFormViewModel
        {
            SchemaType = CsvSchemaType.Sensor,
            File = CreateFormFile("sensor.csv", "Timestamp,Temperature,Humidity\n2026-01-01,10.4,58.0\n2026-01-02,abc,59.0\n")
        };

        var result = await sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);

        Assert.NotNull(pageModel.Summary);
        Assert.Equal(2, pageModel.Summary.TotalRows);
        Assert.Equal(1, pageModel.Summary.ValidRows);
        Assert.Equal(1, pageModel.Summary.InvalidRows);
        Assert.Equal("Import zakończony częściowym sukcesem.", pageModel.Summary.StatusMessage);

        var firstError = Assert.Single(pageModel.Errors);
        Assert.Equal(3, firstError.LineNumber);
        Assert.Equal("Temperature", firstError.Field);
        Assert.Equal("ROW_NUMBER_INVALID", firstError.Code);
    }

    [Fact]
    public async Task Index_Post_WhenImportCancelled_ReturnsUserFacingError()
    {
        var sut = CreateSut((_, _, _) => throw new OperationCanceledException());
        var model = new ClimateImportFormViewModel
        {
            SchemaType = CsvSchemaType.Sensor,
            File = CreateFormFile("sensor.csv", "Timestamp,Temperature,Humidity\n2026-01-01,10.4,58.0\n")
        };

        var result = await sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(sut.ModelState.IsValid);

        var modelStateEntry = Assert.Single(sut.ModelState[string.Empty]!.Errors);
        Assert.Equal("Import anulowany.", modelStateEntry.ErrorMessage);

        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.Same(model, pageModel.Form);
        Assert.Null(pageModel.Summary);
    }

    private static ClimateImportController CreateSut(
        Func<Stream, CsvSchemaType, CancellationToken, Task<CsvImportResult>>? importAsync = null)
    {
        var service = new FakeCsvImportService(importAsync ?? ((_, schemaType, _) => Task.FromResult(CsvImportResult.Empty(schemaType))));
        return new ClimateImportController(service, NullLogger<ClimateImportController>.Instance);
    }

    private static IFormFile CreateFormFile(string fileName, string content)
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "File", fileName);
    }

    private sealed class FakeCsvImportService : ICsvImportService
    {
        private readonly Func<Stream, CsvSchemaType, CancellationToken, Task<CsvImportResult>> _importAsync;

        public FakeCsvImportService(Func<Stream, CsvSchemaType, CancellationToken, Task<CsvImportResult>> importAsync)
        {
            _importAsync = importAsync;
        }

        public Task<CsvImportResult> ImportAsync(Stream csvStream, CsvSchemaType schemaType, CancellationToken cancellationToken = default)
        {
            return _importAsync(csvStream, schemaType, cancellationToken);
        }
    }
}