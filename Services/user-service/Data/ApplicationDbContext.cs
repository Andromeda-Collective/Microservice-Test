using Microsoft.EntityFrameworkCore;
using User_Service.AuthEntities;
using User_Service.Entities;

namespace User_Service.Data;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }


    public DbSet<User> Users => Set<User>();
    public DbSet<Token> Tokens => Set<Token>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.LastName)
            .HasMaxLength(50)
            .IsRequired();


        modelBuilder.Entity<Token>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Token>()
            .HasIndex(t => t.UserId)
            .IsUnique();

        modelBuilder.Entity<Token>()
            .Property(t => t.UserId)
            .IsRequired();

        modelBuilder.Entity<Token>()
            .Property(t => t.HashRefreshToken)
            .IsRequired();

        modelBuilder.Entity<Token>()
            .Property(t => t.ExpireTime)
            .IsRequired();

        modelBuilder.Entity<Token>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tokens)
            .HasForeignKey(t => t.UserId);

    }

}