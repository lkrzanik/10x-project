using _10xPV.Data;
using _10xPV.Models;
using _10xPV.Models.ClimateImport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace _10xPV.Controllers;

public class ClimateDataController : Controller
{
    private const int DefaultPageSize = 50;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<ClimateDataController> _logger;

    public ClimateDataController(AppDbContext dbContext, ILogger<ClimateDataController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        DataSource dataSource = DataSource.Sensor,
        DateOnly? from = null,
        DateOnly? to = null,
        int page = 1,
        int pageSize = DefaultPageSize,
        CancellationToken cancellationToken = default)
    {
        var normalizedPageSize = NormalizePageSize(pageSize);

        if (from.HasValue && to.HasValue && from > to)
        {
            ModelState.AddModelError(string.Empty, "Data początkowa nie może być późniejsza niż data końcowa.");

            return View(BuildEmptyPage(dataSource, from, to, page, normalizedPageSize));
        }

        try
        {
            return dataSource switch
            {
                DataSource.Sensor => View(await BuildSensorPageAsync(from, to, page, normalizedPageSize, cancellationToken)),
                DataSource.Weather => View(await BuildWeatherPageAsync(from, to, page, normalizedPageSize, cancellationToken)),
                _ => View(await BuildSensorPageAsync(from, to, page, normalizedPageSize, cancellationToken))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas odczytu danych klimatycznych.");
            ModelState.AddModelError(string.Empty, "Wystąpił błąd podczas odczytu danych. Spróbuj ponownie.");
            return View(BuildEmptyPage(dataSource, from, to, page, normalizedPageSize));
        }
    }

    private async Task<ClimateDataPageViewModel> BuildSensorPageAsync(
        DateOnly? from,
        DateOnly? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.SensorReadings.AsNoTracking();
        query = ApplyDateFilter(query, from, to);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = ComputeTotalPages(totalCount, pageSize);
        var normalizedPage = NormalizePage(page, totalPages);

        var rows = totalCount == 0
            ? []
            : await query
                .OrderByDescending(row => row.Timestamp)
                .Skip((normalizedPage - 1) * pageSize)
                .Take(pageSize)
                .Select(row => new ClimateDataRowViewModel
                {
                    Timestamp = row.Timestamp,
                    Temperature = row.Temperature,
                    Humidity = row.Humidity,
                    CloudCover = null
                })
                .ToListAsync(cancellationToken);

        return BuildPage(DataSource.Sensor, from, to, normalizedPage, pageSize, totalCount, totalPages, rows);
    }

    private async Task<ClimateDataPageViewModel> BuildWeatherPageAsync(
        DateOnly? from,
        DateOnly? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.WeatherReadings.AsNoTracking();
        query = ApplyDateFilter(query, from, to);

        var totalCount = await query.CountAsync(cancellationToken);
        var totalPages = ComputeTotalPages(totalCount, pageSize);
        var normalizedPage = NormalizePage(page, totalPages);

        var rows = totalCount == 0
            ? []
            : await query
                .OrderByDescending(row => row.Timestamp)
                .Skip((normalizedPage - 1) * pageSize)
                .Take(pageSize)
                .Select(row => new ClimateDataRowViewModel
                {
                    Timestamp = row.Timestamp,
                    Temperature = row.Temperature,
                    Humidity = row.Humidity,
                    CloudCover = row.CloudCover
                })
                .ToListAsync(cancellationToken);

        return BuildPage(DataSource.Weather, from, to, normalizedPage, pageSize, totalCount, totalPages, rows);
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

    private static ClimateDataPageViewModel BuildEmptyPage(
        DataSource dataSource,
        DateOnly? from,
        DateOnly? to,
        int page,
        int pageSize)
    {
        var normalizedPage = NormalizePage(page, totalPages: 0);
        return BuildPage(dataSource, from, to, normalizedPage, pageSize, totalCount: 0, totalPages: 0, rows: []);
    }

    private static ClimateDataPageViewModel BuildPage(
        DataSource dataSource,
        DateOnly? from,
        DateOnly? to,
        int currentPage,
        int pageSize,
        int totalCount,
        int totalPages,
        IReadOnlyList<ClimateDataRowViewModel> rows)
    {
        return new ClimateDataPageViewModel
        {
            Filter = new ClimateDataFilterViewModel
            {
                DataSource = dataSource,
                From = from,
                To = to,
                Page = currentPage,
                PageSize = pageSize
            },
            Rows = rows,
            TotalCount = totalCount,
            CurrentPage = currentPage,
            TotalPages = totalPages,
            PageSize = pageSize,
            HasPreviousPage = totalPages > 0 && currentPage > 1,
            HasNextPage = totalPages > 0 && currentPage < totalPages
        };
    }

    private static int ComputeTotalPages(int totalCount, int pageSize)
    {
        if (totalCount == 0)
        {
            return 0;
        }

        return (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    private static int NormalizePage(int page, int totalPages)
    {
        if (page < 1)
        {
            return 1;
        }

        if (totalPages == 0)
        {
            return 1;
        }

        return page > totalPages ? totalPages : page;
    }

    private static int NormalizePageSize(int pageSize)
    {
        return pageSize == DefaultPageSize ? pageSize : DefaultPageSize;
    }
}