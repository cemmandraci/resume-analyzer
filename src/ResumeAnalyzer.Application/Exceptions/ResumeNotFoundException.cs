namespace ResumeAnalyzer.Application.Exceptions;

public class ResumeNotFoundException : Exception
{
    public ResumeNotFoundException(Guid id) : base($"Resume with id {id} was not found") {}
}