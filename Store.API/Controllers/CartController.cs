using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs.Cart;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();

        var cart = await _cartService.GetOrCreateCartAsync(userId);

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        AddToCartRequest request)
    {
        var userId = GetUserId();

        var cart = await _cartService.AddItemAsync(
            userId,
            request);

        return Ok(cart);
    }

    [HttpPut("items/{productId:int}")]
    public async Task<IActionResult> UpdateItem(
    int productId,
    UpdateCartItemRequest request)
    {
        var userId = GetUserId();

        var cart = await _cartService.UpdateItemAsync(
            userId,
            productId,
            request);

        return Ok(cart);
    }

    [HttpDelete("items/{productId:int}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();

        var cart = await _cartService.RemoveItemAsync(
            userId,
            productId);

        return Ok(cart);
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException(
                "User ID could not be determined from the token.");
        }

        return userId;
    }
}