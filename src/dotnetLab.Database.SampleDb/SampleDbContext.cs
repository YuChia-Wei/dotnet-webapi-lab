#nullable disable

using dotnetLab.Database.SampleDb.Configurations;
using dotnetLab.Database.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnetLab.Database.SampleDb;

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