using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Entities;
using Store.Domain.Enums;

namespace Store.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(
        StoreDbContext context,
        IPasswordHasher passwordHasher)
    {
        var adminExists = await context.Users
            .AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists)
        {
            return;
        }

        var admin = new User
        {
            FirstName = "Store",
            LastName = "Admin",
            Email = "admin@store.com",
            PasswordHash = passwordHasher.Hash("Admin@12345"),
            PhoneNumber = "0000000000",
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();
    }
}