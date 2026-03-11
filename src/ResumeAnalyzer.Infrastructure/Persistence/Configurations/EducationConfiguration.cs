using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class EducationConfiguration : IEntityTypeConfiguration<Education>
{
    public void Configure(EntityTypeBuilder<Education> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Institution)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.Property(x => x.Degree)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Field)
            .IsRequired()
            .HasMaxLength(200);
    }
}