using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ResumeAnalyzer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FileName = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    RawText = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FailureReason = table.Column<string>(type: "text", nullable: true),
                    AiProvider = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ResumeAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeId = table.Column<Guid>(type: "uuid", nullable: false),
                    PersonelInfo_FullName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PersonelInfo_Email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PersonelInfo_Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    PersonelInfo_Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    PersonelInfo_LinkedInUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PersonelInfo_GitHubUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PersonelInfo_Summary = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Skills = table.Column<string>(type: "text", nullable: false),
                    Strengths = table.Column<string>(type: "text", nullable: false),
                    Suggestions = table.Column<string>(type: "text", nullable: false),
                    Weaknesses = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumeAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ResumeAnalyses_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Institution = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    Degree = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Field = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    GPA = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educations_ResumeAnalyses_ResumeAnalysisId",
                        column: x => x.ResumeAnalysisId,
                        principalTable: "ResumeAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "JobMatches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    JobDescription = table.Column<string>(type: "text", nullable: false),
                    MatchScore = table.Column<int>(type: "integer", nullable: false),
                    AIFeedBack = table.Column<string>(type: "character varying(5000)", maxLength: 5000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MatchingSkills = table.Column<string>(type: "text", nullable: false),
                    MissingSkills = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobMatches_ResumeAnalyses_ResumeAnalysisId",
                        column: x => x.ResumeAnalysisId,
                        principalTable: "ResumeAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkExperiences",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    Company = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Position = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Responsibilities = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkExperiences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkExperiences_ResumeAnalyses_ResumeAnalysisId",
                        column: x => x.ResumeAnalysisId,
                        principalTable: "ResumeAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Educations_ResumeAnalysisId",
                table: "Educations",
                column: "ResumeAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_JobMatches_ResumeAnalysisId",
                table: "JobMatches",
                column: "ResumeAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_ResumeAnalyses_ResumeId",
                table: "ResumeAnalyses",
                column: "ResumeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkExperiences_ResumeAnalysisId",
                table: "WorkExperiences",
                column: "ResumeAnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "JobMatches");

            migrationBuilder.DropTable(
                name: "WorkExperiences");

            migrationBuilder.DropTable(
                name: "ResumeAnalyses");

            migrationBuilder.DropTable(
                name: "Resumes");
        }
    }
}
