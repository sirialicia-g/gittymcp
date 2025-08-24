using System.Text.Json;

namespace GittyMcp.GittyMcp.Configuration;

public static class JsonConfig
{
  public static readonly JsonSerializerOptions PropOptions = new()
  {
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true
  };
}
