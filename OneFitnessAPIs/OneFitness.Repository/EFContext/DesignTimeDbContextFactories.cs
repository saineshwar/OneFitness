using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace OneFitness.Repository.EFContext
{
    // Used only by `dotnet ef migrations add/update` at design time, so each provider
    // gets its own migrations history without needing the API host to be runnable.
    public class SqlServerApplicationDbContextFactory : IDesignTimeDbContextFactory<SqlServerApplicationDbContext>
    {
        public SqlServerApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ONEFITNESS_SQLSERVER_CONNECTION")
                ?? "Server=localhost;Database=OneFitnessDb;Trusted_Connection=True;TrustServerCertificate=True;";

            var optionsBuilder = new DbContextOptionsBuilder<SqlServerApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sql => sql.MigrationsAssembly("OneFitness.Repository"));

            return new SqlServerApplicationDbContext(optionsBuilder.Options);
        }
    }

    public class PostgreSqlApplicationDbContextFactory : IDesignTimeDbContextFactory<PostgreSqlApplicationDbContext>
    {
        public PostgreSqlApplicationDbContext CreateDbContext(string[] args)
        {
            var connectionString = Environment.GetEnvironmentVariable("ONEFITNESS_POSTGRESQL_CONNECTION")
                ?? "Host=localhost;Port=5432;Database=onefitnessdb;Username=postgres;Password=postgres;";

            var optionsBuilder = new DbContextOptionsBuilder<PostgreSqlApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly("OneFitness.Repository"));

            return new PostgreSqlApplicationDbContext(optionsBuilder.Options);
        }
    }
}
