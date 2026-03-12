using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class JobMatchConfiguration : IEntityTypeConfiguration<JobMatch>
{
    public void Configure(EntityTypeBuilder<JobMatch> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.JobTitle)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.JobDescription)
            .IsRequired();

        builder.Property(x => x.AIFeedBack)
            .IsRequired(false)
            .HasMaxLength(5000);
        
        builder.Property<List<string>>("_matchingSkills")
            .HasColumnName("MatchingSkills")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?)null) ?? new List<string>()
            ).UsePropertyAccessMode(PropertyAccessMode.Field);
        
        builder.Property<List<string>>("_missingSkills")
            .HasColumnName("MissingSkills")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?)null) ?? new List<string>()
            ).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}