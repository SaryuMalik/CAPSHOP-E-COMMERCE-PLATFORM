using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace CapShop.AdminService.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly HttpClient _catalogClient;
    private readonly HttpClient _orderClient;

    public AdminController(IHttpClientFactory httpClientFactory)
    {
        _catalogClient = httpClientFactory.CreateClient("CatalogService");
        _orderClient = httpClientFactory.CreateClient("OrderService");
    }

    // ✅ Token forward karne ka method
    private void ForwardToken(HttpClient client)
    {
        var token = Request.Headers["Authorization"].ToString();
        client.DefaultRequestHeaders.Remove("Authorization");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Add("Authorization", token);
    }

    // ── Products ──────────────────────────────────────────

   [HttpGet("products")]
public async Task<IActionResult> GetAllProducts()
{
    ForwardToken(_catalogClient);
    var response = await _catalogClient.GetAsync("/api/products?page=1&pageSize=1000"); // ✅ sab products
    var data = await response.Content.ReadAsStringAsync();
    return Content(data, "application/json");
}

    [HttpPost("products")]
    public async Task<IActionResult> CreateProduct([FromBody] object dto)
    {
        ForwardToken(_catalogClient);
        var response = await _catalogClient.PostAsJsonAsync("/api/products", dto);
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    [HttpPut("products/{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] object dto)
    {
        ForwardToken(_catalogClient);
        var response = await _catalogClient.PutAsJsonAsync($"/api/products/{id}", dto);
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    [HttpDelete("products/{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        ForwardToken(_catalogClient);
        var response = await _catalogClient.DeleteAsync($"/api/products/{id}");
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    // ── Orders ───────────────────────────────────────────

    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders()
    {
        ForwardToken(_orderClient);
        var response = await _orderClient.GetAsync("/api/orders/all");
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    [HttpPut("orders/{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] object dto)
    {
        ForwardToken(_orderClient);
        var response = await _orderClient.PutAsJsonAsync($"/api/orders/{id}/status", dto);
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }

    // ── Categories ───────────────────────────────────────

    [HttpGet("categories")]
    public async Task<IActionResult> GetCategories()
    {
        ForwardToken(_catalogClient);
        var response = await _catalogClient.GetAsync("/api/categories");
        var data = await response.Content.ReadAsStringAsync();
        return Content(data, "application/json");
    }
}