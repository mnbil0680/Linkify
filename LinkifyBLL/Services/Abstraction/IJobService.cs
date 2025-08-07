using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IJobService
    {
        Task<Job> CreateJobAsync(Job job);
        Task<Job?> GetJobByIdAsync(int jobId);
        Task<IEnumerable<Job>> GetAllJobsAsync(bool includeInactive = false);
        Task<Job> UpdateJobAsync(int jobId, string? title = null, string? description = null, string? company = null, string? location = null, string? salaryRange = null, JobTypes? type = null, JobPresence? presence = null, DateTime? expiresOn = null);
        Task DeleteJobAsync(Job job);
        Task ToggleJobActivationAsync(Job job);
        Task RestoreJobAsync(Job job);
        Task<IEnumerable<Job>> GetJobsByUserAsync(string userId, bool includeInactive = false);
        Task<IEnumerable<Job>> GetActiveJobsAsync();
        Task<IEnumerable<Job>> GetExpiredJobsAsync();
        Task<IEnumerable<Job>> GetJobsByTypeAsync(JobTypes jobType, bool includeInactive = false);
        Task<IEnumerable<Job>> GetJobsByPresenceAsync(JobPresence presence, bool includeInactive = false);
        Task<IEnumerable<Job>> SearchJobsAsync(
            string? searchTerm = null,
            string? location = null,
            JobTypes? jobType = null,
            JobPresence? presence = null,
            bool includeInactive = false);
        Task<int> GetJobCountAsync(bool includeInactive = false);
        Task<int> GetUserJobCountAsync(string userId, bool includeInactive = false);
        Task<bool> JobExistsAsync(int jobId);
        Task<bool> IsJobActiveAsync(Job job);
    }
}
