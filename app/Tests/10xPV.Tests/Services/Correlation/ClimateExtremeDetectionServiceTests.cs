using System.ComponentModel.DataAnnotations;
using _10xPV.Models.Correlation;
using _10xPV.Services.Correlation;

namespace _10xPV.Tests.Services.Correlation;

public class ClimateExtremeDetectionServiceTests
{
    private readonly ClimateExtremeDetectionService _sut = new(new ExtremeDetectionOptions
    {
        MinTemperatureC = 0,
        MaxTemperatureC = 40,
        MinHumidityPercent = 20,
        MaxHumidityPercent = 80,
        MinCloudCoverPercent = 10,
        MaxCloudCoverPercent = 90
    });

    [Fact]
    public void Detect_BelowMinimum_ReturnsExtremeWithMinimumThreshold()
    {
        var timestamp = Utc(2026, 9, 1, 10);

        var result = _sut.Detect([
            new ClimateExtremeInput(timestamp, ClimateExtremeParameter.Temperature, -1, ClimateExtremeSource.Sensor)
        ]);

        var extreme = Assert.Single(result);
        Assert.Equal(timestamp, extreme.Timestamp);
        Assert.Equal(ClimateExtremeParameter.Temperature, extreme.Parameter);
        Assert.Equal(-1, extreme.Value);
        Assert.Equal(ClimateExtremeDirection.BelowMinimum, extreme.Direction);
        Assert.Equal(0, extreme.Threshold);
        Assert.Equal(ClimateExtremeSource.Sensor, extreme.Source);
    }

    [Fact]
    public void Detect_AboveMaximum_ReturnsExtremeWithMaximumThreshold()
    {
        var result = _sut.Detect([
            new ClimateExtremeInput(Utc(2026, 9, 1, 10), ClimateExtremeParameter.Humidity, 81, ClimateExtremeSource.Weather)
        ]);

        var extreme = Assert.Single(result);
        Assert.Equal(ClimateExtremeDirection.AboveMaximum, extreme.Direction);
        Assert.Equal(80, extreme.Threshold);
        Assert.Equal(ClimateExtremeSource.Weather, extreme.Source);
    }

    [Fact]
    public void Detect_ThresholdValuesAndNulls_AreIgnored()
    {
        var timestamp = Utc(2026, 9, 1, 10);

        var result = _sut.Detect([
            new ClimateExtremeInput(timestamp, ClimateExtremeParameter.Temperature, 0, ClimateExtremeSource.Sensor),
            new ClimateExtremeInput(timestamp.AddHours(1), ClimateExtremeParameter.Temperature, 40, ClimateExtremeSource.Sensor),
            new ClimateExtremeInput(timestamp.AddHours(2), ClimateExtremeParameter.Temperature, null, ClimateExtremeSource.Sensor)
        ]);

        Assert.Empty(result);
    }

    [Fact]
    public void Detect_EmptyInput_ReturnsEmptyResult()
    {
        var result = _sut.Detect([]);

        Assert.Empty(result);
    }

    [Fact]
    public void Detect_SortsByTimestampThenParameterThenSource()
    {
        var timestamp = Utc(2026, 9, 1, 10);

        var result = _sut.Detect([
            new ClimateExtremeInput(timestamp.AddHours(1), ClimateExtremeParameter.Temperature, 50, ClimateExtremeSource.Sensor),
            new ClimateExtremeInput(timestamp, ClimateExtremeParameter.Humidity, 10, ClimateExtremeSource.Weather),
            new ClimateExtremeInput(timestamp, ClimateExtremeParameter.Temperature, 50, ClimateExtremeSource.Sensor)
        ]);

        Assert.Equal(
            [ClimateExtremeParameter.Temperature, ClimateExtremeParameter.Humidity, ClimateExtremeParameter.Temperature],
            result.Select(extreme => extreme.Parameter));
        Assert.Equal(
            [ClimateExtremeSource.Sensor, ClimateExtremeSource.Weather, ClimateExtremeSource.Sensor],
            result.Select(extreme => extreme.Source));
    }

    [Fact]
    public void ExtremeDetectionOptions_InvalidRanges_ReturnValidationErrors()
    {
        var options = new ExtremeDetectionOptions
        {
            MinTemperatureC = 40,
            MaxTemperatureC = 0
        };

        var errors = options.Validate(new ValidationContext(options)).ToList();

        Assert.Contains(errors, error => error.MemberNames.Contains(nameof(ExtremeDetectionOptions.MinTemperatureC)));
    }

    private static DateTimeOffset Utc(int year, int month, int day, int hour)
        => new(year, month, day, hour, 0, 0, TimeSpan.Zero);
}
