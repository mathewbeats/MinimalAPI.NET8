using Microsoft.EntityFrameworkCore;
using MinimalApiDatabase.Models;
namespace MinimalApiDatabase

{
    public class ApplicationDbContext: DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Order> Orders { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(o => o.Id);
                entity.Property(u => u.FirstName).IsRequired().HasMaxLength(128);
                entity.Property(u => u.LastName).IsRequired().HasMaxLength(128);
                entity.Property(u => u.Email).IsRequired().HasMaxLength(128);

                entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);


            });

            modelBuilder.Entity<Order>(entity =>
            {

                entity.ToTable("Orders");
                entity.HasKey(o => o.Id);
                entity.Property(u => u.OrderDate).IsRequired().HasMaxLength(128);
                entity.HasOne(u => u.User)
                .WithMany(u => u.Orders).HasForeignKey(u => u.UserId);
            });
        }
    }
}
