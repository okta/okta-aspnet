using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Okta.AspNet.Test
{
    public class MockHttpClientHandler : DelegatingHandler
    {
        private readonly string _response;
        private readonly HttpStatusCode _statusCode;

        public string Body { get; private set; }
        public int NumberOfCalls { get; private set; }

        public MockHttpClientHandler(string response = "{}", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _response = response;
            _statusCode = statusCode;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            NumberOfCalls++;

            if (request.Content != null)
            {
                Body = await request.Content.ReadAsStringAsync();
            }

            return new HttpResponseMessage
            {
                StatusCode = _statusCode,
                Content = new StringContent(_response),
            };
        }
    }
}
