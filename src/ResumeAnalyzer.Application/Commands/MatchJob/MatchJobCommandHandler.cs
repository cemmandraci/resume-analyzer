using MediatR;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Exceptions;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.Application.Commands.MatchJob;

public class MatchJobCommandHandler : IRequestHandler<MatchJobCommand,JobMatchDto>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IAIServiceFactory _aiServiceFactory;

    public MatchJobCommandHandler(IResumeRepository resumeRepository, IAIServiceFactory aiServiceFactory)
    {
        _resumeRepository = resumeRepository;
        _aiServiceFactory = aiServiceFactory;
    }

    public async Task<JobMatchDto> Handle(MatchJobCommand request, CancellationToken cancellationToken)
    {
        var resume = await _resumeRepository.GetByIdWithAnalysisAsync(request.ResumeId, cancellationToken) ??
                     throw new ResumeNotFoundException(request.ResumeId);

        if (resume.Analysis == null)
            throw new InvalidOperationException("Resume has not been analyzed yet");
        
        var analysisService = _aiServiceFactory.Create(resume.AiProvider);

        var matchResult = await analysisService.MatchWithJobAsync(
            resume.RawText,
            request.JobDescription,
            cancellationToken);

        var jobMatch = JobMatch.Create(
            resume.Analysis.Id,
            request.JobTitle,
            request.JobDescription,
            matchResult.MatchScore,
            matchResult.MatchingSkills,
            matchResult.MissingSkills,
            matchResult.AIFeedback);
        
        resume.Analysis.AddJobMatch(jobMatch);

        return new JobMatchDto
        {
            Id = jobMatch.Id,
            JobTitle = jobMatch.JobTitle,
            MatchScore = jobMatch.MatchScore,
            MatchingSkills = jobMatch.MatchingSkills,
            MissingSkills = jobMatch.MissingSkills,
            AiFeedBack = jobMatch.AIFeedBack,
            CreatedAt = jobMatch.CreatedAt,
        };
    }
}