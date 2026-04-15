using System.Threading.Tasks;
using Epros_CareerHubAPI.Helpers;
using Epros_CareerHubAPI.Models.DTOs;
using Epros_CareerHubAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Epros_CareerHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IJwtHelper _jwtHelper;

        public AuthController(IUserRepository userRepository, IPasswordHelper passwordHelper, IJwtHelper jwtHelper)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (dto is null) return BadRequest("Missing payload.");
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Email and password are required.");

            var userWithRole = await _userRepository.GetUserWithRoleByEmailAsync(dto.Email);
            if (userWithRole == null) return Unauthorized("Invalid credentials.");

            if (!_passwordHelper.VerifyHash(dto.Password, userWithRole.Passwordhash))
            {
                return Unauthorized("Invalid credentials.");
            }

            var usersEntity = new Models.Users
            {
                UserId = userWithRole.UserId,
                Email = userWithRole.Email,
                FullName = userWithRole.FullName,
                RoleId = userWithRole.RoleId,
                IsActive = userWithRole.IsActive
            };

            var token = _jwtHelper.GenerateToken(usersEntity, userWithRole.RoleName);

            return Ok(new
            {
                accessToken = token,
                tokenType = "Bearer",
                expiresInMinutes = _jwtHelper.GetExpiresMinutes()
            });
        }
    }
}
