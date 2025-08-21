using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using GittyMcp.Configuration;
using GittyMcp.Services;
using GittyMcp.Tools;

var builder = Host.CreateApplicationBuilder(args);
var environment = builder.Environment.EnvironmentName;
Console.WriteLine($"Environment: {environment}");

builder.Logging.AddConsole(options =>
{
    options.LogToStandardErrorThreshold = LogLevel.Trace;
});

builder.Services.Configure<GitHubOptions>(
    builder.Configuration.GetSection("GitHub"));

builder.Services.AddHttpClient<GitHubService>("github-api", client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("McpGitHub", "1.0"));
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
});

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
