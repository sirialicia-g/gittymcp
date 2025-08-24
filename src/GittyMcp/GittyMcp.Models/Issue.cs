
using System.Text.Json.Serialization;

namespace GittyMcp.GittyMcp.Models;

public class Issue
{
  public string Title { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
}

public class IssueGet : Issue
{
  public int Number { get; set; }
  public string State { get; set; } = string.Empty;

  [JsonPropertyName("html_url")]
  public string? GittyUrl { get; set; } = string.Empty;
}

public class IssueCreate : Issue
{
}
