using _10xPV.Controllers;
using _10xPV.Models.ClimateImport;
using _10xPV.Services.Csv.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace _10xPV.Tests.Controllers;

public class ClimateImportControllerTests
{
    [Fact]
    public void Index_Get_ReturnsViewWithFormModel()
    {
        var sut = new ClimateImportController();

        var result = sut.Index();

        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.NotNull(model.Form);
    }

    [Fact]
    public void Index_Post_WithoutFile_ReturnsValidationError()
    {
        var sut = new ClimateImportController();
        var model = new ClimateImportFormViewModel
        {
            SchemaType = CsvSchemaType.Sensor,
            File = null
        };

        var result = sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(sut.ModelState.IsValid);
        Assert.True(sut.ModelState.ContainsKey(nameof(ClimateImportFormViewModel.File)));

        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.Same(model, pageModel.Form);
    }

    [Fact]
    public void Index_Post_WithoutSchema_ReturnsValidationError()
    {
        var sut = new ClimateImportController();
        var file = CreateFormFile("import.csv", "Timestamp,SensorId,Value,Unit\n2026-01-01,SEN-01,1.2,kWh\n");
        var model = new ClimateImportFormViewModel
        {
            SchemaType = null,
            File = file
        };

        var result = sut.Index(model);

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.False(sut.ModelState.IsValid);
        Assert.True(sut.ModelState.ContainsKey(nameof(ClimateImportFormViewModel.SchemaType)));

        var pageModel = Assert.IsType<ClimateImportPageViewModel>(viewResult.Model);
        Assert.Same(model, pageModel.Form);
    }

    private static IFormFile CreateFormFile(string fileName, string content)
    {
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        return new FormFile(stream, 0, stream.Length, "File", fileName);
    }
}