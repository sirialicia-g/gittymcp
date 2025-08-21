namespace GittyMcp.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> PatchAsync(this HttpClient client, string requestUri, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) { Content = content };
        return await client.SendAsync(request);
    }
}