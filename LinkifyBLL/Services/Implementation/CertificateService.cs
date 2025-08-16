using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;

namespace LinkifyBLL.Services.Implementation
{
    public class CertificateService : ICertificateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IUserRepository _userRepository;

        public CertificateService(
            ICertificateRepository certificateRepository,
            IUserRepository userRepository)
        {
            _certificateRepository = certificateRepository;
            _userRepository = userRepository;
        }

        public async Task<Certificate> GetByIdAsync(int id)
        {
            return await _certificateRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetAllAsync()
        {
            return await _certificateRepository.GetAllAsync();
        }

        public async Task CreateAsync(Certificate certificate)
        {
            var user = await _userRepository.GetUserByIdAsync(certificate.UserId);
            if (user == null || user.IsDeleted)
            {
                throw new ArgumentException("User not found or is deleted");
            }

            await _certificateRepository.AddAsync(certificate);
        }

        public async Task UpdateAsync(int certificateId, Certificate certificate)
        {
            var existingCertificate = await _certificateRepository.GetByIdAsync(certificateId);
            if (existingCertificate == null)
            {
                throw new KeyNotFoundException("Certificate not found");
            }
            await _certificateRepository.UpdateAsync(certificateId, certificate);
        }

        public async Task DeleteAsync(int id)
        {
            await _certificateRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Certificate>> GetByUserIdAsync(string userId)
        {
            return await _certificateRepository.GetByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Certificate>> GetActiveByUserIdAsync(string userId)
        {
            return await _certificateRepository.GetActiveByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Certificate>> GetExpiredByUserIdAsync(string userId)
        {
            return await _certificateRepository.GetExpiredByUserIdAsync(userId);
        }

        public async Task UpdateStatusAsync(int certificateId, CertificateStatus status)
        {
            await _certificateRepository.UpdateStatusAsync(certificateId, status);
        }

        public async Task SoftDeleteAsync(int id)
        {
            await _certificateRepository.SoftDeleteAsync(id);
        }

        public async Task RestoreAsync(int id)
        {
            await _certificateRepository.RestoreAsync(id);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _certificateRepository.ExistsAsync(id);
        }

        public async Task<bool> ValidateOwnershipAsync(int certificateId, string userId)
        {
            var certificate = await _certificateRepository.GetByIdAsync(certificateId);
            return certificate != null && certificate.UserId == userId;
        }
    }
}
