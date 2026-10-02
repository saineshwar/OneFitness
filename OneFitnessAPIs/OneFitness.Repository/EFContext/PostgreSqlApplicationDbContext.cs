using Microsoft.EntityFrameworkCore;

namespace OneFitness.Repository.EFContext
{
    public class PostgreSqlApplicationDbContext : ApplicationDbContext
    {
        public PostgreSqlApplicationDbContext(DbContextOptions<PostgreSqlApplicationDbContext> options) : base(options)
        {
        }
    }
}
