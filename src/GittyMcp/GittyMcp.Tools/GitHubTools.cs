using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using Microsoft.Extensions.Logging;
using GittyMcp.Services;
using GittyMcp.GittyMcp.Services;

namespace GittyMcp.Tools;

[McpServerToolType]
public sealed class GitHubTools
{
    private readonly IGitHubService _gitHubService;

    public GitHubTools(IGitHubService gitHubService)
    {
        _gitHubService = gitHubService;
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
