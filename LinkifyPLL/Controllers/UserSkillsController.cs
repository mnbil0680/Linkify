using Microsoft.AspNetCore.Mvc;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using LinkifyBLL.ModelView;
using System.Security.Claims;
using LinkifyDAL.Enums;

namespace LinkifyPLL.Controllers
{
    public class UserSkillsController : Controller
    {
        private readonly IUserSkillsService _userSkillsService;
        public UserSkillsController(IUserSkillsService userSkillsService)
        {
            _userSkillsService = userSkillsService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userSkills = await _userSkillsService.GetAllSkillsByUserIdAsync(userId);
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }
           
            if (userSkills == null || !userSkills.Any())
            {
                return View(new SkillListMV
                {
                    skills = new List<SkillItemMV>(),
                    totalSkillsCount = 0,
                    totalBeginnerCount = 0,
                    totalIntermediateCount = 0,
                    totalExpertCount = 0
                });
            }
            var skillsList = userSkills.Select(skill => new SkillItemMV
            {
                Id = skill.Id,
                Name = skill.Name,
                Category = skill.Category ,
                Level = skill.Level ,
                CreatedOn = skill.CreatedOn ?? DateTime.Now
            }).ToList();
            var skillsCount = skillsList.Count();
            var SkillsPerExpert = await _userSkillsService.GetSkillsByLevelAsync(userId, SkillLevel.Expert);
            var SkillsPerIntermediate = await _userSkillsService.GetSkillsByLevelAsync(userId, SkillLevel.Intermediate);
            var SkillsPerBeginner = await _userSkillsService.GetSkillsByLevelAsync(userId, SkillLevel.Beginner);
            
            var model = new SkillListMV
            {
                skills = skillsList,
                totalSkillsCount = skillsCount,
                totalBeginnerCount = SkillsPerBeginner.Count(),
                totalIntermediateCount = SkillsPerIntermediate.Count(),
                totalExpertCount = SkillsPerExpert.Count() 

            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AddSkill(SkillItemMV skillItem)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                var existingSkills = await _userSkillsService.GetAllSkillsByUserIdAsync(userId);
                if (existingSkills.Any(s => s.Name.Equals(skillItem.Name, StringComparison.OrdinalIgnoreCase)
                                          && s.Category == skillItem.Category))
                {
                    ModelState.AddModelError("", "You already added this skill.");
                    return View(skillItem);
                }
                var skill = new UserSkills(userId, skillItem.Name, skillItem.Category, skillItem.Level);
                await _userSkillsService.AddSkillAsync(skill);
                return RedirectToAction("Index");
            }
            return View(skillItem);
        }
    }
}
