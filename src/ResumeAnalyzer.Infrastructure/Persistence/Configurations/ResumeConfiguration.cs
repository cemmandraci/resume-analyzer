using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ResumeAnalyzer.Domain.Entities;

namespace ResumeAnalyzer.Infrastructure.Persistence.Configurations;

public class ResumeConfiguration : IEntityTypeConfiguration<Resume>
{
    public void Configure(EntityTypeBuilder<Resume> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.RawText)
            .IsRequired();
        
        builder.Property(x => x.AiProvider)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);
        
        builder.HasOne(x => x.Analysis)
            .WithOne()
            .HasForeignKey<ResumeAnalysis>(x => x.ResumeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}