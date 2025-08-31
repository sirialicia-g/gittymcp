using GittyMcp.GittyMcp.Services;
using GittyMcp.GittyMcp.Models;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Http;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GittyMcp.Tests.Integration;

namespace GittyMcp.Tests.Integration.Services;

public class GitHubServiceTests
{
    [Fact]
    public async Task GetIssuesAsync_ShouldReturnIssues()
    {
        var expectedIssues = new List<IssueGet>
        {
            new() { Number = 1, Title = "testissue", State = "open", GittyUrl = "https://github.com/test/repo/issues/1" },
            new() { Number = 2, Title = "another issue", State = "open", GittyUrl = "https://github.com/test/repo/issues/2" }
        };

        var jsonResponse = JsonSerializer.Serialize(expectedIssues);
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
        };

        var mockHandler = new MockHttpMessageHandler(httpResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        var service = new GitHubService(httpClient);
        var result = await service.GetIssuesAsync("owner", "repo");

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result[0].Number);
        Assert.Equal("testissue", result[0].Title);
        Assert.Equal("open", result[0].State);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldReturnEmptyList_WhenReturnsEmptyArray()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("[]", Encoding.UTF8, "application/json")
        };

        var mockHandler = new MockHttpMessageHandler(httpResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };
        var service = new GitHubService(httpClient);
        var result = await service.GetIssuesAsync("owner", "repo");
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldThrowException_WhenFail()
    {
        var mockHandler = new MockedHandler((request, cancellationToken) =>
            throw new HttpRequestException("Network error"));

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        var service = new GitHubService(httpClient);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetIssuesAsync("owner", "repo"));

        Assert.Equal("Failed to fetch issues", exception.Message);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldThrowException_WhenJsonIsBad()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("invalid json", Encoding.UTF8, "application/json")
        };

        var mockHandler = new MockHttpMessageHandler(httpResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        var service = new GitHubService(httpClient);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetIssuesAsync("owner", "repo"));

        Assert.Equal("Invalid response format", exception.Message);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldThrowException_WhenReturns404()
    {
        var httpResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("Not Found", Encoding.UTF8, "text/plain")
        };

        var mockHandler = new MockHttpMessageHandler(httpResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };
        var service = new GitHubService(httpClient);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.GetIssuesAsync("owner", "repo"));

        Assert.Equal("Failed to fetch issues", exception.Message);
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldCallCorrectEndpoint()
    {
        HttpRequestMessage capturedRequest = null;

        var mockHandler = new MockedHandler((request, cancellationToken) =>
        {
            capturedRequest = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]", Encoding.UTF8, "application/json")
            });
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        var service = new GitHubService(httpClient);
        await service.GetIssuesAsync("testowner", "testrepo");
        Assert.NotNull(capturedRequest);
        Assert.Equal(HttpMethod.Get, capturedRequest.Method);
        Assert.Equal("https://api.github.com/repos/testowner/testrepo/issues?state=open", capturedRequest.RequestUri?.ToString());
    }

    [Fact]
    public async Task GetIssuesAsync_ShouldHandleANullResponse()
    {

        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", Encoding.UTF8, "application/json")
        };

        var mockHandler = new MockHttpMessageHandler(httpResponse);
        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://api.github.com/")
        };

        var service = new GitHubService(httpClient);
        var result = await service.GetIssuesAsync("owner", "repo");
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
