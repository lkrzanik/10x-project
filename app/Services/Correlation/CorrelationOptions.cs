using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace _10xPV.Services.Correlation;

/// <summary>
/// Opcje konfiguracyjne progów korelacji/interpolacji klimatycznej.
/// Wartości domyślne są robocze dla M2-1 — do potwierdzenia z użytkownikiem.
/// </summary>
public class CorrelationOptions
{
    public const string SectionName = "Correlation";

    /// <summary>
    /// Minimalny próg temperatury (°C). Wartości poniżej traktowane jako anomalia.
    /// </summary>
    [Range(-60, 60, ErrorMessage = "MinTemperatureC must be between -60 and 60.")]
    public double MinTemperatureC { get; set; } = 0;

    /// <summary>
    /// Maksymalny próg temperatury (°C). Wartości powyżej traktowane jako anomalia.
    /// </summary>
    [Range(-60, 80, ErrorMessage = "MaxTemperatureC must be between -60 and 80.")]
    public double MaxTemperatureC { get; set; } = 40;

    public void Validate()
    {
        if (MinTemperatureC >= MaxTemperatureC)
        {
            throw new OptionsValidationException(
                nameof(CorrelationOptions),
                typeof(CorrelationOptions),
                [$"MinTemperatureC ({MinTemperatureC}) must be less than MaxTemperatureC ({MaxTemperatureC})."]);
        }
    }
}
