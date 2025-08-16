
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.Services.Implementation
{
    public class UserSkillsService : IUserSkillsService
    {
        private readonly IUserSkillsRepository _userSkillsRepository;
        public UserSkillsService(IUserSkillsRepository userSkillsRepository)
        {
            _userSkillsRepository = userSkillsRepository;
        }
        public async Task AddSkillAsync(UserSkills skill)
        {
            await _userSkillsRepository.AddSkillAsync(skill);
        }
        public async Task<UserSkills?> GetSkillByIdAsync(int skillId)
        {
            return await _userSkillsRepository.GetSkillByIdAsync(skillId);
        }
        public async Task<UserSkills?> GetSkillByUserIdAsync(string userId)
        {
            return await _userSkillsRepository.GetSkillByUserIdAsync(userId);
        }
        public async Task<IEnumerable<UserSkills>> GetAllSkillsByUserIdAsync(string userId)
        {
            return await _userSkillsRepository.GetAllSkillsByUserIdAsync(userId);
        }
        public async Task<UserSkills?> GetSkillByUserIdAndNameAsync(string userId, string skillName)
        {
            return await _userSkillsRepository.GetSkillByUserIdAndNameAsync(userId, skillName);
        }
        public async Task UpdateSkillAsync(UserSkills skill)
        {
            await _userSkillsRepository.UpdateSkillAsync(skill);
        }
        public async Task RemoveSkillAsync(int skillId)
        {
            await _userSkillsRepository.RemoveSkillAsync(skillId);

        }
        public async Task<IEnumerable<UserSkills>> GetSkillsByLevelAsync(string userId, SkillLevel level)
        {
            return await _userSkillsRepository.GetSkillsByLevelAsync(userId, level);
        }
    }
}
