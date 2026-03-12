# AI-Powered Resume Analyzer

A production-ready resume analysis system built with .NET 10, PostgreSQL, and multiple AI providers. Upload a PDF resume and get structured analysis including personal information, work experience, education, skills, strengths, weaknesses, and improvement suggestions. Match resumes against job descriptions to get compatibility scores.

## How It Works

```
Resume PDF uploaded
  → PDF parsed with PdfPig (text extraction)
  → AI provider selected (Gemini or Claude)
  → Resume text analyzed by LLM
  → Structured data extracted:
      → Personal information
      → Work experiences
      → Education history
      → Skills with levels
      → Strengths & weaknesses
      → Improvement suggestions
  → Results saved to PostgreSQL
  → Optional: Match against job description → compatibility score
```

## Architecture

Clean Architecture with 4 layers and key design patterns:

```
resume-analyzer/
├── docker-compose.yml
├── NuGet.config
├── .env
└── src/
    ├── ResumeAnalyzer.Domain/
    │   ├── Entities/
    │   │   ├── Resume.cs               # File metadata + analysis status
    │   │   ├── ResumeAnalysis.cs       # AI-generated analysis result
    │   │   ├── WorkExperience.cs       # Work history entries
    │   │   ├── Education.cs            # Education history entries
    │   │   └── JobMatch.cs             # Job description match result
    │   ├── ValueObjects/
    │   │   ├── PersonalInfo.cs         # Immutable personal data record
    │   │   └── Skill.cs                # Skill with level and category
    │   ├── Interfaces/
    │   │   ├── IResumeRepository.cs
    │   │   ├── IResumeAnalysisService.cs   # Strategy Pattern interface
    │   │   ├── IPdfParserService.cs
    │   │   └── IAIServiceFactory.cs        # Factory Pattern interface
    │   └── Enums/
    │       ├── AnalysisStatus.cs
    │       └── SkillLevel.cs
    ├── ResumeAnalyzer.Application/
    │   ├── Commands/
    │   │   ├── AnalyzeResume/          # PDF upload + AI analysis pipeline
    │   │   └── MatchJob/               # Job description matching
    │   ├── Queries/
    │   │   ├── GetResume/              # Get resume with full analysis
    │   │   └── GetAllResumes/          # List all resumes
    │   ├── DTOs/
    │   │   ├── ResumeDto.cs
    │   │   ├── ResumeAnalysisDto.cs
    │   │   └── JobMatchDto.cs
    │   └── Exceptions/
    │       └── ResumeNotFoundException.cs
    ├── ResumeAnalyzer.Infrastructure/
    │   ├── AI/
    │   │   ├── Base/
    │   │   │   └── BaseResumeAnalysisService.cs   # Shared parse logic
    │   │   ├── Gemini/
    │   │   │   ├── GeminiResumeAnalysisService.cs
    │   │   │   └── GeminiModels.cs
    │   │   ├── Claude/
    │   │   │   ├── ClaudeResumeAnalysisService.cs
    │   │   │   └── ClaudeModels.cs
    │   │   └── Factory/
    │   │       └── AIServiceFactory.cs
    │   ├── Persistence/
    │   │   ├── ResumeDbContext.cs
    │   │   ├── Configurations/
    │   │   └── Repositories/
    │   │       └── ResumeRepository.cs
    │   └── Services/
    │       └── PdfParserService.cs
    └── ResumeAnalyzer.API/
        ├── Controllers/
        │   └── ResumesController.cs
        ├── Middleware/
        │   └── ExceptionHandlingMiddleware.cs
        └── Program.cs
```

## Design Patterns

### Strategy Pattern
Each AI provider implements the same `IResumeAnalysisService` interface. The calling code never knows which provider is being used — it only works with the interface.

```
IResumeAnalysisService
    ├── GeminiResumeAnalysisService   → Google Gemini 2.5 Flash
    └── ClaudeResumeAnalysisService   → Anthropic Claude Sonnet
```

Adding a new provider (e.g. OpenAI) requires only:
1. Creating `OpenAiResumeAnalysisService : BaseResumeAnalysisService`
2. Implementing `SendRequestAsync` with OpenAI's HTTP format
3. Registering it in `AIServiceFactory`

### Factory Pattern
`AIServiceFactory` selects the correct strategy at runtime based on the `provider` query parameter:

```csharp
return provider.ToLower() switch
{
    "gemini" => _geminiService,
    "claude" => _claudeService,
    _ => throw new NotSupportedException(...)
};
```

### Template Method Pattern
`BaseResumeAnalysisService` defines the algorithm skeleton. Each provider only overrides the HTTP communication part — all JSON parsing logic is shared:

```
BaseResumeAnalysisService (abstract)
    ├── AnalyzeAsync()          ← calls SendRequestAsync + ParseAnalysisResult
    ├── MatchWithJobAsync()     ← calls SendRequestAsync + ParseJobMatchResult
    ├── ParseAnalysisResult()   ← shared JSON parsing logic
    ├── BuildAnalysisPrompt()   ← shared prompt (virtual, can be overridden)
    └── SendRequestAsync()      ← abstract, each provider implements its own HTTP call
```

## Tech Stack

| Component | Technology |
|---|---|
| API | .NET 10 Web API |
| Architecture | Clean Architecture + CQRS |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core + Npgsql |
| PDF Parsing | PdfPig |
| AI Provider 1 | Google Gemini 2.5 Flash |
| AI Provider 2 | Anthropic Claude Sonnet |
| Mediator | MediatR 11.x |
| Containerization | Docker + Docker Compose |

## Getting Started

### Prerequisites

- Docker & Docker Compose
- Gemini API Key → [Get here](https://aistudio.google.com/)
- Claude API Key → [Get here](https://console.anthropic.com/)

### Setup

1. Clone the repository:
```bash
git clone https://github.com/yourusername/resume-analyzer
cd resume-analyzer
```

2. Create `.env` file in the root directory:
```
GEMINI_API_KEY=your_gemini_api_key_here
CLAUDE_API_KEY=your_claude_api_key_here
```

3. Start all services:
```bash
docker-compose up --build
```

4. Open Swagger UI:
```
http://localhost:5000/swagger
```

5. PostgreSQL connection (for Rider/DataGrip/pgAdmin):
```
jdbc:postgresql://localhost:5432/resumeanalyzer?user=postgres&password=postgres
```

## API Endpoints

### Analyze a Resume
```http
POST /api/resumes/analyze?provider=Gemini
Content-Type: multipart/form-data

file: resume.pdf
```

Response:
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "fileName": "resume.pdf",
  "status": "Completed",
  "aiProvider": "Gemini",
  "analysis": {
    "personalInfo": {
      "fullName": "John Doe",
      "email": "john@example.com",
      "phone": "+1 555 000 0000",
      "location": "San Francisco, CA",
      "linkedInUrl": "https://linkedin.com/in/johndoe",
      "gitHubUrl": "https://github.com/johndoe"
    },
    "workExperiences": [...],
    "educations": [...],
    "skills": [
      { "name": "C#", "level": "Expert", "category": "Programming" },
      { "name": "Docker", "level": "Advanced", "category": "DevOps" }
    ],
    "strengths": ["Strong backend architecture skills", "..."],
    "weaknesses": ["Limited frontend experience", "..."],
    "suggestions": ["Consider learning React", "..."]
  }
}
```

### Match Resume with Job Description
```http
POST /api/resumes/{id}/match
Content-Type: application/json

{
  "jobTitle": "Senior Backend Developer",
  "jobDescription": "We are looking for a .NET developer with microservices experience..."
}
```

Response:
```json
{
  "jobTitle": "Senior Backend Developer",
  "matchScore": 87,
  "matchingSkills": ["C#", "Docker", "Kubernetes", "PostgreSQL"],
  "missingSkills": ["React", "GraphQL"],
  "aiFeedback": "Strong candidate with excellent backend skills..."
}
```

### Get Resume by ID
```http
GET /api/resumes/{id}
```

### Get All Resumes
```http
GET /api/resumes
```

### Get Available AI Providers
```http
GET /api/resumes/providers
```

Response: `["Gemini", "Claude"]`

## Switching AI Providers

Pass the `provider` query parameter when analyzing:

```bash
# Use Gemini
POST /api/resumes/analyze?provider=Gemini

# Use Claude
POST /api/resumes/analyze?provider=Claude
```

The same resume analyzed by different providers may yield different insights — try both and compare results.

## Services

| Service | URL | Description |
|---|---|---|
| Resume API | http://localhost:5000 | .NET Web API |
| Swagger UI | http://localhost:5000/swagger | API documentation |
| PostgreSQL | localhost:5432 | Database |

## Key Technical Decisions

**Why PdfPig?** The best free, open-source PDF text extraction library for .NET. iTextSharp and Aspose require commercial licenses.

**Why Strategy + Factory Pattern?** Adding a new AI provider requires zero changes to existing code. New provider → new class → register in factory. Open/Closed Principle in practice.

**Why EF Core Fluent Configuration?** Complex relationships (owned entities, JSON columns, cascade deletes) require explicit configuration. Value objects like `PersonalInfo` are stored as owned entities (same table, no join needed). Simple lists like `Skills` and `Strengths` are stored as JSON columns for efficiency.

**Why separate `AddAnalysisAsync`?** EF Core's `Update()` tries to UPDATE all tracked entities. Since `ResumeAnalysis`, `WorkExperiences`, and `Educations` are new (never INSERT-ed), they must be explicitly added before updating the parent `Resume`.
