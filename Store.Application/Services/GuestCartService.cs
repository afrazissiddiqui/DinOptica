using System.Security.Cryptography;
using Store.Application.Interfaces;

namespace Store.Application.Services;

public class GuestCartService : IGuestCartService
{
    public string GenerateGuestCartId()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(32));
    }
}