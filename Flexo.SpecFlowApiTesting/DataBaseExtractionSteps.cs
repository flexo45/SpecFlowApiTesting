using System;
using System.Collections.Generic;
using System.Text;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Fixture;
using Flexo.SpecFlowApiTesting.Utils;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting
{
    [Binding]
    public class DataBaseExtractionSteps
    {
        private ScenarioContext scenarioContext;
        private SpecFlowContextUtils specFlowContextUtils;
        private SimpleDataBaseFixture dataBaseFixture;

        public DataBaseExtractionSteps(
            ScenarioContext scenarioContext,
            SimpleDataBaseFixture dataBaseFixture,
            SpecFlowContextUtils specFlowContextUtils)
        {
            this.scenarioContext = scenarioContext;
            this.specFlowContextUtils = specFlowContextUtils;
            this.dataBaseFixture = dataBaseFixture;
        }

        [Given(@"""(.*)"" .* [А-Я][А-Яа-я]+ ""(.*)""")]
        public void ДопустимВКонтекстТокенИзИсточника(string contextVariableName, ExpectedJToken extractedToken)
        {
            scenarioContext.Set<JToken>(extractedToken.JToken, contextVariableName);
        }
    }
}
