using Microsoft.AspNetCore.Mvc;
using RemoteMonitoring.Core.Services;

namespace RemoteMonitoring.Api.Controllers;

/// <summary>
/// 设备管理 API。
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DevicesController : ControllerBase
{
    private readonly IDeviceStateService _stateService;

    public DevicesController(IDeviceStateService stateService)
    {
        _stateService = stateService;
    }

    /// <summary>
    /// 获取所有设备列表。
    /// GET /api/devices
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllDevices(CancellationToken cancellationToken)
    {
        var devices = await _stateService.GetAllDevicesAsync(cancellationToken);
        return Ok(devices);
    }

    /// <summary>
    /// 获取指定设备的当前状态。
    /// GET /api/devices/{deviceId}/state
    /// </summary>
    [HttpGet("{deviceId}/state")]
    public async Task<IActionResult> GetDeviceState(string deviceId, CancellationToken cancellationToken)
    {
        var state = await _stateService.GetCurrentStateAsync(deviceId, cancellationToken);
        if (state == null)
            return NotFound(new { error = $"设备 {deviceId} 不存在" });

        return Ok(state);
    }

    /// <summary>
    /// 获取指定设备的历史状态。
    /// GET /api/devices/{deviceId}/history?count=50
    /// </summary>
    [HttpGet("{deviceId}/history")]
    public async Task<IActionResult> GetDeviceHistory(
        string deviceId,
        [FromQuery] int count = 50,
        CancellationToken cancellationToken = default)
    {
        var history = await _stateService.GetStateHistoryAsync(deviceId, count, cancellationToken);
        return Ok(history);
    }
}
