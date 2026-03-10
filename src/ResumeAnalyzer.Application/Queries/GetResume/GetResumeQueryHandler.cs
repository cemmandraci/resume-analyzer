using MediatR;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Exceptions;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Application.Queries.GetResume;

public class GetResumeQueryHandler : IRequestHandler<GetResumeQuery, ResumeDto>
{
    private readonly IResumeRepository _resumeRepository;

    public GetResumeQueryHandler(IResumeRepository resumeRepository)
    {
        _resumeRepository = resumeRepository;
    }

    public async Task<ResumeDto> Handle(GetResumeQuery request, CancellationToken cancellationToken)
    {
        var resume = await _resumeRepository.GetByIdWithAnalysisAsync(request.Id, cancellationToken) ??
                     throw new ResumeNotFoundException(request.Id);

        return new ResumeDto
        {
            Id = resume.Id,
            FileName = resume.FileName,
            FileSize = resume.FileSize,
            Status = resume.Status.ToString(),
            AiProvider = resume.AiProvider,
            FailureReason = resume.FailureReason,
            CreatedAt = resume.CreatedAt
        };
    }
}