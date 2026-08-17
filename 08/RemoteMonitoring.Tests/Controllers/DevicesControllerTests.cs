using Microsoft.AspNetCore.Mvc;
using RemoteMonitoring.Api.Controllers;
using RemoteMonitoring.Core.Services;

namespace RemoteMonitoring.Tests.Controllers;

public sealed class DevicesControllerTests
{
    private readonly DevicesController _controller;
    private readonly SimulatedDeviceStateService _service;

    public DevicesControllerTests()
    {
        _service = new SimulatedDeviceStateService();
        _controller = new DevicesController(_service);
    }

    [Fact]
    public async Task GetAllDevices_ReturnsOkWithDevices()
    {
        var result = await _controller.GetAllDevices(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var devices = Assert.IsAssignableFrom<IReadOnlyList<RemoteMonitoring.Core.Models.DeviceSummary>>(okResult.Value);
        Assert.True(devices.Count > 0);
    }

    [Fact]
    public async Task GetDeviceState_ExistingDevice_ReturnsOk()
    {
        var result = await _controller.GetDeviceState("CNC-001", CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var state = Assert.IsType<RemoteMonitoring.Core.Models.MachineState>(okResult.Value);
        Assert.Equal("CNC-001", state.DeviceId);
    }

    [Fact]
    public async Task GetDeviceState_NonExistingDevice_ReturnsNotFound()
    {
        var result = await _controller.GetDeviceState("NON-EXISTING", CancellationToken.None);

        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetDeviceHistory_ReturnsOkWithHistory()
    {
        var result = await _controller.GetDeviceHistory("CNC-001", 10, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var history = Assert.IsAssignableFrom<IReadOnlyList<RemoteMonitoring.Core.Models.MachineState>>(okResult.Value);
        Assert.True(history.Count > 0);
    }
}
