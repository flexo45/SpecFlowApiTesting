using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Utils;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting
{
    [Binding]
    class ApiAssertionSteps
    {
        private readonly ScenarioContext scenarioContext;
        private SpecFlowContextUtils specFlowContextUtils;
        readonly ExpectedJTokenFactory expectedJTokenFactory;

        public ApiAssertionSteps(
            ExpectedJTokenFactory expectedJTokenFactory,
            ScenarioContext scenarioContext,
            SpecFlowContextUtils specFlowContextUtils)
        {
            this.expectedJTokenFactory = expectedJTokenFactory;
            this.scenarioContext = scenarioContext;
            this.specFlowContextUtils = specFlowContextUtils;
        }

        [Then(@".*[А-Я][а-я]* ""(.*)"" долж[а-я]+ быть ""(.*)""")]
        public void ТоВПолученномОтветеИмяДолжноБыть(IEnumerable<IAssertableJToken> assertableValues, ExpectedJToken expectation)
        {
            foreach (var val in assertableValues)
            {
                val.AssertEquals(expectation);
            }
        }

        [Then(@".*[А-Я][а-я]* ""(.*)"" долж[а-я]+ содержать ""(.*)""")]
        public void ТоСсылкаНаИконкуДолжнаСодержать(IEnumerable<IAssertableJToken> assertableValues, ExpectedJToken expectation)
        {
            foreach (var val in assertableValues)
            {
                val.AssertContains(expectation);
            }
        }

        [Then(@".*[А-Я][а-я]* ""(.*)"" долж[а-я]+ быть (не пуст[а-я]+|пуст[а-я]+)")]
        public void ТоТекстПубликацииДолженБытьНеПустым(IEnumerable<IAssertableJToken> assertableValues, AssertCondition condition)
        {
            foreach (var val in assertableValues)
            {
                val.AssertCondition(condition);
            }
        }

        [StepArgumentTransformation]
        public AssertCondition TransformToAssertCondition(string text)
        {
            if (text.Contains("не пуст"))
            {
                return AssertCondition.NoEmpty;
            }
            else
            {
                return AssertCondition.Empty;
            }
        }

        [StepArgumentTransformation]
        public ExpectedJToken TransformToExpectation(string expectedValueExpression)
        {
            return expectedJTokenFactory.CreateExpectedJToken(expectedValueExpression);
        }

        [StepArgumentTransformation]
        public IEnumerable<IAssertableJToken> TransformToAssertableObjects(string classPathExpressions)
        {
            var assertableObjectsList = new List<IAssertableJToken>();
            foreach (var classPathExpression in classPathExpressions.ParseList())
            {
                if (scenarioContext.GetActualJsonResponse().Type == Newtonsoft.Json.Linq.JTokenType.Array)
                {
                    var tokensArray = ((JArray)scenarioContext.GetActualJsonResponse()).Children();

                    assertableObjectsList.Add(new AssertableJTokenCollection(tokensArray
                            .Select(o => o.SelectToken($"$.{classPathExpression}")), classPathExpression));
                }
                else if (scenarioContext.GetActualJsonResponse().Type == Newtonsoft.Json.Linq.JTokenType.Object)
                {
                    if (classPathExpression.Contains("->"))
                    {
                        var path = classPathExpression.Split("->")[0];
                        var selector = classPathExpression.Split("->")[1];

                        var selectedTokenArray = JTokenParser.ParseJTokenFromJObject(path, (JObject)scenarioContext.GetActualJsonResponse()).ToList();

                        var filteredCollection = new List<JToken>();
                        foreach (var token in selectedTokenArray)
                        {
                            filteredCollection.Add(JTokenParser.ParseJTokenFromJObject(selector, (JObject)token));
                        }

                        assertableObjectsList.Add(new AssertableJTokenCollection(filteredCollection, classPathExpression));
                    }
                    else
                    {
                        var targetToken = JTokenParser.ParseJTokenFromJObject(classPathExpression, (JObject)scenarioContext.GetActualJsonResponse());

                        assertableObjectsList.Add(new AssertableJToken(targetToken, classPathExpression));
                    }
                }
                else
                {
                    throw new NotImplementedException($"Тип токена ответа еще не поддерживается {scenarioContext.GetActualJsonResponse().Type}");
                }

            }
            return assertableObjectsList;
        }
    }
}

