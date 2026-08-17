using IntegrationTest.Core;

namespace IntegrationTest.Tests;

public sealed class DeploymentConfigTests
{
    [Fact]
    public void GeneratePublishCommand_ContainsRequiredParameters()
    {
        var config = new DeploymentConfig
        {
            ApplicationName = "TestApp",
            Version = "2.0.0",
            OutputDirectory = "./dist",
            RuntimeIdentifier = "linux-x64"
        };

        var command = config.GeneratePublishCommand("./MyProject.csproj");

        Assert.Contains("dotnet publish", command);
        Assert.Contains("./MyProject.csproj", command);
        Assert.Contains("-c Release", command);
        Assert.Contains("-o \"./dist\"", command);
        Assert.Contains("-r linux-x64", command);
        Assert.Contains("-p:Version=2.0.0", command);
    }

    [Fact]
    public void GeneratePublishCommand_WithSelfContained_IncludesFlag()
    {
        var config = new DeploymentConfig { SelfContained = true };
        var command = config.GeneratePublishCommand("./test.csproj");
        Assert.Contains("--self-contained true", command);
    }

    [Fact]
    public void GeneratePublishCommand_WithSymbols_UsesValidMsBuildProperties()
    {
        var config = new DeploymentConfig { IncludeSymbols = true };

        var command = config.GeneratePublishCommand("./test.csproj");

        Assert.DoesNotContain("--include-symbols", command);
        Assert.Contains("-p:DebugSymbols=true", command);
        Assert.Contains("-p:DebugType=portable", command);
    }

    [Fact]
    public void GeneratePublishCommand_WithCompression_EnablesSingleFilePublishing()
    {
        var config = new DeploymentConfig { EnableCompression = true };

        var command = config.GeneratePublishCommand("./test.csproj");

        Assert.Contains("--self-contained true", command);
        Assert.Contains("-p:PublishSingleFile=true", command);
        Assert.Contains("-p:EnableCompressionInSingleFile=true", command);
    }

    [Fact]
    public void GenerateDockerfile_ContainsValidStructure()
    {
        var config = new DeploymentConfig { ApplicationName = "MyApp" };
        var dockerfile = config.GenerateDockerfile("./MyApp.csproj");

        Assert.Contains("FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build", dockerfile);
        Assert.Contains("FROM mcr.microsoft.com/dotnet/aspnet:10.0", dockerfile);
        Assert.Contains("dotnet publish", dockerfile);
        Assert.Contains("MyApp.dll", dockerfile);
    }

    [Fact]
    public void GenerateDockerfile_WithDifferentNames_PublishesTheEntrypointAssembly()
    {
        var config = new DeploymentConfig { ApplicationName = "Monitor.Host" };

        var dockerfile = config.GenerateDockerfile("./SourceProject.csproj");

        Assert.Contains("dotnet publish \"./SourceProject.csproj\"", dockerfile);
        Assert.Contains("-p:AssemblyName=Monitor.Host", dockerfile);
        Assert.Contains("ENTRYPOINT [\"dotnet\", \"Monitor.Host.dll\"]", dockerfile);
    }
}
