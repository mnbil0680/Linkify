using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace LinkifyDAL.Repo.Implementation
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly LinkifyDbContext _context;
        public CertificateRepository(LinkifyDbContext context)
        {
            _context = context;
        }
        public async Task<Certificate> GetByIdAsync(int id)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        }
        public async Task<IEnumerable<Certificate>> GetAllAsync()
        {
            return await _context.Certificates
                .Where(c => !c.IsDeleted)
                .ToListAsync();
        }
        public async Task AddAsync(Certificate certificate)
        {
            await _context.Certificates.AddAsync(certificate);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(int id, Certificate updatedCertificate)
        {
            var existingCert = await _context.Certificates.FindAsync(id);
            if (existingCert == null)
            {
                throw new KeyNotFoundException($"Certificate with ID {id} not found");
            }
            if (existingCert.UserId != updatedCertificate.UserId)
            {
                throw new InvalidOperationException("Cannot change certificate ownership");
            }
            existingCert.Edit(
                name: updatedCertificate.Name,
                certificatePath: updatedCertificate.CertificatePath,
                issuingOrganization: updatedCertificate.IssuingOrganization,
                issueDate: updatedCertificate.IssueDate,
                expirationDate: updatedCertificate.ExpirationDate,
                credentialId: updatedCertificate.CredentialId,
                credentialUrl: updatedCertificate.CredentialUrl,
                status: updatedCertificate.Status
            );
            _context.Entry(existingCert).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var certificate = await GetByIdAsync(id);
            if (certificate != null)
            {
                _context.Certificates.Remove(certificate);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Certificate>> GetByUserIdAsync(string userId)
        {
            return await _context.Certificates
                .Where(c => c.UserId == userId && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetActiveByUserIdAsync(string userId)
        {
            return await _context.Certificates
                .Where(c => c.UserId == userId &&
                            !c.IsDeleted &&
                            c.Status == CertificateStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetExpiredByUserIdAsync(string userId)
        {
            return await _context.Certificates
                .Where(c => c.UserId == userId &&
                           !c.IsDeleted &&
                           c.Status == CertificateStatus.Expired)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetByOrganizationAsync(string organization)
        {
            return await _context.Certificates
                .Where(c => c.IssuingOrganization.Contains(organization) && !c.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Certificate>> GetExpiringSoonAsync(DateTime cutoffDate)
        {
            return await _context.Certificates
                .Where(c => !c.IsDeleted &&
                            c.ExpirationDate.HasValue &&
                            c.ExpirationDate <= cutoffDate &&
                            c.Status == CertificateStatus.Active)
                .ToListAsync();
        }

        public async Task UpdateStatusAsync(int certificateId, CertificateStatus status)
        {
            var certificate = await GetByIdAsync(certificateId);
            if (certificate != null)
            {
                certificate.UpdateStatus(status);
                await _context.SaveChangesAsync(); ;
            }
        }
        public async Task SoftDeleteAsync(int id)
        {
            var certificate = await GetByIdAsync(id);
            if (certificate != null)
            {
                certificate.Delete();
                await _context.SaveChangesAsync();
            }
        }
        public async Task RestoreAsync(int id)
        {
            var certificate = await _context.Certificates
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(c => c.Id == id && c.IsDeleted);

            if (certificate != null)
            {
                certificate.Restore();
                await _context.SaveChangesAsync();
            }
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Certificates
                .AnyAsync(c => c.Id == id && !c.IsDeleted);
        }
    }
}
