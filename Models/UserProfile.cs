using System.ComponentModel.DataAnnotations;

namespace Epros_CareerHubAPI.Models
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public string CurrentRole { get; set; }
        public decimal TotalExperience { get; set; }
        public string Summary { get; set; }
        public string ResumeUrl { get; set; }
        public string Skills { get; set; }
    }
}
