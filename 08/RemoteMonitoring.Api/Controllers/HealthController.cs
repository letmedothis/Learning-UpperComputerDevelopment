using Microsoft.AspNetCore.Mvc;

namespace RemoteMonitoring.Api.Controllers;

/// <summary>
/// 健康检查 API。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    /// <summary>
    /// 检查 API 是否正常运行。
    /// GET /api/health
    /// </summary>
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            status = "healthy",
            timestamp = DateTime.Now,
            version = "1.0.0"
        });
    }
}
