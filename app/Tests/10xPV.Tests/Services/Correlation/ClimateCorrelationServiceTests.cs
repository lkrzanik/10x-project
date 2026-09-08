using _10xPV.Models.Correlation;
using _10xPV.Services.Correlation;

namespace _10xPV.Tests.Services.Correlation;

public class ClimateCorrelationServiceTests
{
    private readonly ClimateCorrelationService _sut = new();

    [Fact]
    public void AlignToReferenceTimeline_NullSourceSeries_ThrowsArgumentNullException()
    {
        var referenceTimeline = new[] { Utc(2026, 5, 1, 0) };

        var action = () => _sut.AlignToReferenceTimeline(null!, referenceTimeline);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void AlignToReferenceTimeline_NullReferenceTimeline_ThrowsArgumentNullException()
    {
        var sourceSeries = new[] { new ClimateSeriesPoint(Utc(2026, 5, 1, 0), 10.0) };

        var action = () => _sut.AlignToReferenceTimeline(sourceSeries, null!);

        Assert.Throws<ArgumentNullException>(action);
    }

    [Fact]
    public void AlignToReferenceTimeline_HappyPath_InterpolatesLinearlyForMissingPoints()
    {
        var t0 = Utc(2026, 5, 1, 0);
        var t1 = Utc(2026, 5, 1, 1);
        var t2 = Utc(2026, 5, 1, 2);

        var sourceSeries = new[]
        {
            new ClimateSeriesPoint(t0, 10.0),
            new ClimateSeriesPoint(t2, 14.0)
        };

        var result = _sut.AlignToReferenceTimeline(sourceSeries, new[] { t0, t1, t2 });

        Assert.Equal(3, result.Points.Count);

        Assert.Equal(t0, result.Points[0].Timestamp);
        AssertValue(10.0, result.Points[0].Value);
        Assert.False(result.Points[0].IsInterpolated);

        Assert.Equal(t1, result.Points[1].Timestamp);
        AssertValue(12.0, result.Points[1].Value);
        Assert.True(result.Points[1].IsInterpolated);

        Assert.Equal(t2, result.Points[2].Timestamp);
        AssertValue(14.0, result.Points[2].Value);
        Assert.False(result.Points[2].IsInterpolated);

        Assert.Equal(1, result.Metadata.InterpolatedCount);
        Assert.Equal(0, result.Metadata.DroppedCount);
        Assert.Equal(0, result.Metadata.OutOfRangeCount);
    }

    [Fact]
    public void AlignToReferenceTimeline_DailySeriesAgainstThirtyMinuteTimeline_UsesStepwiseDailyValue()
    {
        var dayOne = new DateTimeOffset(2026, 5, 1, 12, 0, 0, TimeSpan.Zero);
        var dayTwo = new DateTimeOffset(2026, 5, 2, 12, 0, 0, TimeSpan.Zero);

        var sourceSeries = new[]
        {
            new ClimateSeriesPoint(dayOne, 10.0),
            new ClimateSeriesPoint(dayTwo, 12.0)
        };

        var referenceTimeline = new[]
        {
            new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 1, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 1, 12, 30, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 2, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 2, 12, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 5, 2, 12, 30, 0, TimeSpan.Zero)
        };

        var result = _sut.AlignToReferenceTimeline(sourceSeries, referenceTimeline);

        Assert.Equal(6, result.Points.Count);
        Assert.Equal(10.0, result.Points[0].Value);
        Assert.Equal(10.0, result.Points[1].Value);
        Assert.Equal(10.0, result.Points[2].Value);
        Assert.Equal(12.0, result.Points[3].Value);
        Assert.Equal(12.0, result.Points[4].Value);
        Assert.Equal(12.0, result.Points[5].Value);

        Assert.All(result.Points, point => Assert.False(point.IsInterpolated));
        Assert.Equal(0, result.Metadata.InterpolatedCount);
        Assert.Equal(0, result.Metadata.OutOfRangeCount);
    }

    [Fact]
    public void AlignToReferenceTimeline_EmptySource_ReturnsEmptyResultWithZeroMetadata()
    {
        var referenceTimeline = new[]
        {
            Utc(2026, 5, 2, 0),
            Utc(2026, 5, 2, 1)
        };

        var result = _sut.AlignToReferenceTimeline(Array.Empty<ClimateSeriesPoint>(), referenceTimeline);

        Assert.Empty(result.Points);
        Assert.Equal(0, result.Metadata.InterpolatedCount);
        Assert.Equal(0, result.Metadata.DroppedCount);
        Assert.Equal(0, result.Metadata.OutOfRangeCount);
    }

    [Fact]
    public void AlignToReferenceTimeline_SingleSourcePoint_ReturnsValueOnlyForExactTimestamp()
    {
        var exactTimestamp = Utc(2026, 5, 3, 10);
        var sourceSeries = new[]
        {
            new ClimateSeriesPoint(exactTimestamp, 21.5)
        };

        var referenceTimeline = new[]
        {
            exactTimestamp.AddHours(-1),
            exactTimestamp,
            exactTimestamp.AddHours(1)
        };

        var result = _sut.AlignToReferenceTimeline(sourceSeries, referenceTimeline);

        Assert.Equal(3, result.Points.Count);

        Assert.Null(result.Points[0].Value);
        Assert.False(result.Points[0].IsInterpolated);

        AssertValue(21.5, result.Points[1].Value);
        Assert.False(result.Points[1].IsInterpolated);

        Assert.Null(result.Points[2].Value);
        Assert.False(result.Points[2].IsInterpolated);

        Assert.Equal(0, result.Metadata.InterpolatedCount);
        Assert.Equal(0, result.Metadata.DroppedCount);
        Assert.Equal(2, result.Metadata.OutOfRangeCount);
    }

    [Fact]
    public void AlignToReferenceTimeline_ReferencePointOutsideRange_ReturnsNullWithoutExtrapolation()
    {
        var t0 = Utc(2026, 5, 4, 0);
        var t1 = Utc(2026, 5, 4, 1);

        var sourceSeries = new[]
        {
            new ClimateSeriesPoint(t0, 0.0),
            new ClimateSeriesPoint(t1, 10.0)
        };

        var referenceTimeline = new[]
        {
            t0.AddHours(-1),
            t0.AddMinutes(30),
            t1.AddHours(1)
        };

        var result = _sut.AlignToReferenceTimeline(sourceSeries, referenceTimeline);

        Assert.Equal(3, result.Points.Count);
        Assert.Null(result.Points[0].Value);
        Assert.False(result.Points[0].IsInterpolated);

        AssertValue(5.0, result.Points[1].Value);
        Assert.True(result.Points[1].IsInterpolated);

        Assert.Null(result.Points[2].Value);
        Assert.False(result.Points[2].IsInterpolated);

        Assert.Equal(1, result.Metadata.InterpolatedCount);
        Assert.Equal(0, result.Metadata.DroppedCount);
        Assert.Equal(2, result.Metadata.OutOfRangeCount);
    }

    [Fact]
    public void AlignToReferenceTimeline_NullAndDuplicatePoints_DropsInvalidInputAndReturnsExpectedMetadata()
    {
        var t0 = Utc(2026, 5, 5, 0);
        var t1 = Utc(2026, 5, 5, 1);
        var t2 = Utc(2026, 5, 5, 2);
        var t3 = Utc(2026, 5, 5, 3);

        var sourceSeries = new[]
        {
            new ClimateSeriesPoint(t0, null),
            new ClimateSeriesPoint(t1, 10.0),
            new ClimateSeriesPoint(t1, 11.0),
            new ClimateSeriesPoint(t2, null),
            new ClimateSeriesPoint(t3, 30.0)
        };

        var result = _sut.AlignToReferenceTimeline(sourceSeries, new[] { t1, t2, t3 });

        Assert.Equal(3, result.Points.Count);

        AssertValue(10.0, result.Points[0].Value);
        Assert.False(result.Points[0].IsInterpolated);

        AssertValue(20.0, result.Points[1].Value);
        Assert.True(result.Points[1].IsInterpolated);

        AssertValue(30.0, result.Points[2].Value);
        Assert.False(result.Points[2].IsInterpolated);

        Assert.Equal(1, result.Metadata.InterpolatedCount);
        Assert.Equal(3, result.Metadata.DroppedCount);
        Assert.Equal(0, result.Metadata.OutOfRangeCount);
    }

    private static DateTimeOffset Utc(int year, int month, int day, int hour)
        => new(year, month, day, hour, 0, 0, TimeSpan.Zero);

    private static void AssertValue(double expected, double? actual, int precision = 6)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected, actual.Value, precision);
    }
}
