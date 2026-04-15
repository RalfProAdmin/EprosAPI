using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epros_CareerHubAPI.Data;
using Epros_CareerHubAPI.Models;
using Epros_CareerHubAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Epros_CareerHubAPI.Repositories
{
    public class JobPostingRepository : IJobPostingRepository
    {
        private readonly EprosDbContext _dbContext;

        public JobPostingRepository(EprosDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<JobPostingResponseDto>> GetAllAsync()
        {
            var list = await (from j in _dbContext.JobPostings.AsNoTracking()
                              join u in _dbContext.Users.AsNoTracking() on j.PostedByUserId equals u.UserId
                              select new JobPostingResponseDto
                              {
                                  JobId = j.JobId,
                                  PostedByUserId = j.PostedByUserId,
                                  PostedByFullName = u.FullName,
                                  Title = j.Title,
                                  Department = j.Department,
                                  DescriptionHtml = j.DescriptionHtml,
                                  MinExperiences = j.MinExperiences,
                                  MaxExperiences = j.MaxExperiences,
                                  SalaryRange = j.SalaryRange,
                                  CreatedAt = j.CreatedAt,
                                  UpdatedAt = j.UpdatedAt,
                                  IsActive = j.IsActive
                              }).ToListAsync();

            return list;
        }

        public async Task<JobPostingResponseDto?> GetByIdAsync(int id)
        {
            var item = await (from j in _dbContext.JobPostings.AsNoTracking()
                              join u in _dbContext.Users.AsNoTracking() on j.PostedByUserId equals u.UserId
                              where j.JobId == id
                              select new JobPostingResponseDto
                              {
                                  JobId = j.JobId,
                                  PostedByUserId = j.PostedByUserId,
                                  PostedByFullName = u.FullName,
                                  Title = j.Title,
                                  Department = j.Department,
                                  DescriptionHtml = j.DescriptionHtml,
                                  MinExperiences = j.MinExperiences,
                                  MaxExperiences = j.MaxExperiences,
                                  SalaryRange = j.SalaryRange,
                                  CreatedAt = j.CreatedAt,
                                  UpdatedAt = j.UpdatedAt,
                                  IsActive = j.IsActive
                              }).FirstOrDefaultAsync();

            return item;
        }

        public async Task<JobPostingResponseDto> CreateAsync(JobPostingDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            // Validate posted-by user exists
            var userExists = await _dbContext.Users.AnyAsync(u => u.UserId == dto.PostedByUserId);
            if (!userExists) throw new InvalidOperationException("PostedByUserId does not exist.");

            var entity = new JobPosting
            {
                PostedByUserId = dto.PostedByUserId,
                Title = dto.Title,
                Department = dto.Department,
                DescriptionHtml = dto.DescriptionHtml,
                MinExperiences = dto.MinExperiences,
                MaxExperiences = dto.MaxExperiences,
                SalaryRange = dto.SalaryRange,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                IsActive = dto.IsActive ?? true
            };

            _dbContext.JobPostings.Add(entity);
            await _dbContext.SaveChangesAsync();

            var postedBy = await _dbContext.Users.FindAsync(entity.PostedByUserId);

            return new JobPostingResponseDto
            {
                JobId = entity.JobId,
                PostedByUserId = entity.PostedByUserId,
                PostedByFullName = postedBy?.FullName,
                Title = entity.Title,
                Department = entity.Department,
                DescriptionHtml = entity.DescriptionHtml,
                MinExperiences = entity.MinExperiences,
                MaxExperiences = entity.MaxExperiences,
                SalaryRange = entity.SalaryRange,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt,
                IsActive = entity.IsActive
            };
        }

        public async Task UpdateAsync(int id, JobPostingDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _dbContext.JobPostings.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Job posting not found.");

            if (!string.IsNullOrWhiteSpace(dto.Title)) entity.Title = dto.Title;
            entity.Department = dto.Department;
            entity.DescriptionHtml = dto.DescriptionHtml;
            entity.MinExperiences = dto.MinExperiences;
            entity.MaxExperiences = dto.MaxExperiences;
            entity.SalaryRange = dto.SalaryRange;
            entity.IsActive = dto.IsActive ?? entity.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _dbContext.JobPostings.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbContext.JobPostings.FindAsync(id);
            if (entity == null) throw new KeyNotFoundException("Job posting not found.");

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;

            _dbContext.JobPostings.Update(entity);
            await _dbContext.SaveChangesAsync();
        }
    }
}