using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Abstraction
{
    public interface IJobApplicationService
    {
        Task<JobApplication> CreateApplicationAsync(int jobId, string applicantId, string? coverLetter = null);
        Task<JobApplication?> GetApplicationByIdAsync(int applicationId);
        Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobId, bool includeDeleted = false);
        Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId, bool includeDeleted = false);
        Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status, bool includeDeleted = false);
        Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus);
        Task UpdateCoverLetterAsync(int applicationId, string newCoverLetter);
        Task DeleteApplicationAsync(int applicationId);
        Task RestoreApplicationAsync(int applicationId);
        Task<bool> HasUserAppliedForJobAsync(string userId, int jobId);
        Task<int> GetApplicationCountForJobAsync(int jobId, bool includeDeleted = false);
        Task<int> GetApplicationCountByUserAsync(string userId, bool includeDeleted = false);
        Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status, bool includeDeleted = false);
    }
}
