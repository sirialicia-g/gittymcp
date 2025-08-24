using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using GittyMcp.GittyMcp.Configuration;
using ModelContextProtocol.Server;
using GittyMcp.GittyMcp.Services;
using Microsoft.Extensions.Options;
using System.Text.Json;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.Configure<GittyPat>(
    builder.Configuration.GetSection("GitHub"));

builder.Services.AddHttpClient<IGitHubService, GitHubService>("github-api", ( serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<GittyPat>>().Value;
    var token = options.Pat ?? throw new InvalidOperationException("Github token is missing!");

    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("McpGitHub", "1.0"));
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
