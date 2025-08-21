using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Json;
using GittyMcp.Configuration;
using GittyMcp.Models;

namespace GittyMcp.Services;

public class GitHubService
{
    private readonly HttpClient _httpClient;

    public GitHubService(HttpClient httpClient, IOptions<GitHubOptions> options)
    {
        _httpClient = httpClient;

        var token = options.Value.Pat ?? throw new InvalidOperationException("Token for github is missing!");

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
    }

    public async Task<List<Issue>> GetIssuesAsync(string owner, string repo)
    {
        try
        {
            var res = await _httpClient.GetAsync($"repos/{owner}/{repo}/issues?state=open");
            res.EnsureSuccessStatusCode();
            var json = await res.Content.ReadAsStringAsync();
            var issues = JsonSerializer.Deserialize<List<Issue>>(json) ?? new List<Issue>();
            return issues;

        } catch
        {
            throw new Exception("Something is wrong here hmm");
        }
    }
}
