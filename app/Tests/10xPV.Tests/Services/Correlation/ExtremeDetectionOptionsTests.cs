using _10xPV.Services.Correlation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace _10xPV.Tests.Services.Correlation;

public class ExtremeDetectionOptionsTests
{
    [Fact]
    public void Options_BindsAllThresholdsFromConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"{ExtremeDetectionOptions.SectionName}:MinTemperatureC"] = "-10",
                [$"{ExtremeDetectionOptions.SectionName}:MaxTemperatureC"] = "35",
                [$"{ExtremeDetectionOptions.SectionName}:MinHumidityPercent"] = "20",
                [$"{ExtremeDetectionOptions.SectionName}:MaxHumidityPercent"] = "80",
                [$"{ExtremeDetectionOptions.SectionName}:MinCloudCoverPercent"] = "10",
                [$"{ExtremeDetectionOptions.SectionName}:MaxCloudCoverPercent"] = "90"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddOptions<ExtremeDetectionOptions>()
            .Bind(configuration.GetSection(ExtremeDetectionOptions.SectionName))
            .ValidateDataAnnotations();

        var options = services.BuildServiceProvider()
            .GetRequiredService<IOptions<ExtremeDetectionOptions>>()
            .Value;

        Assert.Equal(-10, options.MinTemperatureC);
        Assert.Equal(35, options.MaxTemperatureC);
        Assert.Equal(20, options.MinHumidityPercent);
        Assert.Equal(80, options.MaxHumidityPercent);
        Assert.Equal(10, options.MinCloudCoverPercent);
        Assert.Equal(90, options.MaxCloudCoverPercent);
    }

    [Fact]
    public void Options_InvalidThresholdOrder_ThrowsWhenValueIsResolved()
    {
        var services = new ServiceCollection();
        services.AddOptions<ExtremeDetectionOptions>()
            .Configure(options =>
            {
                options.MinTemperatureC = 40;
                options.MaxTemperatureC = 0;
            })
            .ValidateDataAnnotations();

        var provider = services.BuildServiceProvider();

        Assert.Throws<OptionsValidationException>(() =>
            provider.GetRequiredService<IOptions<ExtremeDetectionOptions>>().Value);
    }

    [Fact]
    public void ServiceRegistration_ResolvesExtremeDetectionService()
    {
        var services = new ServiceCollection();
        services.AddOptions<ExtremeDetectionOptions>();
        services.AddScoped<IClimateExtremeDetectionService>(serviceProvider =>
            new ClimateExtremeDetectionService(
                serviceProvider.GetRequiredService<IOptions<ExtremeDetectionOptions>>().Value));

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var service = scope.ServiceProvider.GetRequiredService<IClimateExtremeDetectionService>();

        Assert.IsType<ClimateExtremeDetectionService>(service);
    }
}