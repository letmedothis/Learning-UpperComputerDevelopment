namespace IntegrationTest.Core;

/// <summary>
/// 部署配置。
/// </summary>
public sealed class DeploymentConfig
{
    /// <summary>目标环境，如 Development / Production。</summary>
    public string Environment { get; set; } = "Development";

    /// <summary>应用程序名称，用于程序集名和 Docker ENTRYPOINT。</summary>
    public string ApplicationName { get; set; } = "UpperComputerMonitor";

    /// <summary>版本号。</summary>
    public string Version { get; set; } = "1.0.0";

    /// <summary>发布输出目录。</summary>
    public string OutputDirectory { get; set; } = "./publish";

    /// <summary>是否自包含发布（不依赖运行时安装）。</summary>
    public bool SelfContained { get; set; } = false;

    /// <summary>目标运行时标识符，如 win-x64、linux-x64。</summary>
    public string RuntimeIdentifier { get; set; } = "win-x64";

    /// <summary>是否启用单文件压缩（需自包含发布）。</summary>
    public bool EnableCompression { get; set; } = true;

    /// <summary>是否包含调试符号。</summary>
    public bool IncludeSymbols { get; set; } = false;

    /// <summary>
    /// 生成 dotnet publish 命令。
    /// </summary>
    public string GeneratePublishCommand(string projectPath)
    {
        var args = new List<string>
        {
            "dotnet publish",
            $"\"{projectPath}\"",
            "-c Release",
            $"-o \"{OutputDirectory}\"",
            $"-r {RuntimeIdentifier}",
            SelfContained || EnableCompression ? "--self-contained true" : "--self-contained false",
            // 单文件压缩要求自包含的单文件发布，相关开关必须作为一组生成。
            EnableCompression ? "-p:PublishSingleFile=true" : "",
            EnableCompression ? "-p:EnableCompressionInSingleFile=true" : "",
            IncludeSymbols ? "-p:DebugSymbols=true" : "",
            IncludeSymbols ? "-p:DebugType=portable" : "",
            $"-p:Version={Version}",
            $"-p:AssemblyName={ApplicationName}"
        };

        return string.Join(" ", args.Where(a => !string.IsNullOrEmpty(a)));
    }

    /// <summary>
    /// 生成 Dockerfile 内容。
    /// </summary>
    public string GenerateDockerfile(string projectPath)
    {
        return $@"# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish ""{projectPath}"" -c Release -o /app/publish -p:AssemblyName={ApplicationName}

# 运行阶段
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
EXPOSE 443
ENTRYPOINT [""dotnet"", ""{ApplicationName}.dll""]
";
    }
}
