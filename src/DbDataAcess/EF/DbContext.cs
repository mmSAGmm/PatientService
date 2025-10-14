using Db.DataAccess.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Db.DataAccess.EF
{
    public class DbContex(IOptions<MySqlOption> mySqlOption) : DbContext
    {
        public DbSet<Models.Patient> Patients { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var version = new MySqlServerVersion(new Version(8, 0, 29));
            optionsBuilder.UseMySql(
                mySqlOption.Value.ConnectionString,
                version);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Patient>().ToTable("tbPatients");
        }
    }
}
