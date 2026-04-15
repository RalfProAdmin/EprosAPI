using System.Threading.Tasks;
using Epros_CareerHubAPI.Data;
using Epros_CareerHubAPI.Models;
using Epros_CareerHubAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Epros_CareerHubAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly EprosDbContext _dbContext;

        public UserRepository(EprosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> RegisterUserWithProfileAsync(UserRegistrationDto dto, string passwordHash, string resumeFileName)
        {
            var result = await _dbContext.Database.ExecuteSqlRawAsync(
                "EXEC sp_RegisterUserWithProfile @p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8",
                dto.FullName,
                dto.Email,
                passwordHash,
                2, // RoleId hardcoded to 2 (Candidate)
                dto.CurrentRole,
                dto.TotalExperience,
                dto.Summary,
                dto.KeySkills,
                resumeFileName
            );

            return result;
        }

        public async Task<UserProfile> GetProfileByUserIdAsync(int userId)
        {
            return await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        // Single LINQ projection to get user + role name in one query
        public async Task<UserWithRoleDto?> GetUserWithRoleByEmailAsync(string email)
        {
            var query = from u in _dbContext.Users
                        join r in _dbContext.Roles on u.RoleId equals r.RoleID
                        where u.Email == email
                        select new UserWithRoleDto
                        {
                            UserId = u.UserId,
                            FullName = u.FullName,
                            Email = u.Email,
                            Passwordhash = u.Passwordhash,
                            RoleId = u.RoleId,
                            RoleName = r.RoleName,
                            IsActive = u.IsActive
                        };

            return await query.AsNoTracking().FirstOrDefaultAsync();
        }
    }
}