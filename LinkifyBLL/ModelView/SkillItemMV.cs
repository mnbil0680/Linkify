
using LinkifyDAL.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkifyBLL.ModelView
{
    public class SkillItemMV
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public CategorySkill  Category { get; set; } 
        [Required]
        public SkillLevel Level { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
      
    }
}
