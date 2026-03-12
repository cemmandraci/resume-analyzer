using Microsoft.EntityFrameworkCore;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Infrastructure.Persistence.Repositories;

public class ResumeRepository : IResumeRepository
{
    private readonly ResumeDbContext _context;

    public ResumeRepository(ResumeDbContext context)
    {
        _context = context;
    }

    public async Task<Resume> AddAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        await _context.Resumes.AddAsync(resume, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return resume;
    }

    public async Task<Resume?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Resumes.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Resume?> GetByIdWithAnalysisAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Resumes
            .Include(x => x.Analysis)
            .ThenInclude(x => x!.WorkExperiences)
            .Include(x => x.Analysis)
            .ThenInclude(x => x!.Educations)
            .Include(x => x.Analysis)
            .ThenInclude(x => x!.JobMatches)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task UpdateAsync(Resume resume, CancellationToken cancellationToken = default)
    {
        _context.Update(resume);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAnalysisAsync(ResumeAnalysis analysis, CancellationToken cancellationToken = default)
    {
        await _context.ResumeAnalyses.AddAsync(analysis, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddJobMatchAsync(JobMatch jobMatch, CancellationToken cancellationToken = default)
    {
        await _context.JobMatches.AddAsync(jobMatch, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Resume>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Resumes
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}