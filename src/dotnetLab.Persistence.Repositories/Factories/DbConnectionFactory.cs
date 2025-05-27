using System.Data;
using dotnetLab.Persistence.Repositories.Enums;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace dotnetLab.Persistence.Repositories.Factories;

/// <summary>
/// db connection factory
/// </summary>
internal class DbConnectionFactory : IDbConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DbConnectionFactory(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public IDbConnection CreateConnection(DbInstanceEnum dbInstanceEnum)
    {
        switch (dbInstanceEnum)
        {
            case DbInstanceEnum.SamplePostgreSQL:
                return new NpgsqlConnection(this._configuration.GetConnectionString(dbInstanceEnum.ToString()));

            default:
                throw new ArgumentOutOfRangeException(nameof(dbInstanceEnum), dbInstanceEnum, null);
        }
    }
}