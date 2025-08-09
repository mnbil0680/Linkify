using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class SaveJobRepository : ISaveJobRepository
    {
        private readonly LinkifyDbContext _context;

        public SaveJobRepository(LinkifyDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SaveJob> SaveJobAsync(int jobId, string userId)
        {
            var existing = await GetUserSavedJobAsync(jobId, userId);
            if (existing != null)
            {
                if (existing.IsArchived)
                {
                    existing.Restore();
                    await _context.SaveChangesAsync();
                }
                return existing;
            }

            var saveJob = new SaveJob(jobId, userId);
            _context.SaveJobs.Add(saveJob);
            await _context.SaveChangesAsync();
            return saveJob;
        }

        public async Task UnsaveJobAsync(int savedJobId)
        {
            var savedJob = await _context.SaveJobs.FindAsync(savedJobId);
            if (savedJob != null)
            {
                _context.SaveJobs.Remove(savedJob);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteSavedJobAsync(int savedJobId)
        {
            var savedJob = await _context.SaveJobs.FindAsync(savedJobId);
            if (savedJob != null)
            {
                savedJob.Delete();
                await _context.SaveChangesAsync();
            }
        }

        public async Task RestoreSavedJobAsync(int savedJobId)
        {
            var savedJob = await _context.SaveJobs.FindAsync(savedJobId);
            if (savedJob != null)
            {
                savedJob.Restore();
                await _context.SaveChangesAsync();
            }
        }

        public async Task<SaveJob?> GetUserSavedJobAsync(int jobId, string userId)
        {
            return await _context.SaveJobs
                .FirstOrDefaultAsync(sj => sj.JobId == jobId && sj.UserId == userId);
        }

        public async Task<IEnumerable<SaveJob>> GetSavedJobsByUserAsync(string userId, bool includeArchived = false)
        {
            var query = _context.SaveJobs
                .Where(sj => sj.UserId == userId)
                .Include(sj => sj.Job)
                .AsQueryable();

            if (!includeArchived)
            {
                query = query.Where(sj => !sj.IsArchived);
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<SaveJob>> GetDeletedSavedJobsByUserAsync(string userId)
        {
            return await _context.SaveJobs
                .Where(sj => sj.UserId == userId && sj.IsArchived)
                .Include(sj => sj.Job)
                .ToListAsync();
        }

        public async Task<bool> IsJobSavedByUserAsync(int jobId, string userId)
        {
            return await _context.SaveJobs
                .AnyAsync(sj => sj.JobId == jobId && sj.UserId == userId && !sj.IsArchived);
        }

        public async Task<int> GetJobSaveCountAsync(int jobId)
        {
            return await _context.SaveJobs
                .CountAsync(sj => sj.JobId == jobId && !sj.IsArchived);
        }
    }
}
