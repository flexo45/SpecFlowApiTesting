using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Flexo.SpecFlowApiTesting.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public class SpecFlowContextUtils
    {
        private ScenarioContext _scenarioContext;
        private FeatureContext _featureContext;
        private DataBaseExtractor _dataBaseExtractor;

        public SpecFlowContextUtils(
            ScenarioContext scenarioContext,
            FeatureContext featureContext,
            DataBaseExtractor dataBaseExtractor)
        {
            _scenarioContext = scenarioContext;
            _featureContext = featureContext;
            _dataBaseExtractor = dataBaseExtractor;
        }

        private readonly string[] tableExtractorWords = { "из таблицы:", "в таблице:" };
        private readonly string[] testRequestExtractorWords = { "из запроса:", "из сигнала:", "как в запросе:", "как в сигнале:" };
        private readonly string[] testResponseExtractorWords = { "из ответа:", "из полученного сигнала:" };
        private readonly string[] specContextExtractorWords = { "из контекста:" };

        /// <summary>
        /// Примеры
        /// из таблицы: ... где: ...
        /// из таблицы: ... где: ... = из запроса: Name
        /// из таблицы: ... где: ... содержит: из контекста: Object->Name
        /// </summary>
        /// <param name="extractorExpression"></param>
        /// <returns></returns>
        public string ResolveExtractorExpression(string extractorExpression)
        {
            while (extractorExpression.Contains(testRequestExtractorWords, testResponseExtractorWords,
                specContextExtractorWords))
            {
                if (extractorExpression.Contains(testRequestExtractorWords))
                {
                    extractorExpression = ResolveSelectorFromJTokenInContext(
                        JToken.Parse(_scenarioContext.GetSimpleApiRequest()), testRequestExtractorWords,
                        extractorExpression);
                }
                else if (extractorExpression.Contains(testResponseExtractorWords))
                {
                    extractorExpression = ResolveSelectorFromJTokenInContext(_scenarioContext.GetActualJsonResponse(),
                        testResponseExtractorWords, extractorExpression);
                }
                else if (extractorExpression.Contains(specContextExtractorWords))
                {
                    extractorExpression =
                        ResolveSelectorFromJTokenInContextByTokenName(specContextExtractorWords, extractorExpression);
                }
            }

            if (extractorExpression.Contains(tableExtractorWords))
            {
                return _dataBaseExtractor.ExtractAsJsonString(extractorExpression);
            }

            return extractorExpression;
        }

        public string ResolveDataGenerationExpression(string requestContent)
        {
            var dateRegex = new Regex("дата: (.*) в формате: ([A-za-z0-9\\+.:-]+)");

            var dateMatch = dateRegex.Match(requestContent);
            while (dateMatch.Success)
            {
                var date = dateMatch.Groups[1].Value.Minus("например,").Trim();
                var dateFormat = dateMatch.Groups[2].Value.Trim();

                var dateTime = DateTimeOffset.MinValue;
                if (date.Contains("сейчас"))
                {
                    dateTime = DateTimeOffset.UtcNow;
                }

                if (date.Contains("минус:"))
                {
                    var timeSettingValue = date.ParseList("минус:")[1];
                    if (timeSettingValue.Contains("час"))
                    {
                        dateTime = dateTime.AddHours(-1 * double.Parse(timeSettingValue
                            .Minus("часа")
                            .Minus("часов")
                            .Minus("час")
                            .Trim()));
                    }
                }

                // ...

                requestContent = requestContent.Replace(dateMatch.Value, $@"""{dateTime.ToString(dateFormat)}""");

                dateMatch = dateMatch.NextMatch();
            }

            var stringMatch = new Regex("строка: (.*)").Match(requestContent);
            while (stringMatch.Success)
            {
                var fileExpressionMatch = new Regex("файл: (.*) в формате: ([A-Za-z0-9]+)").Match(stringMatch.Groups[1].Value);

                if (fileExpressionMatch.Success)
                {
                    var filePath = fileExpressionMatch.Groups[1].Value.Minus("например,").Trim();
                    var fileFormat = fileExpressionMatch.Groups[2].Value.Trim();

                    if (fileFormat.Contains("base64"))
                    {
                        var fileBytes = File.ReadAllBytes(Path.Combine(filePath.ParseList(@"\")));
                        var base64 = Convert.ToBase64String(fileBytes);
                        requestContent = requestContent
                            .Replace($"строка: {fileExpressionMatch.Value}", $@"""{base64}""");
                    }

                    // other file format here ...
                }

                var randomStringOfLengthExpressionMatch = new Regex("длинной: (\\d*)").Match(stringMatch.Groups[1].Value);

                if (randomStringOfLengthExpressionMatch.Success)
                {
                    var wordLength = randomStringOfLengthExpressionMatch.Groups[1].Value.Trim();
                    requestContent = requestContent
                        .Replace($"строка: {randomStringOfLengthExpressionMatch.Value}", $"{RandomValues.RandomInt(int.Parse(wordLength))}");
                }

                stringMatch = stringMatch.NextMatch();
            }

            var textMatch = new Regex("текст: (.*)").Match(requestContent);
            while (textMatch.Success)
            {
                var textLengthExpressionMatch = new Regex("длинной: (\\d*)").Match(textMatch.Groups[1].Value);

                if (textLengthExpressionMatch.Success)
                {
                    var wordLength = textLengthExpressionMatch.Groups[1].Value.Trim();
                    requestContent = requestContent
                        .Replace($"текст: {textLengthExpressionMatch.Value}", $"{RandomValues.RandomWords(int.Parse(wordLength))}");
                }

                textMatch = textMatch.NextMatch();
            }

            var randomNumberMatch = new Regex("#!случай[а-я]+_числ[а-я]+: *(.*)!#").Match(requestContent);
            while (randomNumberMatch.Success)
            {
                long randomValue;
                string alias = null;
                if (randomNumberMatch.Groups.Count > 0)
                {
                    var paraExpression = randomNumberMatch.Groups[1].Value;
                    var extractAlias = paraExpression.ParseList("=>");
                    alias = extractAlias.Length > 1 ? extractAlias[1] : null;
                }

                randomValue = RandomValues.RandomLong(12);

                if (alias != null) // save to context if has alias e.g. => alias
                    _scenarioContext.Set<long>(randomValue, alias);

                requestContent = requestContent
                            .Replace(randomNumberMatch.Value, randomValue.ToString());

                randomNumberMatch = randomNumberMatch.NextMatch();
            }

            var randomDataMatch = new Regex("#!дата: *(.*)!#").Match(requestContent);
            while (randomDataMatch.Success)
            {
                DateTimeOffset randomDateTime;
                string dateFormat = "";
                string alias = null;
                if (randomDataMatch.Groups.Count > 0)
                {
                    var paraExpression = randomDataMatch.Groups[1].Value;
                    var extractAlias = paraExpression.ParseList("=>");
                    alias = extractAlias.Length > 1 ? extractAlias[1] : null;
                    dateFormat = extractAlias[0];
                }

                randomDateTime = DateTimeOffset.UtcNow;

                if (alias != null) // save to context if has alias e.g. => alias
                    _scenarioContext.Set<DateTimeOffset>(randomDateTime, alias);

                requestContent = requestContent
                    .Replace(randomDataMatch.Value, randomDateTime.ToString($"{dateFormat}"));

                randomDataMatch = randomDataMatch.NextMatch();
            }

            var randomGuidMatch = new Regex("#!случай[а-я]+_guid: *(.*)!#").Match(requestContent);
            while (randomGuidMatch.Success)
            {
                Guid randomValue;
                string alias = null;
                if (randomGuidMatch.Groups.Count > 0)
                {
                    var paraExpression = randomNumberMatch.Groups[1].Value;
                    var extractAlias = paraExpression.ParseList("=>");
                    alias = extractAlias.Length > 1 ? extractAlias[1] : null;
                }

                randomValue = Guid.NewGuid();

                if (alias != null) // save to context if has alias e.g. => alias
                    _scenarioContext.Set<Guid>(randomValue, alias);

                requestContent = requestContent
                            .Replace(randomGuidMatch.Value, randomValue.ToString());

                randomGuidMatch = randomGuidMatch.NextMatch();
            }

            return requestContent;
        }

        public string ResolveSqlExtractExtension(string requestContent, bool isJsonArrayRequest, bool isJsonArrayOfObjects)
        {
            if (requestContent.Contains("#")) // Извлечь из скл таблицы
            {
                if (requestContent.Contains("->")) // Взять параметры из каждой строки 
                {
                    var extractRowSetParams = requestContent.Minus("#").ParseList("->");
                    var expectedRows = _scenarioContext.Get<IEnumerable<IDictionary<string, object>>>(extractRowSetParams[0]);
                    var newObjectProps = extractRowSetParams[1].ParseList(",");

                    if (isJsonArrayRequest)  // Если нужен массив
                    {
                        var filteredFieldsInRows = expectedRows.Select(x => x.Where(d => newObjectProps.Contains(d.Key)));
                        if (isJsonArrayOfObjects) // Если нужен массив объектов
                        {
                            var request = filteredFieldsInRows.Select(x => ToJObject(x));
                            return JsonConvert.SerializeObject(request);
                        }
                        else // Иначе соберем массива значений 
                        {
                            var request = filteredFieldsInRows.Select(x => x.FirstOrDefault().Value); // берем перове попашееся значение, остальные игнорим
                            return JsonConvert.SerializeObject(request);
                        }
                    }
                    else // Если не нужен массив, то нужно реализовать
                    {
                        throw new NotImplementedException("Если не нужен массив, то нужно реализовать");
                    }
                }
                else // Взять параметр из одной строки (предполагаем, что в ожидаемом результатет дикшнари
                {
                    var extractRowParams = requestContent.Minus("#").ParseList(".");
                    var expectedRow = _scenarioContext.Get<IDictionary<string, object>>(extractRowParams[0]);
                    return expectedRow[extractRowParams[1]].ToString();
                }
            }

            return requestContent;
        }

        /// <summary>
        /// Поддерживает только выражение типа Object.Name где в Name может быть как значение, так и массив
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string ResolveSelectorFromJsonRequest(string expression)
        {
            var selectorMatch = new Regex("как в (запросе|сигнале): ([A-Za-z0-9_.]+)").Match(expression);
            while (selectorMatch.Success)
            {
                JObject requestJson = JObject.Parse(_scenarioContext.GetSimpleApiRequest());
                var value = requestJson.SelectToken(selectorMatch.Groups[2].Value).ToString();
                expression = expression.Replace(selectorMatch.Value, value);

                selectorMatch = selectorMatch.NextMatch();
            }
            return expression;
        }

        public static void TryParseValueForKeyWord(string[] keyWords, string valuePattern, string expression, out KeyWordParseResult result)
        {
            var selectorMatch = new Regex($"({string.Join("|", keyWords)}) ({valuePattern})").Match(expression);
            if (selectorMatch.Success)
            {
                result = new KeyWordParseResult()
                {
                    KeyWord = selectorMatch.Groups[1].Value,
                    MatchValues = selectorMatch.Groups.Skip(2).Select(x => x.Value),
                    FullMatchText = selectorMatch.Value
                };
            }
            else
            {
                result = null;
            }

        }

        public static void TryParseValuesForKeyWord(string[] keyWords, string valuePattern, string expression, out IEnumerable<KeyWordParseResult> resultList)
        {
            resultList = new List<KeyWordParseResult>();
            var selectorMatch = new Regex($"({string.Join("|", keyWords)}) ({valuePattern})").Match(expression);
            while (selectorMatch.Success)
            {
                ((IList)resultList).Add(new KeyWordParseResult()
                {
                    KeyWord = selectorMatch.Groups[1].Value,
                    MatchValues = selectorMatch.Groups.Skip(1).Select(x => x.Value),
                    FullMatchText = selectorMatch.Value
                });
                expression = expression.Minus(selectorMatch.Value);
                selectorMatch.NextMatch();
            }
        }

        public string ResolveSelectorFromJTokenInContext(JToken jsonObject, string[] keyWords, string expression)
        {
            var selectorMatch = new Regex($"({string.Join("|", keyWords)}) ([A-Za-z0-9_.]+)").Match(expression);
            if (selectorMatch.Success)
            {
                if (jsonObject.Type == JTokenType.Object)
                {
                    var param = selectorMatch.Groups[2].Value;
                    var resolvedObject = param.Equals(".") ? jsonObject : jsonObject.SelectToken(param);
                    var resolvedObjectPrintValue = "";

                    if (resolvedObject == null)
                    {
                        resolvedObject = JToken.Parse("null");
                    }

                    if (resolvedObject.Type == JTokenType.Date)
                    {
                        var dateFormatString = "yyyy-MM-ddTHH:mm:ssZ";

                        var formatMatch = new Regex($" в формате: ([A-Za-z0-9_.:+-]+)").Match(expression);
                        if (formatMatch.Success)
                        {
                            dateFormatString = formatMatch.Groups[1].Value;
                            expression = expression.Replace(formatMatch.Value, "");
                        }

                        resolvedObjectPrintValue = resolvedObject.Value<DateTime>().ToString(dateFormatString);
                    }
                    else
                    {
                        if (IsInQuoted(selectorMatch, expression))
                        {
                            resolvedObjectPrintValue = resolvedObject.ToString(Formatting.None);
                            expression = expression
                                .Remove(selectorMatch.Index + selectorMatch.Length, 1)
                                .Remove(selectorMatch.Index - 1, 1);

                        }
                        else
                        {
                            resolvedObjectPrintValue = EscapeJsonChars(resolvedObject.ToString());
                        }

                    }
                    expression = expression.Replace(selectorMatch.Value, resolvedObjectPrintValue);
                }
                else
                {
                    expression = expression.Replace(selectorMatch.Value, jsonObject.ToString());
                }
            }

            return expression;
        }

        private string EscapeJsonChars(string value)
        {
            //return value;
            return value.Replace(@"""", @"\""").Replace("\r\n", @"\r\n");
        }

        private bool IsInQuoted(Match match, string expression)
        {
            var leftChar = match.Index == 0 ? expression[0] : expression[match.Index - 1];
            var rightChar = match.Index + match.Length == expression.Length ? expression[expression.Length - 1] : expression[match.Index + match.Length];
            var rest = leftChar == '`' && rightChar == '`';
            return rest;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyWords"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ResolveSelectorFromJTokenInContextByTokenName(string[] keyWords, string expression)
        {
            var selectorMatch =
                new Regex($"({string.Join("|", keyWords)}) ([A-Za-z0-9_.]+)")
                    .Match(expression);

            if (selectorMatch.Success)
            {
                var contextJTokenName = selectorMatch.Groups[2].Value.ParseList(".")[0];
                return ResolveSelectorFromJTokenInContext(
                    _scenarioContext.Get<JToken>(contextJTokenName),
                    keyWords, expression);
            }

            throw new Exception($"Найдена ссылка на контекст, но распарсить ее не удалось");
        }

        private object ToJObject(IEnumerable<KeyValuePair<string, object>> dic)
        {
            var jObject = new JObject();
            foreach (var prop in dic)
            {
                jObject.Add(prop.Key, JToken.FromObject(prop.Value));
            }
            return jObject.ToObject<object>();
        }

        public string ResolveContextReference(string strigWithContextReference)
        {
            var referensPattern = new Regex("\\$\\{([a-zA-Zа-яА-Я_]*)\\}");

            var match = referensPattern.Match(strigWithContextReference);
            while (match.Success)
            {
                var key = match.Groups[1].Value;
                string value;
                _scenarioContext.TryGetValue(key, out value);
                strigWithContextReference = strigWithContextReference.Replace("${" + key + "}", value ?? $"#{key}#NaN");
                match = match.NextMatch();
            }
            return strigWithContextReference;
        }

    }

    public class KeyWordParseResult
    {
        public string KeyWord { get; set; }
        public IEnumerable<string> MatchValues { get; set; }
        public string FullMatchText { get; set; }
    }
}
