using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Flexo.SpecFlowApiTesting.Fixture;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public class DataBaseExtractor
    {
        private SimpleDataBaseFixture dataBaseFixture;
        public DataBaseExtractor(SimpleDataBaseFixture dataBaseFixture)
        {
            this.dataBaseFixture = dataBaseFixture;
        }

        private static readonly string[] WhereKeyWords = { "где:" };

        private static readonly string[] InKeyWords = { "включает:", "включают:" };

        private static readonly string[] JoinKeyWords =
            {"в связанной таблице:", "в таблице-связке:", "а в связанной таблице:", "а в таблице-связке:" };

        private static readonly string[] JoinOnKeyWords = { "по:" };

        private static readonly string[] JoinWhereKeyWords = { "поле:", "поля:" };

        private static readonly string[] SelectTop = { "первые:", "первых:" };

        private static readonly string[] OrderByDescWords = { "по убыванию:" };

        private static readonly string[] OrderByAscWords = { "по возрастанию:" };

        protected class SqlBuilder
        {
            private string dataBaseQueryExpression;
            public SqlBuilder(string dataBaseQueryExpression)
            {
                this.dataBaseQueryExpression = dataBaseQueryExpression;
            }

            public SqlBuilder Build()
            {
                var selectFromExpression = dataBaseQueryExpression
                    .ParseList(WhereKeyWords)[0];

                var selectTableMatch = new Regex("([A-Za-z0-9_#]+)\\.([A-Za-z0-9*]+)").Match(selectFromExpression);

                SelectedField = selectTableMatch.Groups[2].Value;

                Select = ParseSelectSection(SelectedField, selectFromExpression);

                DataBaseName = ParseDataBaseName(selectTableMatch.Groups[1].Value);

                From = ParseFromTableName(selectTableMatch.Groups[1].Value);

                if (dataBaseQueryExpression.Contains(OrderByAscWords, OrderByDescWords))
                {
                    OrderBy = ParseOrderBySection(dataBaseQueryExpression);
                }

                if (dataBaseQueryExpression.Contains(WhereKeyWords))
                {
                    var mainWhereExpression = dataBaseQueryExpression
                        .ParseList(WhereKeyWords)[1]?
                        .ParseList(JoinKeyWords)[0]
                        .ParseList(OrderByAscWords.Concat(OrderByDescWords).ToArray())[0];

                    Where = ParseWhereSection(mainWhereExpression);
                }

                if (dataBaseQueryExpression.Contains(JoinKeyWords))
                {
                    var joinWhereExpression = dataBaseQueryExpression
                        .ParseList(WhereKeyWords)[1]?
                        .ParseList(JoinKeyWords)[1];

                    ParseJoinSection(joinWhereExpression);
                }

                return this;
            }

            public string GetQuery()
            {
                return $"SELECT {Select} " +
                       $"FROM {From} " +
                       $"{(Where == null ? "" : $"WHERE {Where}")}" +
                       $"{OrderBy ?? ""}";
            }

            private string Select { get; set; }
            private string From { get; set; }
            private string Where { get; set; }

            private string OrderBy { get; set; }
            public string DataBaseName { get; private set; }
            public bool IsSingleQuery { get; private set; }
            public string JsonPathSelection { get; private set; }

            public string SelectedField { get; private set; }

            /// <summary>
            /// Sections.Name где: SmiId=71
            /// Sections.* где: SmiId=71
            /// список всех: Sections.* где: SmiId=71
            /// список всех: Sections.IsMain где: SmiId=71
            /// количество: Sections.IsMain где: SmiId=71
            /// первые: 100 Sections.IsMain где: SmiId=71
            /// в объекте: Smis.DynamicAttributes->RknTerritoryOfDistribution где: Smis.Id=71
            /// </summary>
            /// <param name="selectFromExpression"></param>
            /// <returns></returns>
            private string ParseSelectSection(string selectExpression, string selectFromExpression)
            {
                SpecFlowContextUtils.TryParseValueForKeyWord(new[] { "в объекте:", "из объекта:" },
                    "([A-Za-z0-9_.#]+)->([A-Za-z0-9]+)", selectFromExpression,
                    out var jsonPathResult);

                if (jsonPathResult != null && jsonPathResult.MatchValues.Any())
                {
                    JsonPathSelection = $"$.{jsonPathResult.MatchValues.ElementAt(2) }";
                    selectFromExpression = selectExpression.Replace(jsonPathResult.FullMatchText,
                        jsonPathResult.MatchValues.ElementAt(1));
                }

                if (selectFromExpression.Contains(new[] { "список:", "список всех:", "списком:" }))
                {
                    IsSingleQuery = false;

                    SpecFlowContextUtils.TryParseValueForKeyWord(SelectTop, "[0-9]+", selectFromExpression,
                        out var selectTopResult);

                    if (selectTopResult != null)
                    {
                        return $"TOP({selectTopResult.MatchValues.FirstOrDefault()}) {selectExpression}";
                    }

                    return $"TOP(1000) {selectExpression}";
                }

                IsSingleQuery = true;

                if (selectFromExpression.Contains("количество:"))
                {
                    return $"COUNT({selectExpression})";
                }

                return $"TOP(1) {selectExpression}";

            }

            private static string ParseDataBaseName(string fromExpression)
            {
                return fromExpression.Contains("#") ? fromExpression.ParseList("#")[0] : "default";
            }

            private static string ParseFromTableName(string fromExpression)
            {
                return fromExpression.Contains("#") ? fromExpression.ParseList("#")[1] : fromExpression;
            }

            /// <summary>
            /// где: а = 1 и: б = 2
            /// где: (а = 1 и: б = 2) или: б = 3
            /// где: а = 2 или: б включает: [ 1, 2, 3]
            /// где: SmiSubjectInSmis.SmisId=71 # тут неявный, если table_A не равен tabke_B, то считаем что table_A.Id == table_B.tavle_A'без s'Id
            /// где: в таблице-связке: SmiSubjectInSmis по: Id=SmiSubjectId поле: SmisId=71
            /// где: Id=3
            /// где: CreationDate период: с 2019-09-21 по 2019-09-31
            /// где: CreationDate последние: 3 дня
            /// где: в связанной таблице: Headlines по: StructuredTextId поле: PublicationId=41		
            /// где: в связанной таблице: Headlines по: MessageId=Id поле: PublicationId=41
            /// где: CreationDate последние: 3 дня"
            /// где: SmiSubjectInSmis.SmisId=71" # тут неявный, если table_A не равен tabke_B, то считаем что table_A.Id == table_B.tavle_A'без s'Id
            /// где: VersionNumber=1 а в связанной таблице: Headlines по: StructuredTextId поле: PublicationId='<publication_id>'
            /// </summary>
            /// <param name="dataBaseQueryExpression"></param>
            /// <returns></returns>
            private static string ParseWhereSection(string whereExpression)
            {
                whereExpression = whereExpression.Replace("или:", "OR").Replace("и:", "AND");

                SpecFlowContextUtils.TryParseValuesForKeyWord(InKeyWords, "[A-Za-z0-9_.\\->\\[\\] ,]+", whereExpression,
                    out var inResult);


                whereExpression = inResult.Aggregate(whereExpression,
                    (current, x) =>
                        current.Replace(x.FullMatchText, ToSqlInExpression(x.MatchValues.FirstOrDefault())));


                SpecFlowContextUtils.TryParseValuesForKeyWord(new[] { "период:" }, "период: с ([\\d\\-]+) по ([\\d\\-]+)",
                    whereExpression, out var betweenDatetimeResult);
                whereExpression = betweenDatetimeResult.Aggregate(whereExpression,
                    (current, x) => current.Replace(x.FullMatchText,
                        $"BETWEEN CONVERT(DATETIMEOFFSET, '{x.MatchValues.ElementAt(0)}', 127) AND CONVERT(DATETIMEOFFSET, '{x.MatchValues.ElementAt(1)}', 127)"));


                SpecFlowContextUtils.TryParseValuesForKeyWord(new[] { "послдение:", "последних:" }, "(\\d+) ([а-я]+)",
                    whereExpression, out var lastDatetimeResult);
                whereExpression = lastDatetimeResult.Aggregate(whereExpression,
                    (current, x) => current.Replace(x.FullMatchText,
                        $">= GETDATE() - {x.MatchValues.ElementAt(0)}")); // TODO дни, часы, месяцы

                return whereExpression;
            }

            private static string ParseOrderBySection(string orderByExpression)
            {
                SpecFlowContextUtils.TryParseValueForKeyWord(OrderByAscWords.Concat(OrderByDescWords).ToArray(), "[A-Za-z0-9]+", orderByExpression, out var orderByResult);
                if (orderByResult.KeyWord.Contains(OrderByAscWords))
                {
                    return $"ORDER BY {orderByResult.MatchValues.FirstOrDefault()} asc";
                }

                return $"ORDER BY {orderByResult.MatchValues.FirstOrDefault()} desc";
            }

            /// <summary>
            /// Headlines по: StructuredTextId поле: PublicationId=41"		
            /// Headlines по: MessageId=Id поле: PublicationId=41"	
            /// SmiSubjectInSmis.SmisId=71" # тут неявный, если table_A не равен tabke_B, то считаем что table_A.Id == table_B.tavle_A'без s'Id
            /// Headlines по: StructuredTextId поле: PublicationId='<publication_id>'"
            /// </summary>
            /// <param name="from"></param>
            /// <param name="where"></param>
            /// <param name="joinExpression"></param>
            /// <returns></returns>
            private void ParseJoinSection(string joinExpression)
            {
                var joinTableName = joinExpression.ParseList(JoinOnKeyWords)[0];
                SpecFlowContextUtils.TryParseValueForKeyWord(JoinOnKeyWords, "[A-Za-z0-9_.=]+", joinExpression,
                    out var joinOnResult);
                SpecFlowContextUtils.TryParseValueForKeyWord(JoinWhereKeyWords, ".+$", joinExpression,
                    out var joinWhereResult);

                var joinOnParams = joinOnResult.MatchValues.FirstOrDefault().ParseList("=");

                From =
                    $"{From} t1 JOIN {joinTableName} t2 ON t1.{(joinOnParams.Length > 1 ? joinOnParams[1] : "Id")} = t2.{joinOnParams[0]}";

                Where = $"{(Where == String.Empty ? "" : $"t1.{Where} AND ")}t2.{joinWhereResult.MatchValues.FirstOrDefault()}";
                //TODO добавдение t1 и t2 ко всем параметрам поиска, пока поддерживаетс по одному условию

                Select = Select.Replace(SelectedField, $"t1.{SelectedField}");

            }

            private static string ToSqlInExpression(string jsonArrayString)
            {
                return jsonArrayString
                    .Replace("[", "(")
                    .Replace("]", ")")
                    .Replace("\"", "'");
            }
        }

        /// <summary>
        /// в таблице: список: CatDb#Sections.Id где: SmiId=71
        /// в таблице: список: Sections.Name где: SmiId=71
        /// в таблице: список: Sections.Url где: SmiId=71
        /// в таблице: список: Sections.IsMain где: SmiId=71
        /// в таблице: список: в объекте: Smis.DynamicAttributes->RknTerritoryOfDistribution где: Smis.Id=71
        /// в таблице: список: SmiSubjects.Id где: SmiSubjectInSmis.SmisId=71 # тут неявный, если table_A не равен tabke_B, то считаем что table_A.Id == table_B.tavle_A'без s'Id
        /// в таблице: список: SmiSubjects.Id где: в таблице-связке: SmiSubjectInSmis по: Id=SmiSubjectId поле: SmisId=71
        /// в таблице: список: Headlines.PublicationId где: PublicationId включает: из контекста: Publications.Headlines->PublicationId - такого не должно быть
        /// в таблице: список: Headlines.PublicationId где: PublicationId включает: [ 123, 432, "3124" ]
        /// </summary>
        /// <param name="dataBaseQueryExpression"></param>
        /// <returns></returns>
        public ExpectedJToken ExtractAsExpectedJToken(string dataBaseQueryExpression)
        {
            var sqlQuery = new SqlBuilder(dataBaseQueryExpression).Build();

            if (sqlQuery.IsSingleQuery)
            {
                var dataSet = dataBaseFixture.Select(sqlQuery.GetQuery(), sqlQuery.DataBaseName);
                var tableValue = dataSet[sqlQuery.SelectedField];

                if (sqlQuery.JsonPathSelection != null)
                {
                    var json = JObject.Parse(tableValue.ToString());
                    var tableValueJToken = json.SelectToken(sqlQuery.JsonPathSelection);
                    return new ExpectedJToken(tableValueJToken);
                }
                return new ExpectedJToken(JToken.Parse(tableValue.ToString()));
            }

            var tableDataSet = dataBaseFixture.SelectMany(sqlQuery.GetQuery(), sqlQuery.DataBaseName);
            var tableValues = tableDataSet.Select(x => ((IDictionary<string, object>)x)[sqlQuery.SelectedField]);
            if (sqlQuery.JsonPathSelection != null)
            {
                var jsonList = tableValues.Select(x => JObject.Parse(x.ToString()));
                var tableValueJTokenList = jsonList.Select(x => x.SelectToken(sqlQuery.JsonPathSelection));
                var tableValueJTokensAsJArray = JArray.Parse(JsonConvert.SerializeObject(tableValueJTokenList));
                return new ExpectedJToken(tableValueJTokensAsJArray);
            }
            var tableValuesJArray = JArray.Parse(JsonConvert.SerializeObject(tableValues));
            return new ExpectedJToken(tableValuesJArray);
        }

        public string ExtractAsJsonString(string dataBaseQueryExpression)
        {
            var sqlQuery = new SqlBuilder(dataBaseQueryExpression).Build();

            if (sqlQuery.IsSingleQuery)
            {
                var dataSet = dataBaseFixture.Select(sqlQuery.GetQuery(), sqlQuery.DataBaseName);
                var tableValue = dataSet[sqlQuery.SelectedField];

                if (sqlQuery.JsonPathSelection != null)
                {
                    var json = JObject.Parse(tableValue.ToString());
                    var tableValueJToken = json.SelectToken(sqlQuery.JsonPathSelection);
                    return tableValueJToken.ToString();
                }
                return toJsonString(tableValue);
            }

            var tableDataSet = dataBaseFixture.SelectMany(sqlQuery.GetQuery(), sqlQuery.DataBaseName);
            var tableValues = tableDataSet.Select(x => ((IDictionary<string, object>)x)[sqlQuery.SelectedField]);
            if (sqlQuery.JsonPathSelection != null)
            {
                var jsonList = tableValues.Select(x => JObject.Parse(x.ToString()));
                var tableValueJTokenList = jsonList.Select(x => x.SelectToken(sqlQuery.JsonPathSelection));
                return JsonConvert.SerializeObject(tableValueJTokenList);
            }

            return JsonConvert.SerializeObject(tableValues);
        }

        private string toJsonString(object value)
        {
            if (value.GetType() == typeof(String))
            {
                return $"'{value}'";
            }

            if (value.GetType() == typeof(Guid))
            {
                return $"'{value}'";
            }
            return value.ToString();
        }
    }
}
