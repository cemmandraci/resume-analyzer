using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeAnalyzer.Domain.Entities;
using ResumeAnalyzer.Domain.ValueObjects;

namespace ResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class ResumeAnalysisConfiguration : IEntityTypeConfiguration<ResumeAnalysis>
{
    public void Configure(EntityTypeBuilder<ResumeAnalysis> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.PersonelInfo, y =>
        {
            y.Property(z => z.FullName).HasMaxLength(200);
            y.Property(z => z.Email).HasMaxLength(200);
            y.Property(z => z.Phone).HasMaxLength(50);
            y.Property(z => z.Location).HasMaxLength(200);
            y.Property(z => z.LinkedInUrl).HasMaxLength(500);
            y.Property(z => z.GitHubUrl).HasMaxLength(500);
            y.Property(z => z.Summary).HasMaxLength(2000);
        });
        
        builder.Property<List<Skill>>("_skills")
            .HasColumnName("Skills")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<Skill>>(y, (JsonSerializerOptions?) null) ?? new List<Skill>())
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property<List<string>>("_strengths")
            .HasColumnName("Strengths")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?) null) ?? new List<string>())
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property<List<string>>("_weaknesses")
            .HasColumnName("Weaknesses")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?) null) ?? new List<string>())
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property<List<string>>("_suggestions")
            .HasColumnName("Suggestions")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?) null) ?? new List<string>())
            .UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.HasMany(x => x.WorkExperiences)
            .WithOne()
            .HasForeignKey(x => x.ResumeAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Educations)
            .WithOne()
            .HasForeignKey(x => x.ResumeAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.JobMatches)
            .WithOne()
            .HasForeignKey(x => x.ResumeAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}