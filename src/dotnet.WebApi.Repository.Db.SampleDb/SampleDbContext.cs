#nullable disable

using dotnet.WebApi.Repository.Db.SampleDb.Configurations;
using dotnet.WebApi.Repository.Db.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;

namespace dotnet.WebApi.Repository.Db.SampleDb;

public class SampleDbContext : DbContext
{
    public SampleDbContext()
    {
    }

    public SampleDbContext(DbContextOptions<SampleDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SampleData> SampleDatas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

        modelBuilder.ApplyConfiguration(new SampleDataConfiguration());
    }
}