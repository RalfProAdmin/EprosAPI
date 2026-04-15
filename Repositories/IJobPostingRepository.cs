using System.Collections.Generic;
using System.Threading.Tasks;
using Epros_CareerHubAPI.Models.DTOs;

namespace Epros_CareerHubAPI.Repositories
{
    public interface IJobPostingRepository
    {
        Task<List<JobPostingResponseDto>> GetAllAsync();
        Task<JobPostingResponseDto?> GetByIdAsync(int id);
        Task<JobPostingResponseDto> CreateAsync(JobPostingDto dto);
        Task UpdateAsync(int id, JobPostingDto dto);
        Task DeleteAsync(int id);
    }
}