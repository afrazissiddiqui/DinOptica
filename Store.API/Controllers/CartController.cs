using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs.Cart;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly IGuestCartService _guestCartService;

    private const string GuestCartCookie = "GuestCartId";

    public CartController(
        ICartService cartService,
        IGuestCartService guestCartService)
    {
        _cartService = cartService;
        _guestCartService = guestCartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = GetUserId();

            var cart = await _cartService.GetOrCreateCartAsync(userId);

            return Ok(cart);
        }

        var guestCartId = GetOrCreateGuestCartId();

        var guestCart =
            await _cartService.GetOrCreateGuestCartAsync(guestCartId);

        return Ok(guestCart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        AddToCartRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = GetUserId();

            var cart = await _cartService.AddItemAsync(
                userId,
                request);

            return Ok(cart);
        }

        var guestCartId = GetOrCreateGuestCartId();

        var guestCart = await _cartService.AddGuestItemAsync(
            guestCartId,
            request);

        return Ok(guestCart);
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(
        int productId,
        UpdateCartItemRequest request)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = GetUserId();

            var cart = await _cartService.UpdateItemAsync(
                userId,
                productId,
                request);

            return Ok(cart);
        }

        var guestCartId = GetOrCreateGuestCartId();

        var guestCart = await _cartService.UpdateGuestItemAsync(
            guestCartId,
            productId,
            request);

        return Ok(guestCart);
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            var userId = GetUserId();

            var cart = await _cartService.RemoveItemAsync(
                userId,
                productId);

            return Ok(cart);
        }

        var guestCartId = GetOrCreateGuestCartId();

        var guestCart = await _cartService.RemoveGuestItemAsync(
            guestCartId,
            productId);

        return Ok(guestCart);
    }

    private string GetOrCreateGuestCartId()
    {
        if (Request.Cookies.TryGetValue(
            GuestCartCookie,
            out var existingGuestCartId)
            && !string.IsNullOrWhiteSpace(existingGuestCartId))
        {
            return existingGuestCartId;
        }

        var guestCartId =
            _guestCartService.GenerateGuestCartId();

        Response.Cookies.Append(
            GuestCartCookie,
            guestCartId,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

        return guestCartId;
    }

    private int GetUserId()
    {
        var userIdClaim =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "User ID could not be determined from the token.");
        }

        return userId;
    }
}