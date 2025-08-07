using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication> CreateApplicationAsync(JobApplication application);
        Task<JobApplication?> GetApplicationByIdAsync(int applicationId);
        Task<IEnumerable<JobApplication>> GetAllApplicationsAsync(bool includeDeleted = false);
        Task<JobApplication> UpdateApplicationAsync(JobApplication application);
        Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus);
        Task UpdateCoverLetterAsync(int applicationId, string newCoverLetter);
        Task DeleteApplicationAsync(int applicationId);
        Task RestoreApplicationAsync(int applicationId);
        Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobId, bool includeDeleted = false);
        Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId, bool includeDeleted = false);
        Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status, bool includeDeleted = false);
        Task<bool> HasUserAppliedForJobAsync(string userId, int jobId);
        Task<bool> ApplicationExistsAsync(int applicationId);
        Task<int> GetApplicationCountForJobAsync(int jobId, bool includeDeleted = false);
        Task<int> GetApplicationCountByUserAsync(string userId, bool includeDeleted = false);
        Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status, bool includeDeleted = false);
    }
}
