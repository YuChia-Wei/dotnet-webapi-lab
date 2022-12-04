using dotnet.WebApi.Repository.Db.SampleDb.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dotnet.WebApi.Repository.Db.SampleDb.Configurations;

public class SampleDataConfiguration : IEntityTypeConfiguration<SampleData>
{
    /// <summary>
    ///     Configures the entity of type <typeparamref name="TEntity" />.
    /// </summary>
    /// <param name="entity">The entity to be used to configure the entity type.</param>
    public void Configure(EntityTypeBuilder<SampleData> entity)
    {
        entity.HasKey(e => e.SerialId);

        entity.ToTable("SampleData", "dbo");

        entity.HasComment("範例資料");

        entity.Property(e => e.SerialId)
              .HasComment("序號");

        entity.Property(e => e.Description)
              .HasMaxLength(100)
              .IsUnicode()
              .HasComment("敘述");

        entity.Property(e => e.DocumentNum)
              .IsRequired()
              .HasComment("文件編號");
    }
}