using LinkifyDAL.Entities;
using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class SkillListMV
    {
        public List<SkillItemMV> skills { get; set; }
        public int totalSkillsCount { get; set; }
        public int totalBeginnerCount { get; set; }
        public int totalIntermediateCount { get; set; }
        public int totalExpertCount { get; set; }
        public string SearchTerm { get; set; }
        public CategorySkill? FilterCategory { get; set; } 
        public SkillLevel? FilterLevel { get; set; }

    }
}
