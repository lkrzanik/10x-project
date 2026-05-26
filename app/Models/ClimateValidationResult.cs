namespace _10xPV.Models;

public record ClimateValidationResult(bool IsValid, IReadOnlyList<string> Errors)
{
    public static ClimateValidationResult Success() => new(true, []);
    public static ClimateValidationResult Failure(List<string> errors) => new(false, errors);
}