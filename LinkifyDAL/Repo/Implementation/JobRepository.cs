using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class JobRepository : IJobRepository
    {
        private readonly LinkifyDbContext _context;

        public JobRepository(LinkifyDbContext context)
        {
            _context = context;
        }

        private static bool IsActiveAndNotDeleted(Job j) => !(j.IsDeleted ?? true) && (j.IsActive ?? false);

        public async Task<Job> CreateJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        public async Task<Job?> GetJobByIdAsync(int jobId)
        {
            return await _context.Jobs
                .Include(j => j.User)
                .FirstOrDefaultAsync(j => j.Id == jobId);
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync(bool includeInactive = false)
        {
            var query = _context.Jobs
                .Include(j => j.User)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.ToListAsync();
        }

        public async Task<Job> UpdateJobAsync(int jobId,
            string? title = null,
            string? description = null,
            string? company = null,
            string? location = null,
            string? salaryRange = null,
            JobTypes? type = null,
            JobPresence? presence = null,
            DateTime? expiresOn = null)
        {
            var job = _context.Jobs.FirstOrDefault(j => j.Id == jobId);
            job.Edit(title, description, company, location, salaryRange, type, presence, expiresOn);
            await _context.SaveChangesAsync();
            return job;
        }
        public async Task DeleteJobAsync(int jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job != null)
            {
                job.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task ToggleJobActivationAsync(int jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job != null)
            {
                job.ToggleActivation();
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreJobAsync(int jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            if (job != null)
            {
                job.Restore();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Job>> GetJobsByUserAsync(string userId, bool includeInactive = false)
        {
            var query = _context.Jobs
                .Where(j => j.UserId == userId)
                .Include(j => j.User)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetActiveJobsAsync()
        {
            return await _context.Jobs
                .Where(j => IsActiveAndNotDeleted(j) && (j.ExpiresOn == null || j.ExpiresOn > DateTime.Now))
                .Include(j => j.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetExpiredJobsAsync()
        {
            return await _context.Jobs
                .Where(j => IsActiveAndNotDeleted(j) && j.ExpiresOn != null && j.ExpiresOn <= DateTime.Now)
                .Include(j => j.User)
                .ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByTypeAsync(JobTypes jobType, bool includeInactive = false)
        {
            var query = _context.Jobs
                .Where(j => j.Type == jobType)
                .Include(j => j.User)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Job>> GetJobsByPresenceAsync(JobPresence presence, bool includeInactive = false)
        {
            var query = _context.Jobs
                .Where(j => j.Presence == presence)
                .Include(j => j.User)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Job>> SearchJobsAsync(
            string? searchTerm = null,
            string? location = null,
            JobTypes? jobType = null,
            JobPresence? presence = null,
            bool includeInactive = false)
        {
            var query = _context.Jobs
                .Include(j => j.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(j =>
                    j.Title.Contains(searchTerm) ||
                    j.Description.Contains(searchTerm) ||
                    j.Company.Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(j => j.Location != null && j.Location.Contains(location));
            }

            if (jobType.HasValue)
            {
                query = query.Where(j => j.Type == jobType.Value);
            }

            if (presence.HasValue)
            {
                query = query.Where(j => j.Presence == presence.Value);
            }

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.ToListAsync();
        }

        public async Task<int> GetJobCountAsync(bool includeInactive = false)
        {
            var query = _context.Jobs.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.CountAsync();
        }

        public async Task<int> GetUserJobCountAsync(string userId, bool includeInactive = false)
        {
            var query = _context.Jobs
                .Where(j => j.UserId == userId);

            if (!includeInactive)
            {
                query = query.Where(j => IsActiveAndNotDeleted(j));
            }

            return await query.CountAsync();
        }

        public async Task<bool> JobExistsAsync(int jobId)
        {
            return await _context.Jobs.AnyAsync(j => j.Id == jobId);
        }

        public async Task<bool> IsJobActiveAsync(int jobId)
        {
            var job = await GetJobByIdAsync(jobId);
            return job != null && IsActiveAndNotDeleted(job);
        }
    }
}
