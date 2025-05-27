#nullable disable

using dotnetLab.Persistence.Metadata.SampleDb.Configurations;
using dotnetLab.Persistence.Metadata.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetLab.Persistence.Metadata.SampleDb;

public class SampleDbContext : DbContext
{
    public SampleDbContext()
    {
    }

    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SimpleDocument> SimpleDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

        modelBuilder.ApplyConfiguration(new SimpleDocumentConfiguration());
    }
}