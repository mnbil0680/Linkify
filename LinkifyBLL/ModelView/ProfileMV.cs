using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkifyBLL.ModelView
{
    public class ProfileMV
    {
        // Basic Information
        public string Id { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ImgPath { get; set; }
        public IFormFile? Image { get; set; }
        public string? Timezone { get; set; }
        public string? PreferredLanguage { get; set; }

        // Professional Information
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? Status { get; set; } // Available, Busy, Do Not Disturb, etc.
        public string? Company { get; set; }
        public string? Industry { get; set; }
        public string? Location { get; set; }
        public string? Website { get; set; }
        public string? CompanySize { get; set; } // Startup, Small, Medium, Large, Enterprise
        public string? SeniorityLevel { get; set; } // Entry, Mid, Senior, Lead, Director, VP, C-Level

        // Contact & Social Links
        public string? LinkedInUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? FacebookUrl { get; set; }
        public string? YouTubeUrl { get; set; }
        public string? TikTokUrl { get; set; }
        public string? MediumUrl { get; set; }
        public string? DevToUrl { get; set; }
        public string? StackOverflowUrl { get; set; }
        public string? BehanceUrl { get; set; }
        public string? DribbbleUrl { get; set; }

        // Professional Details
        public int? YearsOfExperience { get; set; }

        public string? CurrentSalaryRange { get; set; }
        public decimal? ExpectedSalaryMin { get; set; }
        public decimal? ExpectedSalaryMax { get; set; }
        public string? SalaryCurrency { get; set; }

        public bool IsOpenToWork { get; set; }
        public bool IsOpenToNetworking { get; set; }
        public bool IsOpenToMentoring { get; set; }
        public bool IsSeekingMentor { get; set; }
        public bool IsAvailableForFreelance { get; set; }
        public bool IsAvailableForConsulting { get; set; }

        public string? PreferredJobType { get; set; } // Remote, Hybrid, On-site
        public List<string>? PreferredRoles { get; set; }
        public List<string>? PreferredIndustries { get; set; }
        public List<string>? PreferredCompanySizes { get; set; }
        public List<string>? PreferredLocations { get; set; }

        // Skills & Expertise
        public List<SkillMV>? Skills { get; set; }
        public List<LanguageMV>? Languages { get; set; }
        public List<CertificationMV>? Certifications { get; set; }
        public List<string>? Interests { get; set; }
        public List<string>? Hobbies { get; set; }

        // Experience & Career
        public List<ExperienceItemMV>? WorkExperience { get; set; }
        public List<VolunteerExperienceMV>? VolunteerExperience { get; set; }

        // Education & Learning
        public List<EducationItemMV>? Education { get; set; }
        public List<CourseMV>? Courses { get; set; }

        // Projects & Portfolio
        public List<ProjectItemMV>? Projects { get; set; }
        public List<PublicationMV>? Publications { get; set; }
        public List<PatentMV>? Patents { get; set; }

        // Achievements & Recognition
        public List<AchievementItemMV>? Achievements { get; set; }
        public List<AwardMV>? Awards { get; set; }
        public List<HonorMV>? Honors { get; set; }

        // Network & Social Proof
        public int ConnectionsCount { get; set; }
        public int PostsCount { get; set; }
        public int ProfileViews { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int LikesReceived { get; set; }
        public int CommentsReceived { get; set; }
        public int SharesReceived { get; set; }
        public double ProfileCompletionPercentage { get; set; }
        public int ProfileStrengthScore { get; set; }

        // Account & Activity Information
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LastActiveDate { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public DateTime? LastProfileUpdateDate { get; set; }
        public bool IsVerified { get; set; }
        public string? VerificationType { get; set; } // Email, Phone, Identity, Premium
        public bool IsPremiumMember { get; set; }
        public DateTime? PremiumExpiryDate { get; set; }

        // Privacy & Preferences
        public bool IsProfilePublic { get; set; }
        public bool ShowEmail { get; set; }
        public bool ShowPhoneNumber { get; set; }
        public bool ShowSalaryInfo { get; set; }
        public bool ShowExperienceDetails { get; set; }
        public bool ShowEducationDetails { get; set; }
        public bool AllowConnectionRequests { get; set; }
        public bool AllowDirectMessages { get; set; }
        public bool AllowJobOffers { get; set; }
        public bool AllowBusinessProposals { get; set; }
        public bool ShowActiveStatus { get; set; }
        public bool AllowDataForResearch { get; set; }
        public bool ReceiveNewsletters { get; set; }
        public bool ReceiveNotifications { get; set; }

        // Recommendations & Endorsements
        public List<RecommendationMV>? ReceivedRecommendations { get; set; }
        public List<RecommendationMV>? GivenRecommendations { get; set; }
        public List<EndorsementMV>? Endorsements { get; set; }
        public List<TestimonialMV>? Testimonials { get; set; }

        // Professional Preferences
        public string? WorkStyle { get; set; } // Collaborative, Independent, Mixed
        public string? CommunicationStyle { get; set; } // Direct, Diplomatic, Casual, Formal
        public List<string>? PreferredCollaborationTools { get; set; }
        public string? PreferredMeetingStyle { get; set; } // In-person, Virtual, Hybrid

        // Additional Professional Info
        public bool HasManagementExperience { get; set; }
        public int? TeamSize { get; set; }
        public decimal? BudgetManaged { get; set; }
        public List<string>? ManagementAreas { get; set; }

        public ProfileMV()
        {
            Skills = new List<SkillMV>();
            Languages = new List<LanguageMV>();
            Certifications = new List<CertificationMV>();
            Interests = new List<string>();
            Hobbies = new List<string>();
            WorkExperience = new List<ExperienceItemMV>();
            VolunteerExperience = new List<VolunteerExperienceMV>();
            Education = new List<EducationItemMV>();
            Courses = new List<CourseMV>();
            Projects = new List<ProjectItemMV>();
            Publications = new List<PublicationMV>();
            Patents = new List<PatentMV>();
            Achievements = new List<AchievementItemMV>();
            Awards = new List<AwardMV>();
            Honors = new List<HonorMV>();
            ReceivedRecommendations = new List<RecommendationMV>();
            GivenRecommendations = new List<RecommendationMV>();
            Endorsements = new List<EndorsementMV>();
            Testimonials = new List<TestimonialMV>();
            PreferredRoles = new List<string>();
            PreferredIndustries = new List<string>();
            PreferredCompanySizes = new List<string>();
            PreferredLocations = new List<string>();
            PreferredCollaborationTools = new List<string>();
            ManagementAreas = new List<string>();
        }
    }

    // Enhanced supporting classes
    public class ExperienceItemMV
    {
        public string JobTitle { get; set; }
        public string Company { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanySize { get; set; }
        public string? CompanyIndustry { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

        public string? EmploymentType { get; set; } // Full-time, Part-time, Contract, Freelance, Internship
        public List<string>? Skills { get; set; }
        public List<string>? Achievements { get; set; }
        public List<string>? Responsibilities { get; set; }
        public decimal? SalaryAmount { get; set; }
        public string? SalaryCurrency { get; set; }
        public bool ShowSalary { get; set; }
    }

    public class VolunteerExperienceMV
    {
        public string Role { get; set; }
        public string Organization { get; set; }
        public string? OrganizationLogo { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public string? Description { get; set; }
        public string? Cause { get; set; }
        public List<string>? Skills { get; set; }
    }

    public class EducationItemMV
    {
        public string School { get; set; }
        public string Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Grade { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

        public string? DegreeType { get; set; } // Bachelor's, Master's, PhD, Certificate, etc.
        public List<string>? Coursework { get; set; }
        public List<string>? Activities { get; set; }
        public bool IsCurrentlyEnrolled { get; set; }
    }

    public class CourseMV
    {
        public string Name { get; set; }
        public string? Provider { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string? CertificateUrl { get; set; }
        public string? Description { get; set; }
        public List<string>? Skills { get; set; }
        public int? DurationHours { get; set; }
    }

    public class ProjectItemMV
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsOngoing { get; set; }
        public string? ProjectUrl { get; set; }
        public string? RepositoryUrl { get; set; }

        public List<string>? TechnologiesUsed { get; set; }
        public List<string>? Images { get; set; }
        public string? ProjectType { get; set; } // Personal, Professional, Academic, Open Source
        public string? Role { get; set; }
        public List<string>? TeamMembers { get; set; }
        public List<string>? Achievements { get; set; }
    }

    public class PublicationMV
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        public DateTime PublicationDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Url]
        public string? Url { get; set; }

        public List<string>? Authors { get; set; }
        public string? PublicationType { get; set; } // Article, Book, Research Paper, etc.
    }

    public class PatentMV
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string? PatentNumber { get; set; }
        public DateTime? ApplicationDate { get; set; }
        public DateTime? IssueDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Url]
        public string? Url { get; set; }

        public List<string>? Inventors { get; set; }
        public string? Status { get; set; } // Pending, Granted, Expired
    }

    public class AchievementItemMV
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime DateAchieved { get; set; }

        [StringLength(100)]
        public string? IssuingOrganization { get; set; }

        [Url]
        public string? CertificateUrl { get; set; }

        public string? AchievementType { get; set; } // Award, Recognition, Milestone, etc.
        public bool IsPublic { get; set; }
    }

    public class AwardMV
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(100)]
        public string? Issuer { get; set; }

        public DateTime DateReceived { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Url]
        public string? Url { get; set; }
    }

    public class HonorMV
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(100)]
        public string? Issuer { get; set; }

        public DateTime DateReceived { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }
    }

    public class SkillMV
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Range(1, 5)]
        public int? ProficiencyLevel { get; set; } // 1-5 scale

        public int YearsOfExperience { get; set; }
        public bool IsPrimary { get; set; }
        public string? Category { get; set; } // Technical, Soft, Language, etc.
        public List<string>? Endorsers { get; set; }
        public int EndorsementCount { get; set; }
    }

    public class LanguageMV
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(20)]
        public string? ProficiencyLevel { get; set; } // Native, Fluent, Conversational, Basic

        public bool IsNative { get; set; }
        public List<string>? Certifications { get; set; }
    }

    public class CertificationMV
    {
        [Required]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(100)]
        public string? IssuingOrganization { get; set; }

        public DateTime DateObtained { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool DoesNotExpire { get; set; }

        [Url]
        public string? CertificateUrl { get; set; }

        public string? CredentialId { get; set; }
        public List<string>? Skills { get; set; }
    }

    public class RecommendationMV
    {
        [Required]
        [StringLength(100)]
        public string RecommenderName { get; set; }

        [StringLength(150)]
        public string? RecommenderTitle { get; set; }

        public string? RecommenderImagePath { get; set; }

        [Required]
        [StringLength(2000)]
        public string Content { get; set; }

        public DateTime DateGiven { get; set; }

        [StringLength(50)]
        public string? Relationship { get; set; } // Colleague, Manager, Client, etc.

        public string? RecommenderId { get; set; }
        public bool IsPublic { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
    }

    public class EndorsementMV
    {
        [Required]
        [StringLength(50)]
        public string SkillName { get; set; }

        public int EndorsementCount { get; set; }
        public List<EndorserMV>? Endorsers { get; set; }
        public DateTime? LastEndorsementDate { get; set; }
    }

    public class EndorserMV
    {
        public string EndorserId { get; set; }
        public string Name { get; set; }
        public string? Title { get; set; }
        public string? ImagePath { get; set; }
        public DateTime DateEndorsed { get; set; }
    }

    public class TestimonialMV
    {
        [Required]
        [StringLength(100)]
        public string ClientName { get; set; }

        [StringLength(100)]
        public string? ClientCompany { get; set; }

        [StringLength(150)]
        public string? ClientTitle { get; set; }

        [Required]
        [StringLength(1000)]
        public string Content { get; set; }

        public DateTime DateGiven { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? ProjectName { get; set; }
        public bool IsPublic { get; set; }
    }
}