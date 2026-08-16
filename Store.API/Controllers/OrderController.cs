using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.DTOs.Order;
using Store.Application.Interfaces;

namespace Store.API.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
    [FromBody] CreateOrderRequest request)
    {
        var userId = GetUserId();

        var order = await _orderService.CreateOrderAsync(
            userId,
            request);

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = GetUserId();

        var orders = await _orderService.GetMyOrdersAsync(userId);

        return Ok(orders);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetOrder(int orderId)
    {
        var userId = GetUserId();

        var order = await _orderService.GetByIdAsync(
            orderId,
            userId);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
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
    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> CancelOrder(int orderId)
    {
        var userId = GetUserId();

        var order = await _orderService.CancelOrderAsync(
            orderId,
            userId);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}