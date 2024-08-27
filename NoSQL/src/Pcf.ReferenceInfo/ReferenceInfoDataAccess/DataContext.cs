using Microsoft.EntityFrameworkCore;
using ReferenceInfoCore.Domain;


namespace ReferenceInfoDataAccess
{
    public class DataContext
        : DbContext
    {

        public DbSet<Preference> Partners { get; set; }

        public DataContext()
        {

        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}