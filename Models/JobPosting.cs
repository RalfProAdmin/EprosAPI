namespace Epros_CareerHubAPI.Models
{
    public class JobPosting
    {
        public int JobId { get; set; }
        public int PostedByUserId { get; set; }
        public string Title { get; set; }
        public string Department { get; set; }
        public string DescriptionHtml { get; set; }
        public int MinExperiences { get; set; }
        public int MaxExperiences { get; set; }
        public string SalaryRange { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Optional navigation property for EF usage
        public Users PostedByUser { get; set; }
    }
}