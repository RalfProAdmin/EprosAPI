using Epros_CareerHubAPI.Helpers;
using Epros_CareerHubAPI.Models;
using Epros_CareerHubAPI.Models.DTOs;
using Epros_CareerHubAPI.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Epros_CareerHubAPI.Controllers    
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserProfilesController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHelper _passwordHelper;
        private readonly IWebHostEnvironment _env;
        private readonly string _wwwrootPath;

        public UserProfilesController(IUserRepository userRepository, IPasswordHelper passwordHelper, IWebHostEnvironment env)
        {
            _userRepository = userRepository;
            _passwordHelper = passwordHelper;
            _env = env;
            _wwwrootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
        }

        // Minimal: no try/catch for unexpected errors — global middleware will handle them.
        [HttpPost("register-full")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterFull([FromForm] UserRegistrationDto dto)
        {
            if (dto is null) return BadRequest("Missing payload.");
            if (dto.ResumeFile is null || dto.ResumeFile.Length == 0) return BadRequest("Resume file is required.");

            var resumesFolder = Path.Combine(_wwwrootPath, "resumes");
            Directory.CreateDirectory(resumesFolder);

            var storedFileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ResumeFile.FileName);
            var filePath = Path.Combine(resumesFolder, storedFileName);

            // validate + save file + hash + persist
            // if anything fails, ExceptionHandlingMiddleware will log and return ProblemDetails
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ResumeFile.CopyToAsync(stream);
            }

            var passwordHash = _passwordHelper.CreateHash(dto.Password);

            await _userRepository.RegisterUserWithProfileAsync(dto, passwordHash, storedFileName);

            return Ok(new { message = "User and Profile created successfully!" });
        }

        // GET api/userprofiles/{userId}
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var profile = await _userRepository.GetProfileByUserIdAsync(userId);
            if (profile == null) return NotFound();
            return Ok(profile);
        }

    }
}