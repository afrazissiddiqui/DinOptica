using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Application.Interfaces;
using Store.Domain.Entities;

namespace Store.API.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _adminService.GetAllUsersAsync();

        return Ok(users);
    }
    [HttpGet("orders")]
    public async Task<IActionResult> GetOrders()
    {
        var orders = await _adminService.GetAllOrdersAsync();

        return Ok(orders);
    }
    [HttpPut("orders/{orderId:int}/status")]
    public async Task<IActionResult> UpdateOrderStatus(
    int orderId,
    [FromBody] OrderStatus status)
    {
        var order = await _adminService.UpdateOrderStatusAsync(
            orderId,
            status);

        if (order is null)
        {
            return NotFound();
        }

        return Ok(order);
    }
}