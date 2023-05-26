#nullable disable

using dotnet.WebApi.Database.SampleDb.Configurations;
using dotnet.WebApi.Database.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnet.WebApi.Database.SampleDb;

public class SampleDbContext : DbContext
{
    public SampleDbContext()
    {
    }

    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SampleTable> SampleTables { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

        modelBuilder.ApplyConfiguration(new SampleTableConfiguration());
    }
}