using MediatR;
using ResumeAnalyzer.Application.DTOs;

namespace ResumeAnalyzer.Application.Commands.AnalyzeResume;

public record AnalyzeResumeCommand(
    string FileName,
    string ContentType,
    long FileSize,
    Stream FileStream,
    string AiProvider) : IRequest<ResumeDto>;