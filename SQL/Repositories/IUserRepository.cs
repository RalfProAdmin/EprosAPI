using System.Threading.Tasks;
using Epros_CareerHubAPI.Models;
using Epros_CareerHubAPI.Models.DTOs;

namespace Epros_CareerHubAPI.Repositories
{
    public interface IUserRepository
    {
        Task<int> RegisterUserWithProfileAsync(UserRegistrationDto dto, string passwordHash, string resumeFileName);
        Task<UserProfile> GetProfileByUserIdAsync(int userId);
        Task<UserWithRoleDto?> GetUserWithRoleByEmailAsync(string email);
    }
}