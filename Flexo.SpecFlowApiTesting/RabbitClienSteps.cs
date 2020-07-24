using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Fixture;
using Flexo.SpecFlowApiTesting.Utils;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting
{
    [Binding]
    class RabbitClienSteps
    {
        private ScenarioContext scenarioContext;
        private SpecFlowContextUtils specFlowContextUtils;
        private RabbitMqClientFixture rabbitMqClientFixture;

        public RabbitClienSteps(
            ScenarioContext scenarioContext,
            SpecFlowContextUtils specFlowContextUtils,
            RabbitMqClientFixture rabbitMqClientFixture)
        {
            this.scenarioContext = scenarioContext;
            this.specFlowContextUtils = specFlowContextUtils;
            this.rabbitMqClientFixture = rabbitMqClientFixture;
        }

        [Given(@"сервисная шина данных развернута по адресу ""(.*)""")]
        public void ДопустимСервиснаяШинаДанныхРазвернутаПоАдресу(string connectionString)
        {
            rabbitMqClientFixture.StartClient(connectionString);
        }

        [Given(@"ожидаем сигналы ""(.*)"" для проверки из ""(.*)""")]
        public void ДопустимОжидаемСигналыДляПроверкиИз(string signalName, string exchangeName)
        {
            rabbitMqClientFixture.SubscribeToExchange(signalName, exchangeName);
        }

        [When(@"отправляем сигнал в ""(.*)""[А-Яа-я0-9 ]*")]
        public void ЕслиОтправляемСигналВ(string exchangeName, string multilineText)
        {
            multilineText = specFlowContextUtils.ResolveDataGenerationExpression(multilineText);
            multilineText = specFlowContextUtils.ResolveExtractorExpression(multilineText);
            rabbitMqClientFixture.PublishToExchange(exchangeName, multilineText);
            scenarioContext.SetSimpleApiRequest(multilineText);
        }

        [Then(@"должен быть получен сигнал ""(.*)"" с ""(.*)""")]
        public void ТоДолженБытьПолученСигналС(string signalName, string expression)
        {
            var expressionList = expression
                .ParseList()
                .Select(x => x = specFlowContextUtils.ResolveDataGenerationExpression(x))
                .Select(x => x = specFlowContextUtils.ResolveExtractorExpression(x))
                .ToArray();

            var signal = rabbitMqClientFixture.WaitForSignal(signalName, expressionList);
            signal.Should().NotBeNull($"сигнал {signalName} c {expression} не был получен");
            scenarioContext.SetActualJsonResponse(signal);
        }


    }
}
