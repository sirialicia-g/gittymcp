using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using GittyMcp.GittyMcp.Services;
using GittyMcp.GittyMcp.Models;

namespace GittyMcp.GittyMcp.Tools;

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

    [McpServerTool, Description("Create GitHub issue for chosen repository")]
    public async Task<string> CreateIssue(
        [Description("Repository name")] string owner,
        [Description("Owner of the repo")] string repo,
        [Description("Issue title")] string title,
        [Description("Issue contents")]string body)
    {
        //NÖDVÄNDIG? Borde väl kunna gå på klassen?
        var createAnIssue = new IssueCreate { Title = title, Body = body };
        var issue = await _gitHubService.CreateIssueAsync(owner, repo, createAnIssue);
        var result = JsonSerializer.Serialize(issue, new JsonSerializerOptions { WriteIndented = true });
        return result;
    }
}
