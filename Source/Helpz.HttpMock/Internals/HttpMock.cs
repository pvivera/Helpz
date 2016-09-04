// The MIT License (MIT)
//
// Copyright (c) 2015 Rasmus Mikkelsen
// https://github.com/rasmus/Helpz
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do so,
// subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;

namespace Helpz.HttpMock.Internals
{
    internal class HttpMock : IHttpMock
    {
        private readonly List<MockEndpoint> _mockEndpoints = new List<MockEndpoint>();
        private readonly IDisposable _webApp;
        private readonly HttpStatusCode _defaultHttpStatusCode;

        internal HttpMock(HttpStatusCode defaultHttpStatusCode)
        {
            var port = TcpHelpz.GetFreePort();

            Uri = new Uri($"http://localhost:{port}");
            _defaultHttpStatusCode = defaultHttpStatusCode;
            _webApp = WebApp.Start(Uri.ToString(), app => { app.Use(Handler); });
        }

        public Uri Uri { get; }

        public void Mock(HttpMethod httpMethod, string path, Func<IOwinContext, Task> handler)
        {
            _mockEndpoints.Add(new MockEndpoint(httpMethod, path, handler));
        }

        public void Mock(HttpMethod httpMethod, string path, HttpStatusCode httpStatusCode)
        {
            _mockEndpoints.Add(new MockEndpoint(httpMethod, path, c =>
                {
                    c.Response.StatusCode = (int) httpStatusCode;
                    return Task.FromResult(0);
                }));
        }

        public void Mock(HttpMethod httpMethod, string path, string response)
        {
            _mockEndpoints.Add(new MockEndpoint(httpMethod, path, async c =>
                {
                    c.Response.StatusCode = (int) HttpStatusCode.OK;
                    await c.Response.WriteAsync(response).ConfigureAwait(false);
                }));
        }

        public void Dispose()
        {
            _webApp.Dispose();
        }

        public void Mock(HttpMethod httpMethod, string path, Func<Request, Response> action)
        {
            _mockEndpoints.Add(new MockEndpoint(httpMethod, path, async c =>
            {
                string content;
                using (var streamReader = new StreamReader(c.Request.Body))
                {
                    content = await streamReader.ReadToEndAsync().ConfigureAwait(false);
                }
                var mockRequest = new Request(
                    c.Request.Uri,
                    new HttpMethod(c.Request.Method),
                    content,
                    c.Request.Headers);
                var mockResponse = action(mockRequest);
                c.Response.StatusCode = (int) mockResponse.HttpStatusCode;
                await c.Response.WriteAsync(mockResponse.Content).ConfigureAwait(false);
            }));
        }

        private async Task Handler(IOwinContext owinContext, Func<Task> func)
        {
            var httpMethod = new HttpMethod(owinContext.Request.Method);
            var validMockEndpoints = _mockEndpoints
                .Where(m => m.HttpMethod == httpMethod && m.PathRegex.IsMatch(owinContext.Request.Uri.AbsolutePath))
                .ToList();
            if (!validMockEndpoints.Any())
            {
                owinContext.Response.StatusCode = (int) _defaultHttpStatusCode;
                await owinContext.Response.WriteAsync($"No handler for path '{owinContext.Request.Uri.AbsolutePath}'").ConfigureAwait(false);
                return;
            }
            if (validMockEndpoints.Count > 1)
            {
                owinContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await owinContext.Response.WriteAsync($"More than one handler for path '{owinContext.Request.Uri.AbsolutePath}'").ConfigureAwait(false);
                return;
            }

            var mockEndpoint = validMockEndpoints.Single();

            try
            {
                await mockEndpoint.Handler(owinContext).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var response = $"Failed to invoke handler {mockEndpoint.HttpMethod} {mockEndpoint.PathRegex} {Environment.NewLine} {e}";
                owinContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                await owinContext.Response.WriteAsync(response).ConfigureAwait(false);
            }
        }
    }
}
