namespace IntegrationTest.Core;

/// <summary>
/// 部署配置。
/// </summary>
public sealed class DeploymentConfig
{
    public string Environment { get; set; } = "Development";
    public string ApplicationName { get; set; } = "UpperComputerMonitor";
    public string Version { get; set; } = "1.0.0";
    public string OutputDirectory { get; set; } = "./publish";
    public bool SelfContained { get; set; } = false;
    public string RuntimeIdentifier { get; set; } = "win-x64";
    public bool EnableCompression { get; set; } = true;
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
