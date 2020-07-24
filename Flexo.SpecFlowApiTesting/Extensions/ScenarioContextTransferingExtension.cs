using System;
using System.Collections.Generic;
using System.Text;
using Flexo.SpecFlowApiTesting.Fixture;
using Google.Protobuf.WellKnownTypes;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting.Extensions
{
    public static class ScenarioContextTransferingExtension
    { public static void IninializeHttpClientProperties(this ScenarioContext context)
        {
            context.Set("", Field.HttpClientQuery);
            context.Set("", Field.HttpClientBaseAddress);
            context.Set("", Field.SimpleApiRequest);
            context.Set(new Dictionary<string, string>(), Field.HttpClientHeaders);
        }
        public static void Set<T>(this ScenarioContext context, T data, Field key)
        {
            context.Set<T>(data, key.ToString());
        }

        public static T Get<T>(this ScenarioContext context, Field key)
        {
            return context.Get<T>(key.ToString());
        }

        public static void SetSimpleApiRequest(this ScenarioContext context, string apiRequestString)
        {
            context.Set(apiRequestString, Field.SimpleApiRequest);
        }

        public static string GetSimpleApiRequest(this ScenarioContext context)
        {
            return context.Get<string>(Field.SimpleApiRequest);
        }

        public static void SetHttpClientActualResponse(this ScenarioContext context, SimpleApiResponse apiResponse, JToken jsonResponse)
        {
            context.Set(apiResponse, Field.ActualHttpResponse);
            context.Set(jsonResponse, Field.ActualJsonResponse);
        }

        public static SimpleApiResponse GetActualHttpResponse(this ScenarioContext context)
        {
            return context.Get<SimpleApiResponse>(Field.ActualHttpResponse);
        }

        public static JToken GetActualJsonResponse(this ScenarioContext context)
        {
            return context.Get<JToken>(Field.ActualJsonResponse);
        }

        public static void SetActualJsonResponse(this ScenarioContext context, JToken jsonResponse)
        {
            context.Set(jsonResponse, Field.ActualJsonResponse);
        }

        public static void SetHttpClientBaseAddress(this ScenarioContext context, string apiBaseAddress)
        {
            context.Set(apiBaseAddress, Field.HttpClientBaseAddress);
        }

        public static string GetHttpClientBaseAddress(this ScenarioContext context)
        {
            return context.Get<string>(Field.HttpClientBaseAddress);
        }

        public static void SetHttpClientQueryString(this ScenarioContext context, string defaultQueryString)
        {
            context.Set(defaultQueryString, Field.HttpClientQuery);
        }

        public static string GetHttpClientQueryString(this ScenarioContext context)
        {
            return context.Get<string>(Field.HttpClientQuery);
        }

        public static void SetHttpClientHeader(this ScenarioContext context, string key, string value)
        {
            context.GetHttpClientHeader()?.Add(key, value);
        }

        public static IDictionary<string, string> GetHttpClientHeader(this ScenarioContext context)
        {
            return context.Get<IDictionary<string, string>>(Field.HttpClientHeaders);
        }
    }

    public enum Field
    {
        HttpClientQuery,
        HttpClientHeaders,
        HttpClientBaseAddress,
        ActualHttpResponse,
        ActualJsonResponse,
        SimpleApiRequest
    }
}
