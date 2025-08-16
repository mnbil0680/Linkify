using LinkifyBLL.ModelView;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkifyPLL.Controllers
{
    public class ProfileController : Controller
    {
        public readonly IUserService IUS;
        public readonly IFriendsService IFS;
        public readonly IPostService IPS;

        public ProfileController(IUserService iUS, IFriendsService iFS, IPostService iPS)
        {
            IUS = iUS;
            IFS = iFS;
            IPS = iPS;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Edit()
        {
            var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // May be 404 Page
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get user from service
            var user = await IUS.GetUserByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Map User entity to ProfileMV
            var profileMV = await MapUserToProfileMVAsync(user);

            return View(profileMV);
        }

        // Manual Mapping
        private async Task<ProfileMV> MapUserToProfileMVAsync(User user)
        {
            return new ProfileMV
            {
                // Basic Information (from User entity)
                Id = user.Id,
                Name = user.UserName ?? "Unknown User",
                Email = user.Email,
                PhoneNumber = user.PhoneNumber, // from IdentityUser
                ImgPath = user.ImgPath,
                Location = "Unknown Country - Unknown City",

                // Professional Information (from User entity)
                Title = user.Title,
                Bio = user.Bio,
                Status = user.Status.ToString(),

                // Properties not in User entity - set to null/defaults
                Company = "UnKnow Company",
                Industry = null,

                // Social Links - all null (not in User entity)
                LinkedInUrl = null,
                GitHubUrl = null,
                PortfolioUrl = null,

                // Professional Details - defaults
                YearsOfExperience = null,

                // Account & Activity Information
                RegistrationDate = user.RegistrationDate,
                IsPremiumMember = false, // default

                // Stats - set to 0 (you can calculate these later)
                ConnectionsCount = await IFS.GetFriendCountAsync(user.Id),
                PostsCount = await IPS.GetUserPostCountAsync(user.Id),
                ProfileViews = 0,
                LikesCount = 0,
                CommentsCount = 0,
                SharesCount = 0,

                // Collections - initialize as empty (populated later from other services)
                Skills = new List<SkillMV>(),
                Languages = new List<LanguageMV>(),
                Certifications = new List<CertificationMV>(),
                WorkExperience = new List<ExperienceItemMV>(),
                VolunteerExperience = new List<VolunteerExperienceMV>(),
                Education = new List<EducationItemMV>(),
                Courses = new List<CourseMV>(),
                Projects = new List<ProjectItemMV>(),
                Posts = new List<PostMV>(),


                Connections = (await IFS.GetFriendsAsync(user.Id)).ToList()
            };
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileMV model, IFormFile ProfileImage)
        {

            if (ProfileImage == null && string.IsNullOrEmpty(model.ImgPath))
            {
                ModelState.AddModelError("ProfileImage", "Profile image is required.");
            }


            try
            {
                var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId) || userId != model.Id)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Handle profile image upload
                string imageFileName = model.ImgPath; // Keep existing image path by default

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    // Validate file size (5MB max)
                    if (ProfileImage.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("ProfileImage", "File size must be less than 5MB");
                        return View("Edit", model);
                    }

                    // Validate file type
                    var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                    if (!allowedTypes.Contains(ProfileImage.ContentType.ToLower()))
                    {
                        ModelState.AddModelError("ProfileImage", "Please select a valid image file (JPG, PNG, GIF)");
                        return View("Edit", model);
                    }

                    // Upload the new image using your FileManager/Upload helper
                    imageFileName = LinkifyBLL.Helper.FileManager.UploadFile("Files", ProfileImage);

                    // Check if upload was successful
                    if (string.IsNullOrEmpty(imageFileName) || imageFileName.Contains("Error") || imageFileName.Contains("Exception"))
                    {
                        ModelState.AddModelError("ProfileImage", "Failed to upload image. Please try again.");
                        return View("Edit", model);
                    }

                    // Update model with new image path for database storage
                    imageFileName = $"{imageFileName}";
                }

                // Update user profile using the service
                var updateResult = await IUS.UpdateProfileAsync(
                    userId: userId,
                    userName: model.Name,
                    imgPath: imageFileName,
                    cvPath: null, // You can add CV upload functionality later
                    title: model.Title,
                    bio: model.Bio
                );

                if (updateResult)
                {
                    // Set success message
                    TempData["SuccessMessage"] = "Profile updated successfully!";

                    // Redirect to prevent re-submission on refresh
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to update profile. Please try again.");
                    return View("Edit", model);
                }
            }
            catch (Exception ex)
            {
                // Log the exception (you can inject ILogger if needed)
                ModelState.AddModelError("", "An error occurred while updating your profile. Please try again.");
                return View("Edit", model);
            }
        }





    }
}
