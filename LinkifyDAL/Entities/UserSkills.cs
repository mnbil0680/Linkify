using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyDAL.Entities
{
    public class UserSkills
    {
        public int Id { get; private set; }
        // Foreign key to User
        public string userId { get; private set; }
        public virtual User user { get; private set; }

        // Skill info
        public string Name { get; private set; }
        public CategorySkill Category { get; private set; } = CategorySkill.Other;
        public SkillLevel Level { get; private set; } = SkillLevel.Beginner;
        public bool IsDeleted { get; private set; } = false;
        public DateTime? DeletedOn { get; private set; }
        public DateTime? CreatedOn { get; private set; } = DateTime.Now;
        public DateTime? UpdatedOn { get; private set; }


        public void update(string name, CategorySkill category, SkillLevel level)
        {
            if (!string.IsNullOrEmpty(name))
                Name = name;
            Category = category;
            Level = level;
            UpdatedOn = DateTime.Now;
        }
        public UserSkills(string userId, string name, CategorySkill category, SkillLevel level)
        {
            this.userId = userId;
            Name = name;
            Category = category;
            Level = level;

            
        }
        public void Delete()
        {
            IsDeleted = true;
            DeletedOn = DateTime.Now;
        }
        
    }
}
