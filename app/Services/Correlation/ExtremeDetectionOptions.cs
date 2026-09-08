using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace _10xPV.Services.Correlation;

public sealed class ExtremeDetectionOptions : IValidatableObject
{
    public const string SectionName = "ExtremeDetection";

    [Range(-60, 60)]
    public double MinTemperatureC { get; set; } = 0;

    [Range(-60, 80)]
    public double MaxTemperatureC { get; set; } = 40;

    [Range(0, 100)]
    public double MinHumidityPercent { get; set; } = 0;

    [Range(0, 100)]
    public double MaxHumidityPercent { get; set; } = 100;

    [Range(0, 100)]
    public double MinCloudCoverPercent { get; set; } = 0;

    [Range(0, 100)]
    public double MaxCloudCoverPercent { get; set; } = 100;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var (minimumName, minimum, maximumName, maximum) in new[]
        {
            (nameof(MinTemperatureC), MinTemperatureC, nameof(MaxTemperatureC), MaxTemperatureC),
            (nameof(MinHumidityPercent), MinHumidityPercent, nameof(MaxHumidityPercent), MaxHumidityPercent),
            (nameof(MinCloudCoverPercent), MinCloudCoverPercent, nameof(MaxCloudCoverPercent), MaxCloudCoverPercent)
        })
        {
            if (minimum >= maximum)
            {
                yield return new ValidationResult(
                    $"{minimumName} ({minimum}) must be less than {maximumName} ({maximum}).",
                    [minimumName, maximumName]);
            }
        }
    }

    public (double Minimum, double Maximum) GetThresholds(Models.Correlation.ClimateExtremeParameter parameter)
        => parameter switch
        {
            Models.Correlation.ClimateExtremeParameter.Temperature => (MinTemperatureC, MaxTemperatureC),
            Models.Correlation.ClimateExtremeParameter.Humidity => (MinHumidityPercent, MaxHumidityPercent),
            Models.Correlation.ClimateExtremeParameter.CloudCover => (MinCloudCoverPercent, MaxCloudCoverPercent),
            _ => throw new ArgumentOutOfRangeException(nameof(parameter), parameter, "Unsupported climate parameter.")
        };
}
