using System.Text.Json;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.Enums;
using ResumeAnalyzer.Domain.Interfaces;
using ResumeAnalyzer.Domain.ValueObjects;

namespace ResumeAnalyzer.Infrastructure.AI.Base;

public abstract class BaseResumeAnalysisService : IResumeAnalysisService
{
    public abstract string ProviderName { get; }

    public async Task<ResumeAnalysisResult> AnalyzeAsync(
        string resumeText,
        CancellationToken cancellationToken = default)
    {
        var prompt = BuildAnalysisPrompt(resumeText);
        var responseJson = await SendRequestAsync(prompt, cancellationToken);
        return ParseAnalysisResult(responseJson);
    }

    public async Task<JobMatchResult> MatchWithJobAsync(
        string resumeText,
        string jobDescription,
        CancellationToken cancellationToken = default)
    {
        var prompt = BuildJobMatchPrompt(resumeText, jobDescription);
        var responseJson = await SendRequestAsync(prompt, cancellationToken);
        return ParseJobMatchResult(responseJson);
    }

    // Her provider kendi HTTP isteğini implement eder
    protected abstract Task<string> SendRequestAsync(
        string prompt,
        CancellationToken cancellationToken);

    // Prompt'lar her provider için aynı — override edilebilir
    protected virtual string BuildAnalysisPrompt(string resumeText)
    {
        return @"Analyze the following resume and extract all information. Return a JSON object with this exact structure:
        {
            ""personalInfo"": {
                ""fullName"": """",
                ""email"": """",
                ""phone"": """",
                ""location"": """",
                ""linkedInUrl"": null,
                ""gitHubUrl"": null,
                ""summary"": null
            },
            ""workExperiences"": [
                {
                    ""company"": """",
                    ""position"": """",
                    ""description"": null,
                    ""startDate"": ""2020-01-01"",
                    ""endDate"": null,
                    ""responsibilities"": []
                }
            ],
            ""educations"": [
                {
                    ""institution"": """",
                    ""degree"": """",
                    ""field"": """",
                    ""startDate"": ""2016-01-01"",
                    ""endDate"": ""2020-01-01"",
                    ""gpa"": null
                }
            ],
            ""skills"": [
                {
                    ""name"": """",
                    ""level"": ""Intermediate"",
                    ""category"": null
                }
            ],
            ""strengths"": [],
            ""weaknesses"": [],
            ""suggestions"": []
        }

        Resume text:
        " + resumeText;
            }

    protected virtual string BuildJobMatchPrompt(string resumeText, string jobDescription)
    {
        return @"Compare the following resume with the job description and return a JSON object:
                {
                    ""matchScore"": 85,
                    ""matchingSkills"": [],
                    ""missingSkills"": [],
                    ""aiFeedback"": """"
                }

                matchScore must be between 0-100.

                Resume:
                " + resumeText + @"

                Job Description:
                " + jobDescription;
                    }

    // Tüm parse logic burada — provider'lar tekrar yazmaz
    private static ResumeAnalysisResult ParseAnalysisResult(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var personalInfo = new PersonelInfo
        {
            FullName = GetString(root, "personalInfo", "fullName"),
            Email = GetString(root, "personalInfo", "email"),
            Phone = GetString(root, "personalInfo", "phone"),
            Location = GetString(root, "personalInfo", "location"),
            LinkedInUrl = GetNullableString(root, "personalInfo", "linkedInUrl"),
            GitHubUrl = GetNullableString(root, "personalInfo", "gitHubUrl"),
            Summary = GetNullableString(root, "personalInfo", "summary")
        };

        return new ResumeAnalysisResult(
            personalInfo,
            ParseWorkExperiences(root),
            ParseEducations(root),
            ParseSkills(root),
            ParseStringList(root, "strengths"),
            ParseStringList(root, "weaknesses"),
            ParseStringList(root, "suggestions"));
    }

    private static JobMatchResult ParseJobMatchResult(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        return new JobMatchResult(
            root.TryGetProperty("matchScore", out var score) ? score.GetInt32() : 0,
            ParseStringList(root, "matchingSkills"),
            ParseStringList(root, "missingSkills"),
            root.TryGetProperty("aiFeedback", out var f) ? f.GetString() ?? "" : "");
    }

    private static List<WorkExperience> ParseWorkExperiences(JsonElement root)
    {
        var list = new List<WorkExperience>();
        if (!root.TryGetProperty("workExperiences", out var experiences)) return list;

        foreach (var exp in experiences.EnumerateArray())
        {
            var startDate = DateTime.TryParse(
                exp.TryGetProperty("startDate", out var sd) ? sd.GetString() : null,
                out var parsedStart) ? parsedStart : DateTime.UtcNow.AddYears(-1);

            DateTime? endDate = null;
            if (exp.TryGetProperty("endDate", out var ed) && ed.ValueKind != JsonValueKind.Null)
                endDate = DateTime.TryParse(ed.GetString(), out var parsedEnd) ? parsedEnd : null;

            var responsibilities = new List<string>();
            if (exp.TryGetProperty("responsibilities", out var resp))
                responsibilities.AddRange(resp.EnumerateArray()
                    .Select(r => r.GetString() ?? string.Empty)
                    .Where(r => !string.IsNullOrEmpty(r)));

            list.Add(WorkExperience.Create(
                Guid.Empty,
                exp.TryGetProperty("company", out var c) ? c.GetString() ?? "" : "",
                exp.TryGetProperty("position", out var p) ? p.GetString() ?? "" : "",
                startDate, endDate,
                exp.TryGetProperty("description", out var d) && d.ValueKind != JsonValueKind.Null
                    ? d.GetString() : null,
                responsibilities));
        }

        return list;
    }

    private static List<Education> ParseEducations(JsonElement root)
    {
        var list = new List<Education>();
        if (!root.TryGetProperty("educations", out var educations)) return list;

        foreach (var edu in educations.EnumerateArray())
        {
            var startDate = DateTime.TryParse(
                edu.TryGetProperty("startDate", out var sd) ? sd.GetString() : null,
                out var parsedStart) ? parsedStart : DateTime.UtcNow.AddYears(-4);

            DateTime? endDate = null;
            if (edu.TryGetProperty("endDate", out var ed) && ed.ValueKind != JsonValueKind.Null)
                endDate = DateTime.TryParse(ed.GetString(), out var parsedEnd) ? parsedEnd : null;

            double? gpa = null;
            if (edu.TryGetProperty("gpa", out var g) && g.ValueKind != JsonValueKind.Null)
                gpa = g.TryGetDouble(out var parsedGpa) ? parsedGpa : null;

            list.Add(Education.Create(
                Guid.Empty,
                edu.TryGetProperty("institution", out var i) ? i.GetString() ?? "" : "",
                edu.TryGetProperty("degree", out var d) ? d.GetString() ?? "" : "",
                edu.TryGetProperty("field", out var f) ? f.GetString() ?? "" : "",
                startDate, endDate, gpa));
        }

        return list;
    }

    private static List<Skill> ParseSkills(JsonElement root)
    {
        var list = new List<Skill>();
        if (!root.TryGetProperty("skills", out var skills)) return list;

        foreach (var skill in skills.EnumerateArray())
        {
            var levelStr = skill.TryGetProperty("level", out var l) ? l.GetString() ?? "" : "";
            var level = Enum.TryParse<SkillLevel>(levelStr, true, out var parsedLevel)
                ? parsedLevel : SkillLevel.Intermediate;

            list.Add(new Skill
            {
                Name = skill.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "",
                Level = level,
                Category = skill.TryGetProperty("category", out var cat)
                    && cat.ValueKind != JsonValueKind.Null ? cat.GetString() : null
            });
        }

        return list;
    }

    private static List<string> ParseStringList(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var array)) return [];
        return array.EnumerateArray()
            .Select(x => x.GetString() ?? string.Empty)
            .Where(x => !string.IsNullOrEmpty(x))
            .ToList();
    }

    private static string GetString(JsonElement root, string parent, string child)
    {
        if (root.TryGetProperty(parent, out var parentEl)
            && parentEl.TryGetProperty(child, out var childEl))
            return childEl.GetString() ?? string.Empty;
        return string.Empty;
    }

    private static string? GetNullableString(JsonElement root, string parent, string child)
    {
        if (root.TryGetProperty(parent, out var parentEl)
            && parentEl.TryGetProperty(child, out var childEl)
            && childEl.ValueKind != JsonValueKind.Null)
            return childEl.GetString();
        return null;
    }

}