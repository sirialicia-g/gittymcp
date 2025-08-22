using GittyMcp.Models;

namespace GittyMcp.GittyMcp.Services;
public interface IGitHubService
{
  Task<List<Issue>> GetIssuesAsync(string owner, string repo);
};
