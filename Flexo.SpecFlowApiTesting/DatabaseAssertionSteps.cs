using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Fixture;
using Flexo.SpecFlowApiTesting.Utils;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting
{
    [Binding]
    class DatabaseAssertionSteps
    {
        private ScenarioContext scenarioContext;
        private SpecFlowContextUtils specFlowContextUtils;
        private SimpleDataBaseFixture dataBaseFixture;
        ExpectedJTokenFactory expectedJTokenFactory;

        public DatabaseAssertionSteps(
            ExpectedJTokenFactory expectedJTokenFactory,
            SimpleDataBaseFixture dataBaseFixture,
            ScenarioContext scenarioContext,
            SpecFlowContextUtils specFlowContextUtils)
        {
            this.expectedJTokenFactory = expectedJTokenFactory;
            this.dataBaseFixture = dataBaseFixture;
            this.scenarioContext = scenarioContext;
            this.specFlowContextUtils = specFlowContextUtils;
        }

        [Then(@".* ""(.*)"" в таблице ""(.*)"" где ""(.*)"" долж[а-я]+ быть ""(.*)""")]
        public void ТоВПолученномОтветеИмяДолжноБыть(string field, string table, string where, ExpectedJToken expectation)
        {
            var assertableValueExpression = $"из таблицы: {table}.{field} где: {where}";

            var assertableValue = specFlowContextUtils.ResolveExtractorExpression(assertableValueExpression);

            JToken expectedToken = JToken.Parse(assertableValue);

            var assertableToken = new AssertableJToken(expectedToken, $"{table}.{field}");

            assertableToken.AssertEquals(expectation);
        }

        [Then(@"ждем ""(.*)"" пока .*")]
        public void ЕслиЖдемПокаОбработаетсяДубль(string timeString)
        {
            var timeNumString = timeString.ParseList(" ")[0];
            var timeMilliseconds = 0;
            if (timeString.Contains("мили"))
            {
                timeMilliseconds = int.Parse(timeNumString);
            }
            else if (timeString.Contains("сек"))
            {
                timeMilliseconds = int.Parse(timeNumString) * 1000;
            }
            Thread.Sleep(timeMilliseconds);
        }

    }
}
