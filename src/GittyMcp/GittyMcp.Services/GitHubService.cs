using System.Text;
using System.Text.Json;
using GittyMcp.GittyMcp.Configuration;
using GittyMcp.GittyMcp.Models;

namespace GittyMcp.GittyMcp.Services;

public class GitHubService : IGitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<IssueGet>> GetIssuesAsync(string owner, string repo)
    {
        try
        {
            var res = await _httpClient.GetAsync($"repos/{owner}/{repo}/issues?state=open");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var issues = JsonSerializer.Deserialize<List<IssueGet>>(json) ?? new List<IssueGet>();
            return issues;

        }
        catch (HttpRequestException)
        {
            throw new InvalidOperationException("Failed to fetch issues");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Invalid response format");
        }
    }

    public async Task<IssueGet> CreateIssueAsync(string owner, string repo, IssueCreate createAnIssue)
    {
        try
        {
            var json = JsonSerializer.Serialize(createAnIssue, JsonConfig.PropOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = $"repos/{owner}/{repo}/issues";

            var res = await _httpClient.PostAsync(uri, content);
            res.EnsureSuccessStatusCode();

            var jsonRes = await res.Content.ReadAsStringAsync();
            var createdIssue = JsonSerializer.Deserialize<IssueGet>(jsonRes) ??
                throw new InvalidOperationException("Invalid format of the respons");

            return createdIssue;
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException($"Failed to create issue, {ex.Message}");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Invalid json format, {ex.Message}");
        }
    }
}
