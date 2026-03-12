using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
{
    public void Configure(EntityTypeBuilder<WorkExperience> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Company)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(x => x.Position)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .IsRequired(false)
            .HasMaxLength(2000);

        builder.Property(x => x.EndDate)
            .IsRequired(false);
        
        builder.Property<List<string>>("_responsibilities")
            .HasColumnName("Responsibilities")
            .HasConversion(
                y => JsonSerializer.Serialize(y, (JsonSerializerOptions?)null),
                y => JsonSerializer.Deserialize<List<string>>(y, (JsonSerializerOptions?)null) ?? new List<string>()
                ).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}