using LinkifyDAL.Entities;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface ISaveJobRepository
    {
        Task<SaveJob> SaveJobAsync(int jobId, string userId);
        Task UnsaveJobAsync(int savedJobId);
        Task DeleteSavedJobAsync(int savedJobId);
        Task RestoreSavedJobAsync(int savedJobId);
        Task<SaveJob?> GetUserSavedJobAsync(int jobId, string userId);
        Task<IEnumerable<SaveJob>> GetSavedJobsByUserAsync(string userId, bool includeArchived = false);
        Task<IEnumerable<SaveJob>> GetDeletedSavedJobsByUserAsync(string userId);
        Task<bool> IsJobSavedByUserAsync(int jobId, string userId);
        Task<int> GetJobSaveCountAsync(int jobId);
    }
}
