using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class SaveJobService : ISaveJobService
    {
        private readonly ISaveJobRepository _saveJobRepository;

        public SaveJobService(ISaveJobRepository saveJobRepository)
        {
            this._saveJobRepository = saveJobRepository;
        }

        public async Task<SaveJob> SaveJobAsync(int jobId, string userId)
        {
            if (jobId <= 0) throw new ArgumentException("Invalid job ID");
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required");

            return await _saveJobRepository.SaveJobAsync(jobId, userId);
        }

        public async Task UnsaveJobAsync(int savedJobId)
        {
            await _saveJobRepository.UnsaveJobAsync(savedJobId);
        }

        public async Task DeleteSavedJobAsync(int savedJobId)
        {
            await _saveJobRepository.DeleteSavedJobAsync(savedJobId);
        }

        public async Task RestoreSavedJobAsync(int savedJobId)
        {
            await _saveJobRepository.RestoreSavedJobAsync(savedJobId);
        }

        public async Task<SaveJob?> GetUserSavedJobAsync(int jobId, string userId)
        {
            return await _saveJobRepository.GetUserSavedJobAsync(jobId, userId);
        }

        public async Task<IEnumerable<SaveJob>> GetUserSavedJobsAsync(string userId, bool includeArchived = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required");
            return await _saveJobRepository.GetSavedJobsByUserAsync(userId, includeArchived);
        }

        public async Task<IEnumerable<SaveJob>> GetUserArchivedSavesAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required");
            return await _saveJobRepository.GetDeletedSavedJobsByUserAsync(userId);
        }

        public async Task<bool> IsJobSavedByUserAsync(int jobId, string userId)
        {
            return await _saveJobRepository.IsJobSavedByUserAsync(jobId, userId);
        }

        public async Task<int> GetJobSaveCountAsync(int jobId)
        {
            return await _saveJobRepository.GetJobSaveCountAsync(jobId);
        }
    }
}
