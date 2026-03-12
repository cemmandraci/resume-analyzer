using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Domain.Interfaces;

public interface IResumeRepository
{
    Task<Resume> AddAsync(Resume resume, CancellationToken cancellationToken = default);
    Task<Resume?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Resume?> GetByIdWithAnalysisAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateAsync(Resume resume, CancellationToken cancellationToken = default);
    Task AddAnalysisAsync(ResumeAnalysis resume, CancellationToken cancellationToken = default);
    Task AddJobMatchAsync(JobMatch jobMatch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Resume>> GetAllAsync(CancellationToken cancellationToken = default);
}