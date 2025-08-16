using LinkifyDAL.DataBase;
using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using LinkifyDAL.Repo.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyDAL.Repo.Implementation
{
    public class UserSkillsRepository : IUserSkillsRepository
    {
        readonly LinkifyDbContext _db;
        public UserSkillsRepository(LinkifyDbContext db)
        {
            _db = db;
        }
        public async Task AddSkillAsync(UserSkills skill)
        {
            await _db.UserSkills.AddAsync(skill);
            await _db.SaveChangesAsync();
        }

        public async Task<UserSkills?> GetSkillByIdAsync(int skillId)
        {
            return await _db.UserSkills.FindAsync(skillId);
        }
        public async Task<UserSkills?> GetSkillByUserIdAsync(string userId)
        {
            return await _db.UserSkills
                .FirstOrDefaultAsync(s => s.userId == userId);
        }
        public async Task<IEnumerable<UserSkills>> GetAllSkillsByUserIdAsync(string userId)
        {
            return await _db.UserSkills
                .Where(s => s.userId == userId)
                .ToListAsync();
        }
        public async Task<UserSkills?> GetSkillByUserIdAndNameAsync(string userId, string skillName)
        {
            return await _db.UserSkills
                .FirstOrDefaultAsync(s => s.userId == userId && s.Name == skillName);
        }
        public async Task UpdateSkillAsync(UserSkills skill)
        {
            _db.UserSkills.Update(skill);
            await _db.SaveChangesAsync();
        }

        public async Task RemoveSkillAsync(int skillId)
        {
            var skill = await GetSkillByIdAsync(skillId);
            if (skill != null)
            {
                skill.Delete();
                _db.UserSkills.Update(skill);
                await _db.SaveChangesAsync();
            }
        }
        public async Task <IEnumerable<UserSkills>> GetSkillsByLevelAsync(string userId, SkillLevel level)
        {
            return await _db.UserSkills
                .Where(s => s.userId == userId && s.Level == level && !s.IsDeleted)
                .ToListAsync();
        }

        
           
    }
}
