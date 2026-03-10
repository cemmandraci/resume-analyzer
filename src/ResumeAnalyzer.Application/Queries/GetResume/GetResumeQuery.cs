using MediatR;
using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Queries.GetResume;

public record GetResumeQuery(Guid Id) : IRequest<ResumeDto>;