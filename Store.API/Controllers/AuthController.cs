using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs.Auth;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ICartService _cartService;

    private const string GuestCartCookie = "GuestCartId";

    public AuthController(
        IAuthService authService,
        ICartService cartService)
    {
        _authService = authService;
        _cartService = cartService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (Request.Cookies.TryGetValue(
            GuestCartCookie,
            out var guestCartId)
            && !string.IsNullOrWhiteSpace(guestCartId))
        {
            await _cartService.MergeGuestCartAsync(
                guestCartId,
                response.UserId);

            Response.Cookies.Delete(GuestCartCookie);
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            message = "You are authenticated.",
            userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        });
    }
    [Authorize(Roles = "Admin")]
    [HttpGet("admin-test")]
    public IActionResult AdminTest()
    {
        return Ok(new
        {
            message = "You are an Admin.",
            userId = User.FindFirst(
                System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            role = User.FindFirst(
                System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }
}