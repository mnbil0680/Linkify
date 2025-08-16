using LinkifyDAL.Entities;
using LinkifyDAL.Enums;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface ICertificateRepository
    {
        Task<Certificate> GetByIdAsync(int id);
        Task<IEnumerable<Certificate>> GetAllAsync();
        Task AddAsync(Certificate certificate);
        Task UpdateAsync(int certificateId, Certificate certificate);
        Task DeleteAsync(int id);
        Task<IEnumerable<Certificate>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Certificate>> GetActiveByUserIdAsync(string userId);
        Task<IEnumerable<Certificate>> GetExpiredByUserIdAsync(string userId);
        Task<IEnumerable<Certificate>> GetByOrganizationAsync(string organization);
        Task<IEnumerable<Certificate>> GetExpiringSoonAsync(DateTime cutoffDate);
        Task UpdateStatusAsync(int certificateId, CertificateStatus status);
        Task SoftDeleteAsync(int id);
        Task RestoreAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
