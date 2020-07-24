﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:3.1.0.0
//      SpecFlow Generator Version:3.1.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace Flexo.SpecFlowApiTesting.Examples
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public partial class ПолучениеДанныхОFeature : object, Xunit.IClassFixture<ПолучениеДанныхОFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = ((string[])(null));
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "SpecFlowFeature1.feature"
#line hidden
        
        public ПолучениеДанныхОFeature(ПолучениеДанныхОFeature.FixtureData fixtureData, Flexo_SpecFlowApiTesting_Examples_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("ru-RU"), "Получение данных о /", null, ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 3
#line hidden
#line 4
    testRunner.Given("сервис расположен по адресу \"http://rest.test.hostname/api/service\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Пусть ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Запросить всю информацию о объекте по Id")]
        [Xunit.TraitAttribute("FeatureTitle", "Получение данных о /")]
        [Xunit.TraitAttribute("Description", "Запросить всю информацию о объекте по Id")]
        public virtual void ЗапроситьВсюИнформациюООбъектеПоId()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Запросить всю информацию о объекте по Id", null, ((string[])(null)));
#line 7
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 8
 testRunner.When("запрашиваем метод GET \"\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Когда ");
#line hidden
#line 9
 testRunner.Then("должен вернуться успешный ответ со статус кодом \"OK\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "То ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableFactAttribute(DisplayName="Получить основную информацию о СМИ по Id")]
        [Xunit.TraitAttribute("FeatureTitle", "Получение данных о /")]
        [Xunit.TraitAttribute("Description", "Получить основную информацию о СМИ по Id")]
        public virtual void ПолучитьОсновнуюИнформациюОСМИПоId()
        {
            string[] tagsOfScenario = ((string[])(null));
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Получить основную информацию о СМИ по Id", "\tИмя, Описание, Уровень, Охват, Тип, Степерь доверия, Ссылку на иконку", ((string[])(null)));
#line 11
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 3
this.FeatureBackground();
#line hidden
#line 13
 testRunner.When("запрашиваем метод GET \"MassMedia/71\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Когда ");
#line hidden
#line 14
 testRunner.Then("в полученном ответе Имя \"name\" должно быть \"\'Российская газета - сайт\'\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "То ");
#line hidden
#line 15
 testRunner.And("Описание \"description\" должно быть \"из таблицы: CatDb#Smis.Description где: Id = " +
                        "71\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
#line 16
 testRunner.And("Уровень \"level\" должен быть \"\'Federal\'\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
#line 17
 testRunner.And("Охват \"audience\" должен быть \"950\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
#line 18
 testRunner.And("Тип \"massMediaType\" должен быть \"\'Blog\'\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
#line 19
 testRunner.But("тип Источника \"massMediaSource\" должен быть \"\'Internet\'\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "А ");
#line hidden
#line 20
 testRunner.And("СМИ недолжно быть Доверенным \"isReliable\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
#line 21
 testRunner.And("ссылки на Иконку \"iconUri, internalIconUri\" должна содержать \"\'/images/s002/SmiIc" +
                        "ons/2612f5e1-71ee-4b6d-9192-a651dfaa772e.ico\'\"", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "И ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.1.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                ПолучениеДанныхОFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                ПолучениеДанныхОFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
