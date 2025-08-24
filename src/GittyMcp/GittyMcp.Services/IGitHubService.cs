using GittyMcp.GittyMcp.Models;

namespace GittyMcp.GittyMcp.Services;

public interface IGitHubService
{
  Task<List<IssueGet>> GetIssuesAsync(string owner, string repo);
  Task<IssueGet> CreateIssueAsync(string owner, string repo, IssueCreate createAnIssue);
};
