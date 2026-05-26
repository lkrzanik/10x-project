using _10xPV.Models;

namespace _10xPV.Services;

public interface IDeduplicationService
{
    Task<bool> ExistsAsync(DateTimeOffset timestamp, DataSource source);
}