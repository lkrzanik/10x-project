using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Models.Correlation;
using _10xPV.Services.Correlation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _10xPV.Controllers;

public class ClimateCorrelationController : Controller
{
    private readonly AppDbContext _dbContext;
    private readonly IClimateCorrelationService _correlationService;
    private readonly IClimateExtremeDetectionService _extremeDetectionService;
    private readonly ILogger<ClimateCorrelationController> _logger;

    public ClimateCorrelationController(
        AppDbContext dbContext,
        IClimateCorrelationService correlationService,
        IClimateExtremeDetectionService extremeDetectionService,
        ILogger<ClimateCorrelationController> logger)
    {
        _dbContext = dbContext;
        _correlationService = correlationService;
        _extremeDetectionService = extremeDetectionService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Correlation(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var viewModel = new CorrelationViewModel { From = from, To = to };

        if (from.HasValue && to.HasValue && from > to)
        {
            ModelState.AddModelError(string.Empty, "Data początkowa nie może być późniejsza niż data końcowa.");
            return View(viewModel);
        }

        try
        {
            var sensorQuery = _dbContext.SensorReadings.AsNoTracking();
            var weatherQuery = _dbContext.WeatherReadings.AsNoTracking();

            sensorQuery = ApplyDateFilter(sensorQuery, from, to);
            weatherQuery = ApplyDateFilter(weatherQuery, from, to);

            var sensorPoints = await sensorQuery
                .Where(r => r.Temperature != null)
                .OrderBy(r => r.Timestamp)
                .Select(r => new ClimateSeriesPoint(r.Timestamp, r.Temperature))
                .ToListAsync(cancellationToken);

            var weatherPoints = await weatherQuery
                .Where(r => r.Temperature != null)
                .OrderBy(r => r.Timestamp)
                .Select(r => new ClimateSeriesPoint(r.Timestamp, r.Temperature))
                .ToListAsync(cancellationToken);

            var sensorExtremeRows = await sensorQuery
                .Select(r => new { r.Timestamp, r.Temperature, r.Humidity })
                .ToListAsync(cancellationToken);

            var weatherExtremeRows = await weatherQuery
                .Select(r => new { r.Timestamp, r.Temperature, r.Humidity, r.CloudCover })
                .ToListAsync(cancellationToken);

            var extremeInputs = sensorExtremeRows
                .SelectMany(row => new[]
                {
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Temperature, row.Temperature, ClimateExtremeSource.Sensor),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Humidity, row.Humidity, ClimateExtremeSource.Sensor)
                })
                .Concat(weatherExtremeRows.SelectMany(row => new[]
                {
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Temperature, row.Temperature, ClimateExtremeSource.Weather),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Humidity, row.Humidity, ClimateExtremeSource.Weather),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.CloudCover, row.CloudCover, ClimateExtremeSource.Weather)
                }));

            viewModel.Extremes = _extremeDetectionService.Detect(extremeInputs);

            var weatherTimeline = weatherPoints.Select(p => p.Timestamp).ToList();
            var sensorTimeline = sensorPoints.Select(p => p.Timestamp).ToList();

            var sensorResult = _correlationService.AlignToReferenceTimeline(sensorPoints, weatherTimeline);
            var weatherResult = _correlationService.AlignToReferenceTimeline(weatherPoints, sensorTimeline);

            viewModel.SensorPoints = sensorResult.Points;
            viewModel.WeatherPoints = weatherResult.Points;
            viewModel.TableRows = BuildCorrelationTableRows(sensorResult.Points, weatherResult.Points);
            viewModel.SensorMetadata = sensorResult.Metadata;
            viewModel.WeatherMetadata = weatherResult.Metadata;
            viewModel.TotalAlignedPoints = sensorResult.Points.Count + weatherResult.Points.Count;
            viewModel.HasResults = sensorResult.Points.Count > 0 || weatherResult.Points.Count > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas obliczania korelacji danych klimatycznych.");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas obliczania korelacji. Spróbuj ponownie.");
        }

        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Extremes(
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var viewModel = new ClimateExtremeViewModel { From = from, To = to };

        if (from.HasValue && to.HasValue && from > to)
        {
            ModelState.AddModelError(string.Empty, "Data początkowa nie może być późniejsza niż data końcowa.");
            return View(viewModel);
        }

        try
        {
            var query = _dbContext.SensorReadings.AsNoTracking();
            query = ApplyDateFilter(query, from, to);

            var sensorRows = await query
                .Select(r => new { r.Timestamp, r.Temperature, r.Humidity })
                .ToListAsync(cancellationToken);

            var weatherQuery = _dbContext.WeatherReadings.AsNoTracking();
            weatherQuery = ApplyDateFilter(weatherQuery, from, to);

            var weatherRows = await weatherQuery
                .Select(r => new { r.Timestamp, r.Temperature, r.Humidity, r.CloudCover })
                .ToListAsync(cancellationToken);

            var extremeInputs = sensorRows
                .SelectMany(row => new[]
                {
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Temperature, row.Temperature, ClimateExtremeSource.Sensor),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Humidity, row.Humidity, ClimateExtremeSource.Sensor)
                })
                .Concat(weatherRows.SelectMany(row => new[]
                {
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Temperature, row.Temperature, ClimateExtremeSource.Weather),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.Humidity, row.Humidity, ClimateExtremeSource.Weather),
                    new ClimateExtremeInput(row.Timestamp, ClimateExtremeParameter.CloudCover, row.CloudCover, ClimateExtremeSource.Weather)
                }));

            viewModel.Extremes = _extremeDetectionService.Detect(extremeInputs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas obliczania ekstremów klimatycznych.");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas odczytu ekstremów. Spróbuj ponownie.");
        }

        return View(viewModel);
    }

    private static IReadOnlyList<CorrelationTableRowViewModel> BuildCorrelationTableRows(
        IReadOnlyList<ClimateCorrelationAlignedPoint> sensorPoints,
        IReadOnlyList<ClimateCorrelationAlignedPoint> weatherPoints)
    {
        var sensorByTimestamp = sensorPoints.ToDictionary(point => point.Timestamp);
        var weatherByTimestamp = weatherPoints.ToDictionary(point => point.Timestamp);
        var timestamps = sensorByTimestamp.Keys
            .Union(weatherByTimestamp.Keys)
            .OrderBy(timestamp => timestamp);

        return timestamps
            .Select(timestamp =>
            {
                sensorByTimestamp.TryGetValue(timestamp, out var sensorPoint);
                weatherByTimestamp.TryGetValue(timestamp, out var weatherPoint);

                return new CorrelationTableRowViewModel(
                    timestamp,
                    sensorPoint?.Value,
                    sensorPoint?.IsInterpolated ?? false,
                    weatherPoint?.Value,
                    weatherPoint?.IsInterpolated ?? false);
            })
            .ToList();
    }

    private static IQueryable<TReading> ApplyDateFilter<TReading>(
        IQueryable<TReading> query,
        DateOnly? from,
        DateOnly? to)
        where TReading : class
    {
        var fromUtc = from.HasValue
            ? new DateTimeOffset(from.Value.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
            : (DateTimeOffset?)null;

        var toExclusiveUtc = to.HasValue
            ? new DateTimeOffset(to.Value.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc))
            : (DateTimeOffset?)null;

        if (typeof(TReading) == typeof(SensorReading))
        {
            var sensorQuery = (IQueryable<SensorReading>)query;

            if (fromUtc.HasValue)
            {
                sensorQuery = sensorQuery.Where(row => row.Timestamp >= fromUtc.Value);
            }

            if (toExclusiveUtc.HasValue)
            {
                sensorQuery = sensorQuery.Where(row => row.Timestamp < toExclusiveUtc.Value);
            }

            return (IQueryable<TReading>)sensorQuery;
        }

        var weatherQuery = (IQueryable<WeatherReading>)query;

        if (fromUtc.HasValue)
        {
            weatherQuery = weatherQuery.Where(row => row.Timestamp >= fromUtc.Value);
        }

        if (toExclusiveUtc.HasValue)
        {
            weatherQuery = weatherQuery.Where(row => row.Timestamp < toExclusiveUtc.Value);
        }

        return (IQueryable<TReading>)weatherQuery;
    }
}