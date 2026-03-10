using MediatR;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Application.Queries.GetAllResume;

public class GetAllResumeQueryHandler : IRequestHandler<GetAllResumeQuery, IReadOnlyList<ResumeDto>>
{
    private readonly IResumeRepository _resumeRepository;

    public GetAllResumeQueryHandler(IResumeRepository repository)
    {
        _resumeRepository = repository;
    }

    public async Task<IReadOnlyList<ResumeDto>> Handle(GetAllResumeQuery request, CancellationToken cancellationToken)
    {
        var resumes = await _resumeRepository.GetAllAsync(cancellationToken);

        return resumes.Select(r => new ResumeDto
        {
            Id = r.Id,
            FileName = r.FileName,
            FileSize = r.FileSize,
            Status = r.Status.ToString(),
            AiProvider = r.AiProvider,
            FailureReason = r.FailureReason,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}