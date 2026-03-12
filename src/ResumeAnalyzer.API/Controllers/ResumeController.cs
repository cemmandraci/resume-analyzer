using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResumeAnalyzer.Application.Commands.AnalyzeResume;
using ResumeAnalyzer.Application.Commands.MatchJob;
using ResumeAnalyzer.Application.DTOs;
using ResumeAnalyzer.Application.Queries.GetAllResume;
using ResumeAnalyzer.Application.Queries.GetResume;
using ResumeAnalyzer.Domain.Interfaces;

namespace ResumeAnalyzer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResumeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IAIServiceFactory _aiServiceFactory;

    public ResumeController(IMediator mediator, IAIServiceFactory aiServiceFactory)
    {
        _mediator = mediator;
        _aiServiceFactory = aiServiceFactory;
    }

    [HttpPost("analyze")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Analyze(
        IFormFile file,
        CancellationToken cancellationToken,
        [FromQuery] string provider = "Gemini"
        )
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        if (!file.ContentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase)
            && !file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Only PDF files are accepted");

        if (file.Length > 10 * 1024 * 1024)
            return BadRequest("File size cannot exceed 10MB");
        
        var command = new AnalyzeResumeCommand(
            file.FileName,
            file.ContentType,
            file.Length,
            file.OpenReadStream(),
            provider);

        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }


    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetResumeQuery(id), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet]
    [ProducesResponseType(typeof(ResumeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllResumeQuery(), cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("{id:guid}/match")]
    [ProducesResponseType(typeof(JobMatchDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MatchJob(
        Guid id,
        [FromBody] MatchJobRequest request,
        CancellationToken cancellationToken)
    {
        var command = new MatchJobCommand(id, request.JobTitle, request.JobDescription);
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
    
    [HttpGet("providers")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    public IActionResult GetProviders(CancellationToken cancellationToken)
    {
        var providers = _aiServiceFactory.GetAvailableProviders();
        return Ok(providers);
    }
}

public record MatchJobRequest(string JobTitle, string JobDescription);