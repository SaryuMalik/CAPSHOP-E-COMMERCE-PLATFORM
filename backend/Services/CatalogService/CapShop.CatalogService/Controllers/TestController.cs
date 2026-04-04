using Microsoft.AspNetCore.Mvc;

namespace CapShop.CatalogService.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new { message = "Working!" });

    [HttpPost]
    public IActionResult Post([FromBody] object data) => Ok(new { message = "POST Working!", data });
}