using MediatR;
using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Commands.MatchJob;

public record MatchJobCommand(
    Guid ResumeId,
    string JobTitle,
    string JobDescription) : IRequest<JobMatchDto>;