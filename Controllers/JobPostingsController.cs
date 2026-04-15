using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Epros_CareerHubAPI.Models.DTOs;
using Epros_CareerHubAPI.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Epros_CareerHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostingsController : ControllerBase
    {
        private readonly IJobPostingRepository _repo;
        private readonly ILogger<JobPostingsController> _logger;

        public JobPostingsController(IJobPostingRepository repo, ILogger<JobPostingsController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // GET: api/jobpostings
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _repo.GetAllAsync();
            return Ok(list);
        }

        // GET: api/jobpostings/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var item = await _repo.GetByIdAsync(id);
                if (item == null) return NotFound();
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving job posting {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        // POST: api/jobpostings
        // Use the authenticated user's id as PostedByUserId
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] JobPostingDto dto)
        {
            if (dto is null) return BadRequest("Missing payload.");
            if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required.");
            if (dto.MinExperiences < 0 || dto.MaxExperiences < 0) return BadRequest("Experience values must be non-negative.");
            if (dto.MaxExperiences < dto.MinExperiences) return BadRequest("MaxExperiences must be >= MinExperiences.");

            // extract user id from token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                // override PostedByUserId with authenticated user id
                dto.PostedByUserId = userId;

                var created = await _repo.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.JobId }, created);
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Validation failed creating job posting for user {UserId}", dto.PostedByUserId);
                return BadRequest(invEx.Message);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error creating job posting for user {UserId}", dto.PostedByUserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating job posting for user {UserId}", dto.PostedByUserId);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        // PUT: api/jobpostings/{id}
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] JobPostingDto dto)
        {
            if (dto is null) return BadRequest("Missing payload.");

            // extract user id from token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                // Optional: ensure the authenticated user is the owner of the posting
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null) return NotFound();
                if (existing.PostedByUserId != userId)
                {
                    return Forbid(); // or return Unauthorized()
                }

                await _repo.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error updating job posting {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating job posting {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        // DELETE (soft): api/jobpostings/{id}
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // extract user id from token
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub);
            if (idClaim == null || !int.TryParse(idClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            try
            {
                // Optional: ensure the authenticated user is the owner
                var existing = await _repo.GetByIdAsync(id);
                if (existing == null) return NotFound();
                if (existing.PostedByUserId != userId)
                {
                    return Forbid();
                }

                await _repo.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "DB error deleting job posting {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "Database error occurred.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error deleting job posting {JobId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}