
using System.Text.Json.Serialization;

namespace GittyMcp.Models;
public class Issue
{
  public int Number { get; set; }
  public string Title { get; set; } = string.Empty;
  public string State { get; set; } = string.Empty;
  [JsonPropertyName("html_url")] public string HtmlUrl { get; set; } = string.Empty;
  public string Body { get; set; } = string.Empty;
}
