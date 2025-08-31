using GittyMcp.GittyMcp.Tools;
using GittyMcp.GittyMcp.Services;
using GittyMcp.GittyMcp.Models;
using NSubstitute;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using System.Collections.Generic;
using System;

namespace GittyMcp.Tests.Tools
{
    public class GitHubToolsTests
    {
        private readonly IGitHubService _mockGitHubService;
        private readonly GitHubTools _gitHubTools;

        public GitHubToolsTests()
        {
            _mockGitHubService = Substitute.For<IGitHubService>();
            _gitHubTools = new GitHubTools(_mockGitHubService);
        }

        [Fact]
        public async Task ListIssues_ShouldReturnSerializedIssues_WhenSuccessful()
        {
            var expectedIssues = new List<IssueGet>
            {
                new() { Number = 1, Title = "First testy", State = "open", GittyUrl = "https://github.com/owner/repo/issues/1" },
                new() { Number = 2, Title = "Second testy", State = "closed", GittyUrl = "https://github.com/owner/repo/issues/2" }
            };

            _mockGitHubService.GetIssuesAsync("testowner", "testrepo")
                .Returns(expectedIssues);
            var result = await _gitHubTools.ListIssues("testowner", "testrepo");

            Assert.NotNull(result);
            var deserializedIssues = JsonSerializer.Deserialize<List<IssueGet>>(result);
            Assert.NotNull(deserializedIssues);
            Assert.Equal(2, deserializedIssues.Count);
            Assert.Equal("First testy", deserializedIssues[0].Title);
            Assert.Equal("Second testy", deserializedIssues[1].Title);
        }

        [Fact]
        public async Task ListIssues_ShouldReturnEmptyArray_WhenNoIssues()
        {
            var emptyIssues = new List<IssueGet>();
            _mockGitHubService.GetIssuesAsync("testowner", "testrepo")
                .Returns(emptyIssues);
            var result = await _gitHubTools.ListIssues("testowner", "testrepo");

            Assert.NotNull(result);
            var deserializedIssues = JsonSerializer.Deserialize<List<IssueGet>>(result);
            Assert.NotNull(deserializedIssues);
            Assert.Empty(deserializedIssues);
        }

        [Fact]
        public async Task ListIssues_ShouldCallServiceWithCorrectParameters()
        {
            var issues = new List<IssueGet>();
            _mockGitHubService.GetIssuesAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(issues);
            await _gitHubTools.ListIssues("myowner", "myrepo");

            await _mockGitHubService.Received(1).GetIssuesAsync("myowner", "myrepo");
        }

        [Fact]
        public async Task CreateIssue_ShouldReturnSerializedIssue_WhenSuccessful()
        {
            var createdIssue = new IssueGet
            {
                Number = 1337,
                Title = "New testissue",
                Body = "Issuedescription",
                State = "open",
                GittyUrl = "https://github.com/owner/repo/issues/1337"
            };

            _mockGitHubService.CreateIssueAsync("testowner", "testrepo", Arg.Any<IssueCreate>())
                .Returns(createdIssue);
            var result = await _gitHubTools.CreateIssue("testowner", "testrepo", "New testissue", "Issuedescription");

            Assert.NotNull(result);
            var deserializedIssue = JsonSerializer.Deserialize<IssueGet>(result);
            Assert.NotNull(deserializedIssue);
            Assert.Equal(1337, deserializedIssue.Number);
            Assert.Equal("New testissue", deserializedIssue.Title);
            Assert.Equal("Issuedescription", deserializedIssue.Body);
        }

        [Fact]
        public async Task CreateIssue_ShouldCallServiceWithCorrectParameters()
        {
            var createdIssue = new IssueGet { Number = 1, Title = "Test", State = "open" };
            _mockGitHubService.CreateIssueAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IssueCreate>())
                .Returns(createdIssue);

            await _gitHubTools.CreateIssue("myowner", "myrepo", "issue title", "issue text");

            await _mockGitHubService.Received(1).CreateIssueAsync(
                "myowner",
                "myrepo",
                Arg.Is<IssueCreate>(issue =>
                    issue.Title == "issue title" &&
                    issue.Body == "issue text"));
        }

        [Theory]
        [InlineData("", "body")]
        [InlineData("title", "")]
        [InlineData("", "")]
        public async Task CreateIssue_ShouldHandleEmptyParameters(string title, string body)
        {
            var createdIssue = new IssueGet { Number = 1, Title = title, Body = body, State = "open" };
            _mockGitHubService.CreateIssueAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IssueCreate>())
                .Returns(createdIssue);
            var result = await _gitHubTools.CreateIssue("owner", "repo", title, body);

            Assert.NotNull(result);
            await _mockGitHubService.Received(1).CreateIssueAsync(
                "owner",
                "repo",
                Arg.Is<IssueCreate>(issue =>
                    issue.Title == title &&
                    issue.Body == body));
        }

        [Fact]
        public async Task ListIssues_ShouldPropagateException_WhenServiceThrows()
        {
            _mockGitHubService.GetIssuesAsync(Arg.Any<string>(), Arg.Any<string>())
                .Returns(Task.FromException<List<IssueGet>>(new InvalidOperationException("Service error")));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _gitHubTools.ListIssues("owner", "repo"));
            Assert.Equal("Service error", exception.Message);
        }

        [Fact]
        public async Task CreateIssue_ShouldPropagateException_WhenServiceThrows()
        {
            _mockGitHubService.CreateIssueAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IssueCreate>())
                .Returns(Task.FromException<IssueGet>(new InvalidOperationException("Creation failed")));

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _gitHubTools.CreateIssue("owner", "repo", "title", "body"));
            Assert.Equal("Creation failed", exception.Message);
        }
    }
}
