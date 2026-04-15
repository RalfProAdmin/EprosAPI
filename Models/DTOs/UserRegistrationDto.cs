namespace Epros_CareerHubAPI.Models.DTOs
{
    public class UserRegistrationDto
    {// User Table Info
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        // Profile Table Info
        public string CurrentRole { get; set; }
        public decimal TotalExperience { get; set; }
        public string Summary { get; set; }
        public string KeySkills { get; set; }
        public IFormFile ResumeFile { get; set; } // For the file upload
    }
}
