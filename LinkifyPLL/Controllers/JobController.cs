using LinkifyBLL.ModelView;
using LinkifyDAL.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using LinkifyBLL.Services.Implementation;
using LinkifyBLL.Services.Abstraction;
using LinkifyDAL.Enums;
using SempaBLL.Helper;

namespace LinkifyPLL.Controllers
{
    public class JobController : Controller
    {
        private readonly IJobService _jobService;
        private readonly IJobApplicationService _jobApplicationService;
        private readonly ISaveJobService _saveJobService;
        public JobController(IJobService jobService, IJobApplicationService jobApplicationService, ISaveJobService saveService)
        {
            _jobService = jobService;
            _jobApplicationService = jobApplicationService;
            _saveJobService = saveService;
        }



        [HttpGet]
        public async Task<IActionResult> FindJobs(JobSearchMV model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
                return View(model);
            var jobs = await _jobService.SearchJobsAsync(model.Keyword, model.Location, model.Type, model.Presence, includeInactive: false);
            if (model.Sort == "newest")
            {
                jobs = jobs.OrderByDescending(job => job.CreatedOn);
            }
            else if (model.Sort == "oldest")
            {
                jobs = jobs.OrderBy(job => job.CreatedOn);
            }
            else if (model.Sort == "salary-high")
            {
                jobs = jobs.OrderByDescending(job => job.SalaryRange);
            }
            else if (model.Sort == "salary-low")
            {
                jobs = jobs.OrderBy(job => job.SalaryRange);
            }
            var jobDetails = new List<JobMV>();

            foreach (var job in jobs)
            {
                jobDetails.Add(new JobMV
                {
                    Id = job.Id,
                    Title = job.Title,
                    Description = job.Description,
                    Company = job.Company,
                    Location = job.Location,
                    SalaryRange = job.SalaryRange,
                    Type = job.Type ?? JobTypes.FullTime,
                    Presence = job.Presence ?? JobPresence.Onsite,
                    CreatedOn = job.CreatedOn,
                    IsSaved = await _saveJobService.IsJobSavedByUserAsync(job.Id, userId),
                    Applied = await _jobApplicationService.HasUserAppliedForJobAsync(userId, job.Id)
                });
            }

            model.Jobs = jobDetails;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ApplicantDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound();
            if (await _jobApplicationService.HasUserAppliedForJobAsync(userId, id))
            {
                TempData["Message"] = "You have already applied for this job.";
                return RedirectToAction("FindJobs");
            }
            var model = new ApplicationDetailsMV
            {

                JobId = job.Id,
                JobCreatedOn = job.CreatedOn
            };
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> ApplicantDetails(ApplicationDetailsMV model)
        {
            ModelState.Remove("CoverLetterPath");
            if (!ModelState.IsValid)
                return View(model);
            if (model.CoverLetter != null && model.CoverLetter.Length > 0)
            {
                var result = FileManager.UploadFile("uploads/CoverLetters", model.CoverLetter);
                if (result.StartsWith("/"))
                {
                    model.CoverLetterPath = result; // Store the path if upload was successful
                }
                else
                {
                    ModelState.AddModelError("CoverLetter", result); // Add error if upload failed
                    return View(model);
                }
            }
            // Normalize LinkedIn URL
            if (!string.IsNullOrWhiteSpace(model.LinkedInProfile))
            {
                // If it doesn't start with http:// or https://, add https://
                if (!model.LinkedInProfile.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !model.LinkedInProfile.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    model.LinkedInProfile = "https://" + model.LinkedInProfile.Trim();
                }
            }

            model.AppliedOn = DateTime.UtcNow;
            model.Status = ApplicationStatus.Pending.ToString();

            if (model.TermsAccepted == false)
            {
                ModelState.AddModelError("TermsAccepted", "You must accept the terms and conditions.");
                return View(model);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "You must be logged in to apply for a job.");
                return View(model);
            }
            var job = await _jobService.GetJobByIdAsync(model.JobId);
            if (job == null) return NotFound();
            if (job.UserId == userId) return Unauthorized();
            await _jobApplicationService.CreateApplicationAsync(job.Id, userId, model.CoverLetterPath);
            return RedirectToAction("FindJobs");
        }

        [HttpGet]
        public IActionResult AddJob()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddJob(AddJobMV model)
        {
            if (!ModelState.IsValid)
            {
                // Log or inspect ModelState errors
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    // For debugging, you could log or inspect these errors
                    Console.WriteLine(error.ErrorMessage);
                }
                return View(model);
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var newJob = new Job(userId, model.Title, model.Description, model.Company, model.Location, model.SalaryRange, model.Type, model.Presence, model.ExpiresOn);
            await _jobService.CreateJobAsync(newJob);
            return RedirectToAction("JobList", "Job");
        }
        [HttpGet]
        public async Task<IActionResult> JobList(string filter = "all")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Job> jobs;

            switch (filter.ToLower())
            {
                case "active":
                    jobs = (await _jobService.GetActiveJobsAsync())
                          .Where(job => job.UserId == userId);
                    break;
                case "inactive":
                    var allJobs = await _jobService.GetJobsByUserAsync(userId, includeInactive: true);
                    jobs = allJobs.Where(job =>
                        !job.IsActive.GetValueOrDefault() &&
                        !job.IsDeleted.GetValueOrDefault() &&
                        (job.ExpiresOn >= DateTime.UtcNow || job.ExpiresOn == null));
                    break;
                case "expired":
                    var expiredJobs = await _jobService.GetExpiredJobsAsync();
                    jobs = expiredJobs.Where(job => job.UserId == userId);
                    break;
                default:
                    jobs = await _jobService.GetJobsByUserAsync(userId, includeInactive: true);
                    break;
            }

            var jobsList = jobs.ToList();
            var jobDetails = new List<JobListItemMV>();


            foreach (var job in jobsList)
            {
                jobDetails.Add(new JobListItemMV
                {
                    Id = job.Id,
                    Title = job.Title,
                    Description = job.Description,
                    Company = job.Company,
                    Location = job.Location,
                    SalaryRange = job.SalaryRange,
                    Type = job.Type ?? JobTypes.FullTime,
                    Presence = job.Presence ?? JobPresence.Onsite,
                    ExpiresOn = job.ExpiresOn ?? DateTime.UtcNow,
                    Status = job.IsActive == true ? "active" : "inactive",
                    CreatedOn = job.CreatedOn,
                    Applications = await _jobApplicationService.GetApplicationCountForJobAsync(job.Id, includeDeleted: false),
                    Applied = await _jobApplicationService.HasUserAppliedForJobAsync(userId, job.Id),
                    IsSaved = await _saveJobService.IsJobSavedByUserAsync(job.Id, userId)
                });
            }

            var model = new JobListMV
            {
                jobs = jobDetails,
                TotalJobsCount = jobsList.Count,
                ActiveJobsCount = (await _jobService.GetActiveJobsAsync()).Count(),
                ApplicationsCount = await _jobApplicationService.GetApplicationCountByUserAsync(userId, includeDeleted: false),
                CurrentFilter = filter.ToLower()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            if (job.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized();
            }
            await _jobService.DeleteJobAsync(job);
            return RedirectToAction("JobList", "Job");
        }
        [HttpPost]

        public async Task<IActionResult> ToggleJobStatus(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null) return NotFound();
            if (job.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier)) return Unauthorized();

            await _jobService.ToggleJobActivationAsync(job);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> EditJob(int id)
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            if (job.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Unauthorized();
            }
            var model = new EditJobMV
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Company = job.Company,
                Location = job.Location,
                SalaryRange = job.SalaryRange,
                Type = job.Type ?? JobTypes.FullTime,
                Presence = job.Presence ?? JobPresence.Onsite,
                ExpiresOn = job.ExpiresOn ?? DateTime.Now
            };
            return View(model);

        }
        [HttpPost]
        public async Task<IActionResult> EditJob(EditJobMV model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var job = await _jobService.GetJobByIdAsync(model.Id);
            if (job == null)
                return NotFound();
            if (job.UserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
                return Unauthorized();
            if (job.ExpiresOn.HasValue && job.ExpiresOn.Value < DateTime.UtcNow)
                return BadRequest("You cannot edit an expired job.");

            await _jobService.UpdateJobAsync(model.Id, model.Title, model.Description, model.Company, model.Location, model.SalaryRange, model.Type, model.Presence, model.ExpiresOn);
            return RedirectToAction("JobList", "Job");
        }
        [HttpGet]
        public async Task<IActionResult> ViewAppsSummary(int id, string filter = "all")
        {
            var job = await _jobService.GetJobByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            var applications = await _jobApplicationService.GetApplicationsForJobAsync(id);
            switch (filter.ToLower())
            {
                case "pending":
                    applications = applications.Where(app => app.Status == ApplicationStatus.Pending);
                    break;
                case "under-review":
                    applications = applications.Where(app => app.Status == ApplicationStatus.UnderReview);
                    break;
                case "interviewing":
                    applications = applications.Where(app => app.Status == ApplicationStatus.Interviewing);
                    break;
                case "accepted":
                    applications = applications.Where(app => app.Status == ApplicationStatus.Accepted);
                    break;
                case "rejected":
                    applications = applications.Where(app => app.Status == ApplicationStatus.Rejected);
                    break;
                case "archived":
                    applications = applications.Where(app => app.Status == ApplicationStatus.Archived);
                    break;
                case "all":
                default:
                    // no filtering
                    break;
            }
            var model = new JobApplicationsMV
            {
                JobId = job.Id,
                JobTitle = job.Title,
                JobCompany = job.Company,
                JobLocation = job.Location,
                JobType = job.Type.ToString(),
                ExpiresOn = job.ExpiresOn,
                JobCreatedOn = job.CreatedOn,
                TotalApplications = applications.Count(),
                PendingApplications = applications.Count(app => app.Status == ApplicationStatus.Pending),
                InterviewingApplications = applications.Count(app => app.Status == ApplicationStatus.Interviewing),
                AcceptedApplications = applications.Count(app => app.Status == ApplicationStatus.Accepted),
                Applications = applications.Select(app => new JobAppsSummaryMV
                {
                    Id = app.Id,
                    FirstName = app.Applicant.UserName,
                    Email = app.Applicant.Email,
                    Phone = app.Applicant.PhoneNumber,
                    Status = app.Status,
                    AppliedOn = app.AppliedOn,
                    ResumeUrl = app.Applicant.CVPath,
                    CoverLetter = app.CoverLetter
                }).ToList(),
                CurrentFilter = filter.ToLower()
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SavedJob(string filter = "all")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var savedJobs = await _saveJobService.GetUserSavedJobsAsync(userId);


            var jobsList = new List<JobListItemMV>();

            foreach (var sj in savedJobs)
            {
                var applicationsCount = await _jobApplicationService.GetApplicationCountForJobAsync(sj.Job.Id);

                jobsList.Add(new JobListItemMV
                {
                    Id = sj.Id,
                    Title = sj.Job.Title,
                    Company = sj.Job.Company,
                    Description = sj.Job.Description,
                    Location = sj.Job.Location,
                    SalaryRange = sj.Job.SalaryRange,
                    Type = sj.Job.Type ?? JobTypes.FullTime,
                    Presence = sj.Job.Presence ?? JobPresence.Onsite,
                    ExpiresOn = sj.Job.ExpiresOn ?? DateTime.UtcNow,
                    CreatedOn = sj.Job.CreatedOn,
                    Applications = applicationsCount,
                    Applied = await _jobApplicationService.HasUserAppliedForJobAsync(userId, sj.Job.Id)
                });
            }

            jobsList = filter switch
            {
                "applied" => jobsList.Where(j => j.Applied).ToList(),
                "not-applied" => jobsList.Where(j => !j.Applied).ToList(),
                _ => jobsList
            };
            var model = new SavedJobsMV
            {
                Jobs = jobsList,
                SavedJobsCount = jobsList.Count,
                AppliedJobsCount = savedJobs.Count(j => _jobApplicationService.HasUserAppliedForJobAsync(userId, j.Job.Id).Result)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveJob(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _saveJobService.SaveJobAsync(id, userId);
            return RedirectToAction("FindJobs");
        }
        [HttpPost]
        public async Task<IActionResult> UnSaveJob(int jobId)
        {
            await _saveJobService.UnsaveJobAsync(jobId);
            return RedirectToAction("SavedJob");
        }

        [HttpGet]
        public async Task<IActionResult> DisplayDetails(int id)
        {
            var application = await _jobApplicationService.GetApplicationByIdAsync(id);
            if (application == null)
                return NotFound();

          
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           var job = await _jobService.GetJobByIdAsync(application.JobId);
            if (job == null)
                return NotFound();
            if (job.UserId != userId)
                return Unauthorized();
            var model = new ApplicationDetailsMV
            {
                Id = application.Id,
                JobId = job.Id,
                JobCreatedOn = job.CreatedOn,
                FirstName = application.Applicant.UserName,
                Email = application.Applicant.Email,
                Phone = application.Applicant.PhoneNumber,
                CoverLetterPath = application.CoverLetter,
                AppliedOn = application.AppliedOn,
                Status = application.Status.ToString(),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateApplicationStatus(JobAppsSummaryMV model)
        {
            if (model == null) return BadRequest();

            try
            {
                // Use your service method
                await _jobApplicationService.UpdateApplicationStatusAsync(model.Id,model.Status );
                return RedirectToAction("ViewAppsSummary", new { id = model.Id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }

}
