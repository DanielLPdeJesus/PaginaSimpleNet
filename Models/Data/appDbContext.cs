using Microsoft.EntityFrameworkCore;
using Proyectonext.Models;
namespace Proyectonext.Models.Data
{
    public class appDbContext : DbContext
    {
        public appDbContext(DbContextOptions<appDbContext> options) : base(options) { }

        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Users>( tb => { tb.HasKey( col => col.IdUser);
                tb.Property(col => col.IdUser).UseIdentityColumn().ValueGeneratedOnAdd();
                tb.Property(col => col.UserName).HasMaxLength(50);
                tb.Property(col => col.Password).HasMaxLength(50);
                tb.Property(col => col.Mail).HasMaxLength(50);

            }
                
                );
            modelBuilder.Entity<Users>().ToTable("Users");
        }
    }
}
