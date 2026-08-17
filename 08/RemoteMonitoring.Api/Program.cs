using RemoteMonitoring.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// 注册服务
builder.Services.AddControllers();
builder.Services.AddSingleton<IDeviceStateService, SimulatedDeviceStateService>();

// 配置 CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 配置中间件
app.UseCors();
app.MapControllers();

app.Run();
