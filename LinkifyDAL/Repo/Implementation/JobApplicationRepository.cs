using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;
namespace LinkifyDAL.Repo.Implementation
{
    public class JobApplicationRepository : IJobApplicationRepository
    {
        private readonly LinkifyDbContext _context;

        public JobApplicationRepository(LinkifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<JobApplication> CreateApplicationAsync(JobApplication application)
        {
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }
        public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.JobApplications
                .Include(ja => ja.Job)
                .Include(ja => ja.Applicant)
                .FirstOrDefaultAsync(ja => ja.Id == applicationId);
        }
        public async Task<IEnumerable<JobApplication>> GetAllApplicationsAsync(bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Include(ja => ja.Job)
                .Include(ja => ja.Applicant)
                .AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<JobApplication> UpdateApplicationAsync(JobApplication application)
        {
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync();
            return application;
        }
        public async Task UpdateApplicationStatusAsync(int applicationId, ApplicationStatus newStatus)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                application.UpdateStatus(newStatus);
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateCoverLetterAsync(int applicationId, string newCoverLetter)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                application.UpdateCoverLetter(newCoverLetter);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteApplicationAsync(int applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                application.Delete();
                await _context.SaveChangesAsync();
            }
        }
        public async Task RestoreApplicationAsync(int applicationId)
        {
            var application = await GetApplicationByIdAsync(applicationId);
            if (application != null)
            {
                application.Restore();
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsForJobAsync(int jobId, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.JobId == jobId)
                .Include(ja => ja.Applicant)
                .AsQueryable();

            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsByUserAsync(string userId, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.ApplicantId == userId)
                .Include(ja => ja.Job)
                .AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<JobApplication>> GetApplicationsByStatusAsync(ApplicationStatus status, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.Status == status)
                .Include(ja => ja.Job)
                .Include(ja => ja.Applicant)
                .AsQueryable();
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }
            return await query.ToListAsync();
        }
        public async Task<bool> HasUserAppliedForJobAsync(string userId, int jobId)
        {
            return await _context.JobApplications
                .AnyAsync(ja => ja.ApplicantId == userId &&
                               ja.JobId == jobId &&
                               !ja.IsDeleted.Value);
        }
        public async Task<bool> ApplicationExistsAsync(int applicationId)
        {
            return await _context.JobApplications
                .AnyAsync(ja => ja.Id == applicationId);
        }
        public async Task<int> GetApplicationCountForJobAsync(int jobId, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.JobId == jobId);
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }
            return await query.CountAsync();
        }
        public async Task<int> GetApplicationCountByUserAsync(string userId, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.ApplicantId == userId);
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }
            return await query.CountAsync();
        }
        public async Task<int> GetApplicationCountByStatusAsync(ApplicationStatus status, bool includeDeleted = false)
        {
            var query = _context.JobApplications
                .Where(ja => ja.Status == status);
            if (!includeDeleted)
            {
                query = query.Where(ja => !ja.IsDeleted.Value);
            }
            return await query.CountAsync();
        }
    }
}
