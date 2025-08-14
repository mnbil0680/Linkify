using LinkifyDAL.Entities;
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
        public string? Location { get; set; }
        
       
        // Professional Information
        public string? Title { get; set; }
        public string? Bio { get; set; }
        public string? Status { get; set; } // Available, Busy, Do Not Disturb, etc.
        public string? Company { get; set; }
        public string? Industry { get; set; }
        
        // Contact & Social Links
        public string? LinkedInUrl { get; set; }
        public string? GitHubUrl { get; set; }
        public string? PortfolioUrl { get; set; }

        // Professional Details
        public int? YearsOfExperience { get; set; }


        // Skills & Expertise
        public List<SkillMV>? Skills { get; set; }
        public List<LanguageMV>? Languages { get; set; }
        public List<CertificationMV>? Certifications { get; set; }

        // Experience & Career
        public List<ExperienceItemMV>? WorkExperience { get; set; }
        public List<VolunteerExperienceMV>? VolunteerExperience { get; set; }

        // Education & Learning
        public List<EducationItemMV>? Education { get; set; }
        public List<CourseMV>? Courses { get; set; }

        // Projects & Portfolio
        public List<ProjectItemMV>? Projects { get; set; }


        // Network & Social Proof
        public int ConnectionsCount { get; set; }
        public int PostsCount { get; set; }
        public int ProfileViews { get; set; }
       
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public int SharesCount { get; set; }

        // Account & Activity Information
        public DateTime? RegistrationDate { get; set; }
        
        public bool IsPremiumMember { get; set; }

        // Posts
        public List<PostMV> Posts { get; set; }
        public List<Friends> Connections { get; set; }


        //// Recommendations & Endorsements
        //public List<RecommendationMV>? ReceivedRecommendations { get; set; }
        //public List<RecommendationMV>? GivenRecommendations { get; set; }
        //public List<EndorsementMV>? Endorsements { get; set; }
        //public List<TestimonialMV>? Testimonials { get; set; }


        // CTOR
        public ProfileMV()
        {
            Skills = new List<SkillMV>();
            Languages = new List<LanguageMV>();
            Certifications = new List<CertificationMV>();
            WorkExperience = new List<ExperienceItemMV>();
            VolunteerExperience = new List<VolunteerExperienceMV>();
            Education = new List<EducationItemMV>();
            Courses = new List<CourseMV>();
            Projects = new List<ProjectItemMV>();
            Posts = new List<PostMV>();

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
        public string? GitHubUrl { get; set; }

        public List<string>? TechnologiesUsed { get; set; }
        public List<string>? Images { get; set; }
        public string? ProjectType { get; set; } // Personal, Professional, Academic, Open Source
        public string? Role { get; set; }
        public List<string>? TeamMembers { get; set; }
        public List<string>? Achievements { get; set; }
    }


    public class SkillMV
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Range(1, 5)]
        public int? ProficiencyLevel { get; set; } // 1-5 scale 
        public int YearsOfExperience { get; set; }
        public SkillCategory? Category { get; set; } // Technical, Soft, etc.
        public List<string>? Endorsers { get; set; }
        public int EndorsementCount { get; set; }
    }
    public enum SkillCategory 
    {
        TechnicalSkills,
        SoftSkills,
        ManagerialSkills
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