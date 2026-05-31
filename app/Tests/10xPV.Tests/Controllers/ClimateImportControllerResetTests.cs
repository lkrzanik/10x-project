using _10xPV.Controllers;
using _10xPV.Services;
using _10xPV.Services.ClimateImport;
using _10xPV.Services.Csv;
using _10xPV.Services.Csv.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace _10xPV.Tests.Controllers;

public class ClimateImportControllerResetTests
{
    [Fact]
    public async Task ResetAllData_Post_CallsDeleteAllAndRedirectsToIndex()
    {
        var mockReset = new Mock<IClimateDataResetService>();
        mockReset.Setup(s => s.DeleteAllAsync()).Returns(Task.CompletedTask);
        var sut = CreateSut(mockReset.Object);

        var result = await sut.ResetAllData();

        mockReset.Verify(s => s.DeleteAllAsync(), Times.Once);
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClimateImportController.Index), redirect.ActionName);
        Assert.Equal("Wszystkie dane klimatyczne zostały usunięte.", sut.TempData["SuccessMessage"]);
    }

    [Fact]
    public async Task ResetAllData_WhenServiceThrows_RedirectsWithError()
    {
        var mockReset = new Mock<IClimateDataResetService>();
        mockReset.Setup(s => s.DeleteAllAsync()).ThrowsAsync(new InvalidOperationException("DB error"));
        var sut = CreateSut(mockReset.Object);

        var result = await sut.ResetAllData();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal(nameof(ClimateImportController.Index), redirect.ActionName);
        Assert.Equal("Wystąpił błąd podczas usuwania danych. Spróbuj ponownie.", sut.TempData["ErrorMessage"]);
    }

    private static ClimateImportController CreateSut(IClimateDataResetService resetService)
    {
        var orchestrator = new FakeOrchestrator();
        var controller = new ClimateImportController(orchestrator, resetService, NullLogger<ClimateImportController>.Instance);
        controller.TempData = new TempDataDictionary(
            new DefaultHttpContext(),
            Mock.Of<ITempDataProvider>());
        return controller;
    }

    private sealed class FakeOrchestrator : IClimateImportOrchestrator
    {
        public Task<CsvImportResult> ImportAsync(Stream csvStream, CsvSchemaType schemaType, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CsvImportResult.Empty(schemaType));
        }
    }
}
