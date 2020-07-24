using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Flexo.SpecFlowApiTesting.Extensions;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class SimpleApiHttpClientFixture : IDisposable
    {
        private HttpClient httpClient;

        private ScenarioContext scenatioContext;

        public SimpleApiHttpClientFixture(ScenarioContext scenatioContext)
        {
            this.scenatioContext = scenatioContext;
            this.scenatioContext.IninializeHttpClientProperties();

            httpClient = new HttpClient();
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public SimpleApiResponse SendRequest(System.Net.Http.HttpMethod httpMethod, string path)
        {
            var request = new HttpRequestMessage(httpMethod, BuildScenarioUri(path));
            ConfigureScenarioHeaders(request);
            var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
            return new SimpleApiResponse()
            {
                StatusCode = response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                Headers = response.Headers,
                StringContent = response.Content == null ? default : response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            };
        }

        public SimpleApiResponse SendRequest(System.Net.Http.HttpMethod httpMethod, string path, string content, string mediaType = "application/json")
        {
            var request = new HttpRequestMessage(httpMethod, BuildScenarioUri(path));
            request.Content = new StringContent(content, Encoding.UTF8, mediaType);
            ConfigureScenarioHeaders(request);
            var response = httpClient.SendAsync(request).GetAwaiter().GetResult();
            return new SimpleApiResponse()
            {
                StatusCode = response.StatusCode,
                IsSuccess = response.IsSuccessStatusCode,
                Headers = response.Headers,
                StringContent = response.Content == null ? default : response.Content.ReadAsStringAsync().GetAwaiter().GetResult()
            };
        }

        private string BuildScenarioUri(string path)
        {
            var scenarioApiBaseAddress = scenatioContext.GetHttpClientBaseAddress();
            if (scenarioApiBaseAddress.Equals(""))
            {
                scenarioApiBaseAddress = httpClient.BaseAddress?.AbsoluteUri;
            }

            var defaultQueryString = scenatioContext.GetHttpClientQueryString();
            if (defaultQueryString != null)
            {
                path = path.Contains("?")
                    ? $"{path}&{defaultQueryString}"
                    : $"{path}?{defaultQueryString}";
            }
            return $"{scenarioApiBaseAddress}{path}";
        }

        private void ConfigureScenarioHeaders(HttpRequestMessage requestMessage)
        {
            foreach (var header in scenatioContext.GetHttpClientHeader())
            {
                requestMessage.Headers.Add(header.Key, header.Value);
            }
        }
    }


    public class SimpleApiResponse
    {

        public HttpStatusCode StatusCode { get; set; }

        public string StringContent { get; set; }

        public HttpResponseHeaders Headers { get; set; }

        public bool IsSuccess { get; set; }
    }
}
