using Microsoft.EntityFrameworkCore;

namespace OneFitness.Repository.EFContext
{
    public class SqlServerApplicationDbContext : ApplicationDbContext
    {
        public SqlServerApplicationDbContext(DbContextOptions<SqlServerApplicationDbContext> options) : base(options)
        {
        }
    }
}
