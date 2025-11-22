using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Repositories.Contracts
{
    public interface ILogRepository
    {
        Task AddLogAsync(Log log);
        Task<List<Log>> GetLogsAsync(string entityType, int entityId);
    }
}
