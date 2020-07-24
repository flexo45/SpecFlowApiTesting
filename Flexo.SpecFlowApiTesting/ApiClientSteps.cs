using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Fixture;
using Flexo.SpecFlowApiTesting.Utils;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting
{
    [Binding]
    class ApiClientSteps
    {
        private ScenarioContext scenarioContext;
        private SpecFlowContextUtils specFlowContextUtils;
        private SimpleApiHttpClientFixture httpClientFixture;

        private string lastRequestPathWithQueryParams;
        private HttpMethod lastRequestHttpMethod;

        public ApiClientSteps(
            ScenarioContext scenarioContext,
            SpecFlowContextUtils specFlowContextUtils)
        {
            this.scenarioContext = scenarioContext;
            this.specFlowContextUtils = specFlowContextUtils;
            httpClientFixture = new SimpleApiHttpClientFixture(scenarioContext);
        }

        [Given(@"запросы .+ в заголов[а-я]+ ""(.*)""")]
        public void ДопустимЗапросыКАпиСодержатАктивныйБилетПользователяВЗаголовке(string headerKeyValue)
        {
            var headerParam = headerKeyValue.ParseList(":");
            scenarioContext.SetHttpClientHeader(headerParam[0], headerParam[1]);
        }

        [Given(@".*в параметрах запроса ""(.*)""")]
        public void ДопустимВПараметрахЗапроса(string p0)
        {
            scenarioContext.SetHttpClientQueryString(p0);
        }


        [Given(@".*располож[а-я]+ по адресу ""(.*)"".*")]
        public void GivenApiPlacedAtBaseAddress(string baseApiAddress)
        {
            scenarioContext.SetHttpClientBaseAddress(baseApiAddress);
        }

        [When(@"запрашива[а-я]+ метод ([A-Z]+) ""(.*)"".*")]
        public void WhenRequestApiMethod(HttpMethod httpMethod, string pathWithQueryParams)
        {
            pathWithQueryParams = specFlowContextUtils.ResolveExtractorExpression(pathWithQueryParams);
            lastRequestPathWithQueryParams = pathWithQueryParams;
            lastRequestHttpMethod = httpMethod;
            ProcessSimpleApiResponse(httpClientFixture.SendRequest(httpMethod, pathWithQueryParams));
        }

        [When(@"отправл[а-я]+ запрос с json в метод ([A-Z]+) ""(.*)"".*")]
        public void ЕслиОтправляемЗапросСJsonВМетодPOST(HttpMethod httpMethod, string pathWithQueryParams, string multilineText)
        {
            pathWithQueryParams = specFlowContextUtils.ResolveExtractorExpression(pathWithQueryParams);
            multilineText = specFlowContextUtils.ResolveExtractorExpression(multilineText);
            multilineText = specFlowContextUtils.ResolveDataGenerationExpression(multilineText);
            ProcessSimpleApiResponse(httpClientFixture.SendRequest(httpMethod, pathWithQueryParams, multilineText));
        }

        [Then(@"[дД]олж[а-я]+ вернуть.*ответ со статус кодом ""(.*)"".*")]
        public void ThenResponseShouldHasStatusCode(HttpStatusCode statusCode)
        {
            scenarioContext.GetActualHttpResponse().StatusCode.Should().Be(statusCode, $"Получен ответ: {scenarioContext.GetActualHttpResponse().StringContent}");
        }

        [Then(@"запрашива[а-я]+ метод ([A-Z]+) ""(.*)"".*")]
        public void ТоЗапрашиваемМетодGETВЕС(System.Net.Http.HttpMethod httpMethod, string pathWithQueryParams)
        {
            pathWithQueryParams = specFlowContextUtils.ResolveExtractorExpression(pathWithQueryParams);
            lastRequestPathWithQueryParams = pathWithQueryParams;
            lastRequestHttpMethod = httpMethod;
            ProcessSimpleApiResponse(httpClientFixture.SendRequest(httpMethod, pathWithQueryParams));
        }

        [Then(@"жде[а-я]+ пока ""(.*)"" будет равен ""(.*)""")]
        public void ТоЖдемПокаБудетРавен(string selectTokenPath, ExpectedJToken expectation)
        {
            var timeout = 240;
            var complete = false;
            while (!complete)
            {
                ProcessSimpleApiResponse(httpClientFixture.SendRequest(lastRequestHttpMethod, lastRequestPathWithQueryParams));
                try
                {
                    var targetToken = JTokenParser.ParseJTokenFromJObject(selectTokenPath,
                        (JObject)scenarioContext.GetActualJsonResponse());

                    JTokenAssertion.AssertJTokenEquals(targetToken, expectation, selectTokenPath);

                    complete = true;
                }
                catch
                {
                    // ignored
                }
                finally
                {
                    if (timeout <= 0)
                    {
                        throw new Exception($"Не удалось дождаться результата {selectTokenPath}={expectation.JToken} от запроса {lastRequestHttpMethod} {lastRequestPathWithQueryParams}");
                    }
                }

                Thread.Sleep(1000);
                timeout--;
            }
        }

        [StepArgumentTransformation]
        public System.Net.Http.HttpMethod TransformToHttpMethod(string httpMethodString)
        {
            return typeof(HttpMethod).ExtractStaticProperty(httpMethodString.Trim().ToLower().ToUpperCaseFirstChar()) as System.Net.Http.HttpMethod;
        }

        private void ProcessSimpleApiResponse(SimpleApiResponse response)
        {
            JToken jsonResponse = null;
            if (!string.IsNullOrEmpty(response.StringContent))
            {
                try
                {
                    jsonResponse = JToken.Parse(response.StringContent);
                }
                catch (Exception e)
                {
                    jsonResponse = JObject.Parse(@"{""nonJsonResponse"": """", ""error"": """"}");
                    jsonResponse["nonJsonResponse"] = response.StringContent;
                    jsonResponse["error"] = e.ToString();
                }

            }
            scenarioContext.SetHttpClientActualResponse(response, jsonResponse);
        }
    }
}
