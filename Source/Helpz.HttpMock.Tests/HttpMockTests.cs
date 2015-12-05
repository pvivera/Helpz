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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Helpz.HttpMock.Tests
{
    public class HttpMockTests
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private IHttpMock _httpMock;

        [SetUp]
        public void SetUp()
        {
            _httpMock = HttpMockHelpz.CreateHttpMock();
        }

        [TearDown]
        public void TearDown()
        {
            _httpMock.Dispose();
        }

        [Test]
        public async Task MockResponseString()
        {
            // Arrange
            const string expectedResponse = "good response";
            _httpMock.Mock(HttpMethod.Get, "/endpoint/1", HttpStatusCode.Forbidden);
            _httpMock.Mock(HttpMethod.Get, "/endpoint/2", expectedResponse);

            // Act
            var response = await GetAsStringAsync("/endpoint/2").ConfigureAwait(false);

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Test]
        public async Task MockResponse()
        {
            // Arrange
            const string expectedResponse = "good response";
            _httpMock.Mock(HttpMethod.Get, "/endpoint", r => new MockResponse(expectedResponse));

            // Act
            var response = await GetAsStringAsync("/endpoint").ConfigureAwait(false);

            // Assert
            response.Should().Be(expectedResponse);
        }

        [Test]
        public async Task ThrownExceptionIsHandled()
        {
            // Arrange
            _httpMock.Mock(HttpMethod.Get, "/endpoint", r =>
                {
                    if (r != null) throw new Exception();
                    return MockResponse();
                });

            // Act
            var response = await GetAsync("/endpoint").ConfigureAwait(false);
            var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
            content.Should().Contain("Failed to invoke handler");
        }

        private async Task<string> GetAsStringAsync(string path)
        {
            using (var httpResponseMessage = await GetAsync(path).ConfigureAwait(false))
            {
                return await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        private Task<HttpResponseMessage> GetAsync(string path)
        {
            var mockUri = new Uri(_httpMock.Uri, path);
            return HttpClient.GetAsync(mockUri);
        }
    }
}