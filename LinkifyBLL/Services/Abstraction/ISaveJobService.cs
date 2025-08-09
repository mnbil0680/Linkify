using LinkifyDAL.Entities;

namespace LinkifyBLL.Services.Abstraction
{
    public interface ISaveJobService
    {
        Task<SaveJob> SaveJobAsync(int jobId, string userId);
        Task UnsaveJobAsync(int savedJobId);
        Task DeleteSavedJobAsync(int savedJobId);
        Task RestoreSavedJobAsync(int savedJobId);
        Task<SaveJob?> GetUserSavedJobAsync(int jobId, string userId);
        Task<IEnumerable<SaveJob>> GetUserSavedJobsAsync(string userId, bool includeArchived = false);
        Task<IEnumerable<SaveJob>> GetUserArchivedSavesAsync(string userId);
        Task<bool> IsJobSavedByUserAsync(int jobId, string userId);
        Task<int> GetJobSaveCountAsync(int jobId);
    }
}
