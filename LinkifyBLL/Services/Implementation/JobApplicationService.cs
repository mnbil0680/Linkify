using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
namespace LinkifyBLL.Services.Implementation
{
    public class JobApplicationService : IJobApplicationService
    {
        private readonly IJobApplicationRepository _applicationRepository;

        public JobApplicationService(IJobApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        }

        public async Task<JobApplication> CreateApplicationAsync(int jobId, string applicantId, string? coverLetter = null)
        {
            if (jobId <= 0) throw new ArgumentException("Invalid job ID");
            if (string.IsNullOrWhiteSpace(applicantId)) throw new ArgumentException("Applicant ID is required");
            if (await _applicationRepository.HasUserAppliedForJobAsync(applicantId, jobId))
                throw new InvalidOperationException("User has already applied for this job");

            var application = new JobApplication(jobId, applicantId, coverLetter);
            return await _applicationRepository.CreateApplicationAsync(application);
        }
        public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
        {
            return await _applicationRepository.GetApplicationByIdAsync(applicationId);
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobId, bool includeDeleted = false)
        {
            return await _applicationRepository.GetApplicationsForJobAsync(jobId, includeDeleted);
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId, bool includeDeleted = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required");
            return await _applicationRepository.GetApplicationsByUserAsync(userId, includeDeleted);
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status, bool includeDeleted = false)
        {
            return await _applicationRepository.GetApplicationsByStatusAsync(status, includeDeleted);
        }
        public async Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            await _applicationRepository.UpdateApplicationStatusAsync(applicationId, newStatus);
        }
        public async Task UpdateCoverLetterAsync(int applicationId, string newCoverLetter)
        {
            if (string.IsNullOrWhiteSpace(newCoverLetter))
                throw new ArgumentException("Cover letter cannot be empty");

            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            await _applicationRepository.UpdateCoverLetterAsync(applicationId, newCoverLetter);
        }
        public async Task DeleteApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            await _applicationRepository.DeleteApplicationAsync(applicationId);
        }
        public async Task RestoreApplicationAsync(int applicationId)
        {
            var application = await _applicationRepository.GetApplicationByIdAsync(applicationId);
            if (application == null) throw new KeyNotFoundException("Application not found");

            await _applicationRepository.RestoreApplicationAsync(applicationId);
        }
        public async Task<bool> HasUserAppliedForJobAsync(string userId, int jobId)
        {
            return await _applicationRepository.HasUserAppliedForJobAsync(userId, jobId);
        }
        public async Task<int> GetApplicationCountForJobAsync(int jobId, bool includeDeleted = false)
        {
            return await _applicationRepository.GetApplicationCountForJobAsync(jobId, includeDeleted);
        }
        public async Task<int> GetApplicationCountByUserAsync(string userId, bool includeDeleted = false)
        {
            return await _applicationRepository.GetApplicationCountByUserAsync(userId, includeDeleted);
        }
        public async Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status, bool includeDeleted = false)
        {
            return await _applicationRepository.GetApplicationCountByStatusAsync(status, includeDeleted);
        }
    }
}
