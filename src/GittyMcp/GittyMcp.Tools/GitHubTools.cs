using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;
using GittyMcp.Services;

namespace GittyMcp.Tools;

[McpServerToolType]
public sealed class GitHubTools
{
    private readonly GitHubService _gitHubService;
    // private readonly ILogger<GitHubTools> _logger;

    public GitHubTools(GitHubService gitHubService)
    {
        _gitHubService = gitHubService;
        // _logger = logger;
    }

    [McpServerTool, Description("List open GitHub issues for a repository.")]
    public async Task<string> ListIssues(
        [Description("Owner of the repo")] string owner,
        [Description("Repository name")] string repo)
    {

            var issues = await _gitHubService.GetIssuesAsync(owner, repo);

            var result = JsonSerializer.Serialize(issues, new JsonSerializerOptions { WriteIndented = true });
            return result;
    }
}
