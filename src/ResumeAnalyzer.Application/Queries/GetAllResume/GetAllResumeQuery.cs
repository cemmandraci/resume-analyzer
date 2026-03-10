using MediatR;
using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Queries.GetAllResume;

public record GetAllResumeQuery : IRequest<IReadOnlyList<ResumeDto>>;