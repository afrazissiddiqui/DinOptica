using Microsoft.AspNetCore.Identity;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password)
    {
        var user = new User();

        return _hasher.HashPassword(user, password);
    }

    public bool Verify(string password, string passwordHash)
    {
        var user = new User();

        var result = _hasher.VerifyHashedPassword(
            user,
            passwordHash,
            password);

        return result == PasswordVerificationResult.Success ||
               result == PasswordVerificationResult.SuccessRehashNeeded;
    }
}