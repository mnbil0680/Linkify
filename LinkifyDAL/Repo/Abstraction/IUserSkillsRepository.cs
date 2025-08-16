using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyDAL.Repo.Abstraction
{
    public interface IUserSkillsRepository
    {
        Task AddSkillAsync(UserSkills skill);
        Task<UserSkills?> GetSkillByIdAsync(int skillId);
        Task<UserSkills?> GetSkillByUserIdAsync(string userId);
        Task<IEnumerable<UserSkills>> GetAllSkillsByUserIdAsync(string userId);
        Task<UserSkills?> GetSkillByUserIdAndNameAsync(string userId, string skillName);
        Task UpdateSkillAsync(UserSkills skill);
        Task RemoveSkillAsync(int skillId);
        Task<IEnumerable<UserSkills>> GetSkillsByLevelAsync(string userId, SkillLevel level);
    }
}
