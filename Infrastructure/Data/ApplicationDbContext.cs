using Microsoft.EntityFrameworkCore;
using SDTechnicalAssessment.Domain.Entities;

namespace SDTechnicalAssessment.Infrastructure.Data
{
    // ApplicationDbContext is the main bridge between our C# application
    // and the SQL Server database.
    //
    // Entity Framework Core uses this class to:
    // 1. Connect to SQL Server
    // 2. Read data
    // 3. Insert data
    // 4. Update data
    // 5. Delete data
    // 6. Create database tables through migrations
    public class ApplicationDbContext : DbContext
    {
        // Constructor receives database configuration from dependency injection.
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Represents the Product table in SQL Server.
        public DbSet<Product> Products { get; set; }

        // Represents the Item table in SQL Server.
        public DbSet<Item> Items { get; set; }


        // ------------------------------------------------------------
        // Represents the User table in SQL Server.
        // ------------------------------------------------------------
        public DbSet<User> Users { get; set; }



        public DbSet<RefreshToken> RefreshTokens { get; set; }

        // This method is used for configuring relationships and database rules.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Call the base implementation first.
            base.OnModelCreating(modelBuilder);

            // Configure Product table.
            modelBuilder.Entity<Product>(entity =>
            {
                // Id is the primary key.
                entity.HasKey(p => p.Id);

                // ProductName is required and maximum length is 255.
                entity.Property(p => p.ProductName)
                      .IsRequired()
                      .HasMaxLength(255);

                // CreatedBy is required and maximum length is 100.
                entity.Property(p => p.CreatedBy)
                      .IsRequired()
                      .HasMaxLength(100);

                // CreatedOn is required.
                entity.Property(p => p.CreatedOn)
                      .IsRequired();

                // ModifiedBy is optional and maximum length is 100.
                entity.Property(p => p.ModifiedBy)
                      .HasMaxLength(100);

                // Configure Product -> Item relationship.
                // One Product can have many Items.
                entity.HasMany(p => p.Items)
                      .WithOne(i => i.Product)
                      .HasForeignKey(i => i.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Item table.
            modelBuilder.Entity<Item>(entity =>
            {
                // Id is the primary key.
                entity.HasKey(i => i.Id);

                // Quantity is required.
                entity.Property(i => i.Quantity)
                      .IsRequired();
            });

            // ------------------------------------------------------------
            // Configure User table.
            // ------------------------------------------------------------
            modelBuilder.Entity<User>(entity =>
            {
                // Id is the primary key.
                entity.HasKey(u => u.Id);

                // Username is required.
                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(100);

                // Password is required.
                entity.Property(u => u.Password)
                      .IsRequired()
                      .HasMaxLength(255);

                // Role is required.
                entity.Property(u => u.Role)
                      .IsRequired()
                      .HasMaxLength(50);

                // Username should be unique.
                entity.HasIndex(u => u.Username)
                      .IsUnique();
            });

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                // Id is the primary key.
                entity.HasKey(r => r.Id);

                // Token is required and cannot exceed 500 characters.
                entity.Property(r => r.Token)
                      .IsRequired()
                      .HasMaxLength(500);

                // Create an index on Token so token lookup is faster.
                entity.HasIndex(r => r.Token)
                      .IsUnique();

                // Configure the relationship:
                // One User can have many RefreshTokens.
                entity.HasOne(r => r.User)
                      .WithMany(u => u.RefreshTokens)
                      .HasForeignKey(r => r.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // These fields are required.
                entity.Property(r => r.ExpiresOn)
                      .IsRequired();

                entity.Property(r => r.CreatedOn)
                      .IsRequired();
            });


        }
    }
}