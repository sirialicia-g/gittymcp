using GittyMcp.GittyMcp.Services;
using GittyMcp.GittyMcp.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;
using GittyMcp.GittyMcp.Configuration;
using GittyMcp.Tests.Integration;

namespace GittyMcp.Tests.Integration.Services;

public class GitHubServiceTestsCreateIssue
{

    private static IssueGet CreateTestIssue() => new()
    {
        Number = 3,
        Title = "Created Issue",
        State = "open",
        GittyUrl = "https://github.com/test/repo/issues/42",
        Body = "This is a test issue"
    };

    private static GitHubService CreateServiceWithJsonResponse(object data, HttpStatusCode statusCode = HttpStatusCode.OK)
  {
    var json = JsonSerializer.Serialize(data);
    var response = new HttpResponseMessage(statusCode)
    {
      Content = new StringContent(json, Encoding.UTF8, "application/json")
    };
    return CreateServiceWithResponse(response);
  }

    private static GitHubService CreateServiceWithTextResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, Encoding.UTF8, "text/plain")
        };
        return CreateServiceWithResponse(response);
    }

    private static GitHubService CreateServiceWithResponse(HttpResponseMessage response)
    {
        var mockHandler = new MockHttpMessageHandler(response);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };
        return new GitHubService(httpClient);
    }

    private static GitHubService CreateServiceMock(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsyncFunc)
    {
        var mockHandler = new MockedHandler(sendAsyncFunc);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };
        return new GitHubService(httpClient);
    }

    [Fact]
    public async Task CreateIssue_ShouldReturnCreatedIssue_WhenSuccessful()
    {
        var createdIssue = CreateTestIssue();
        var service = CreateServiceWithJsonResponse(createdIssue);
        var issueToCreate = new IssueCreate { Title = "Test", Body = "Body" };

        var result = await service.CreateIssueAsync("owner", "repo", issueToCreate);

        Assert.NotNull(result);
        Assert.Equal(3, result.Number);
        Assert.Equal("Created Issue", result.Title);
    }

    [Fact]
    public async Task CreateIssue_ShouldCallCorrectEndpoint()
    {
        HttpRequestMessage capturedRequest = null;
        string capturedContent = null;

        var service = CreateServiceMock(async (request, _) =>
        {
            capturedRequest = request;
            capturedContent = await request.Content?.ReadAsStringAsync();
            var json = JsonSerializer.Serialize(CreateTestIssue(), JsonConfig.PropOptions);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            return response;
        });

        var issueToCreate = new IssueCreate { Title = "test title", Body = "test text" };
        await service.CreateIssueAsync("testowner", "testrepo", issueToCreate);

        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Post, capturedRequest.Method);
        Assert.Equal("https://api.github.com/repos/testowner/testrepo/issues",
            capturedRequest.RequestUri?.ToString());

        var sentData = JsonSerializer.Deserialize<IssueCreate>(capturedContent, JsonConfig.PropOptions);
        Assert.Equal("test title", sentData.Title);
        Assert.Equal("test text", sentData.Body);
    }

    [Fact]
    public async Task CreateIssue_ShouldThrowException_WhenFail()
    {
        var service = CreateServiceMock((_, _) =>
            throw new InvalidOperationException("Network error"));
        var issueToCreate = new IssueCreate { Title = "Test", Body = "Body" };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateIssueAsync("owner", "repo", issueToCreate));
        Assert.Equal("Network error", exception.Message);
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.Forbidden)]
    [InlineData(HttpStatusCode.UnprocessableEntity)]
    public async Task CreateIssue_ShouldThrowException_WhenApiReturnsError(HttpStatusCode statusCode)
    {

        var service = CreateServiceWithTextResponse("Error", statusCode);
        var issueToCreate = new IssueCreate { Title = "Test", Body = "Body" };

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateIssueAsync("owner", "repo", issueToCreate));
        Assert.StartsWith("Failed to create issue", exception.Message);
    }
}
