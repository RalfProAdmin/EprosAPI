namespace Epros_CareerHubAPI.Models.DTOs
{
    public class JobPostingDto
    {
        public int PostedByUserId { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string DescriptionHtml { get; set; }
        public int MinExperiences { get; set; }
        public int MaxExperiences { get; set; }
        public string SalaryRange { get; set; }
        // Allow client to set IsActive on create/update; default handling performed server-side
        public bool? IsActive { get; set; }
    }
}