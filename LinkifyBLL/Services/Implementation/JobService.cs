using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class JobService : IJobService
    {
        private readonly IJobRepository _jobRepository;

        public JobService(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
        }

        public async Task<Job> CreateJobAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return await _jobRepository.CreateJobAsync(job);
        }

        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _jobRepository.GetJobByIdAsync(jobId);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync(bool includeInactive = false)
        {
            return await _jobRepository.GetAllJobsAsync(includeInactive);
        }

        public async Task<Job> UpdateJobAsync(int jobId, string? title = null, string? description = null, string? company = null, string? location = null, string? salaryRange = null, JobTypes? type = null, JobPresence? presence = null, DateTime? expiresOn = null)
        {
            if (await _jobRepository.GetJobByIdAsync(jobId) is not Job existingJob)
            {
                throw new KeyNotFoundException($"Job with ID {jobId} not found");
            }
            if (existingJob.ExpiresOn < DateTime.Now)
            {
                throw new InvalidOperationException("Cannot update expired jobs");
            }
            return await _jobRepository.UpdateJobAsync(
                jobId,
                title,
                description,
                company,
                location,
                salaryRange,
                type,
                presence,
                expiresOn);
        }

        public async Task DeleteJobAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            await _jobRepository.DeleteJobAsync(job.Id);
        }

        public async Task ToggleJobActivationAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            await _jobRepository.ToggleJobActivationAsync(job.Id);
        }

        public async Task RestoreJobAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            await _jobRepository.RestoreJobAsync(job.Id);
        }

        public async Task<IEnumerable<Job>> GetJobsByUserAsync(string userId, bool includeInactive = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await _jobRepository.GetJobsByUserAsync(userId, includeInactive);
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _jobRepository.GetActiveJobsAsync();
        }

        public async Task<IEnumerable<Job>> GetExpiredJobsAsync()
        {
            return await _jobRepository.GetExpiredJobsAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByTypeAsync(JobTypes jobType, bool includeInactive = false)
        {
            return await _jobRepository.GetJobsByTypeAsync(jobType, includeInactive);
        }

        public async Task<IEnumerable<Job>> GetJobsByPresenceAsync(JobPresence presence, bool includeInactive = false)
        {
            return await _jobRepository.GetJobsByPresenceAsync(presence, includeInactive);
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(
            string? searchTerm = null,
            string? location = null,
            JobTypes? jobType = null,
            JobPresence? presence = null,
            bool includeInactive = false)
        {
            return await _jobRepository.SearchJobsAsync(
                searchTerm,
                location,
                jobType,
                presence,
                includeInactive);
        }
        public async Task<int> GetJobCountAsync(bool includeInactive = false)
        {
            return await _jobRepository.GetJobCountAsync(includeInactive);
        }
        public async Task<int> GetUserJobCountAsync(string userId, bool includeInactive = false)
        {
            return await _jobRepository.GetUserJobCountAsync(userId, includeInactive);
        }
        public async Task<bool> JobExistsAsync(int jobId)
        {
            return await _jobRepository.JobExistsAsync(jobId);
        }
        public async Task<bool> IsJobActiveAsync(Job job)
        {
            if (job == null) throw new ArgumentNullException(nameof(job));
            return await _jobRepository.IsJobActiveAsync(job.Id);
        }
    }
}
