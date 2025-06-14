namespace web_app.Data;

using Microsoft.EntityFrameworkCore;
using web_app.Models;
using BCrypt.Net;


public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            Email = "admin@example.com",
            Password = "$2a$11$O1QRTvfHNN/VOKTiWLsOcuJhqX.tvV4QGP9vnCStpHSFqfYi.a1Ci",
            CreatedAt = new DateTime(2025, 6, 12, 2, 19, 21, 180, DateTimeKind.Utc).AddTicks(4552),
            IsDeleted = false
        });
    }
}
