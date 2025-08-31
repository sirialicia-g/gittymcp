using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace GittyMcp.Tests.Integration.Services;
internal class MockHttpMessageHandler : HttpMessageHandler
{
  private readonly HttpResponseMessage _responseMessage;

  public MockHttpMessageHandler(HttpResponseMessage responseMessage)
  {
    _responseMessage = responseMessage;
  }

  protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
      CancellationToken cancellationToken)
  {
    return Task.FromResult(_responseMessage);
  }
}

internal class MockedHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _sendAsyncFunc;

    public MockedHandler(
        Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> sendAsyncFunc)
    {
        _sendAsyncFunc = sendAsyncFunc;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return await _sendAsyncFunc(request, cancellationToken);
    }
}
