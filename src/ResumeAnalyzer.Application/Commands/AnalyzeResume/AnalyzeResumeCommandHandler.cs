using MediatR;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Domain.ValueObjects;

namespace ResumeAnalyzer.Application.Commands.AnalyzeResume;

public class AnalyzeResumeCommandHandler : IRequestHandler<AnalyzeResumeCommand, ResumeDto>
{
    private readonly IResumeRepository _resumeRepository;
    private readonly IPdfParserService _pdfParserService;
    private readonly IAIServiceFactory _aiServiceFactory;

    public AnalyzeResumeCommandHandler(
        IResumeRepository resumeRepository,
        IPdfParserService pdfParserService,
        IAIServiceFactory aiServiceFactory)
    {
        _resumeRepository = resumeRepository;
        _pdfParserService = pdfParserService;
        _aiServiceFactory = aiServiceFactory;
    }

    public async Task<ResumeDto> Handle(AnalyzeResumeCommand request, CancellationToken cancellationToken)
    {
        var rawText = await _pdfParserService.ParseAsync(request.FileStream, cancellationToken);

        var analysisService = _aiServiceFactory.Create(request.AiProvider);

        var resume = Resume.Create(
            request.FileName,
            request.ContentType,
            request.FileSize,
            rawText,
            analysisService.ProviderName);

        await _resumeRepository.AddAsync(resume, cancellationToken);

        try
        {
            resume.MarkAsProcessing();
            await _resumeRepository.UpdateAsync(resume, cancellationToken);

            var analysisResult = await analysisService.AnalyzeAsync(
                rawText, cancellationToken);

            var analysis = ResumeAnalysis.Create(
                resume.Id,
                analysisResult.PersonelInfo,
                analysisResult.WorkExperiences,
                analysisResult.Educations,
                analysisResult.Skills,
                analysisResult.Strengths,
                analysisResult.Weaknesses,
                analysisResult.Suggestions);
            
            resume.MarkAsCompleted(analysis);
            await _resumeRepository.UpdateAsync(resume, cancellationToken);

        }
        catch (Exception ex)
        {
            resume.MarkAsFailed(ex.Message);
            await _resumeRepository.UpdateAsync(resume, cancellationToken);
            throw;
        }

        return MapToDto(resume);
    }

    private static ResumeDto MapToDto(Resume resume) => new()
    {
        Id = resume.Id,
        FileName = resume.FileName,
        FileSize = resume.FileSize,
        Status = resume.Status.ToString(),
        AiProvider = resume.AiProvider,
        FailureReason = resume.FailureReason,
        CreatedAt = resume.CreatedAt,
        Analysis = resume.Analysis == null
            ? null
            : new ResumeAnalysisDto
            {
                Id = resume.Analysis.Id,
                PersonelInfoDto = new PersonelInfoDto
                {
                    FullName = resume.Analysis.PersonelInfo.FullName,
                    Email = resume.Analysis.PersonelInfo.Email,
                    Phone = resume.Analysis.PersonelInfo.Phone,
                    Location = resume.Analysis.PersonelInfo.Location,
                    LinkedInUrl = resume.Analysis.PersonelInfo.LinkedInUrl,
                    GitHubUrl = resume.Analysis.PersonelInfo.GitHubUrl,
                    Summary = resume.Analysis.PersonelInfo.Summary
                },
                WorkExperiences = resume.Analysis.WorkExperiences.Select(w => new WorkExperienceDto
                {
                    Company = w.Company,
                    Position = w.Position,
                    Description = w.Description,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    IsCurrent = w.IsCurrent,
                    Responsibilities = w.Responsibilities
                }).ToList(),
                Educations = resume.Analysis.Educations.Select(e => new EducationDto
                {
                    Institution = e.Institution,
                    Degree = e.Degree,
                    Field = e.Field,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    GPA = e.GPA
                }).ToList(),
                Skills = resume.Analysis.Skills.Select(s => new SkillDto
                {
                    Name = s.Name,
                    Level = s.Level.ToString(),
                    Category = s.Category,
                }).ToList(),
                Strengths = resume.Analysis.Strength,
                Weaknesses = resume.Analysis.Weaknesses,
                Suggestions = resume.Analysis.Suggestions,
                JobMatches = resume.Analysis.JobMatches.Select(j => new JobMatchDto
                {
                    Id = j.Id,
                    JobTitle = j.JobTitle,
                    MatchScore = j.MatchScore,
                    MatchingSkills = j.MatchingSkills,
                    MissingSkills = j.MissingSkills,
                    AiFeedBack = j.AIFeedBack,
                    CreatedAt = j.CreatedAt
                }).ToList(),
                CreatedAt = resume.Analysis.CreatedAt
            }
    };
}