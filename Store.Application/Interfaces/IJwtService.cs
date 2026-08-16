using Store.Domain.Entities;

namespace Store.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}