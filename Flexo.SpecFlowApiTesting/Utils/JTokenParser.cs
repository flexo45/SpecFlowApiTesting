using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Flexo.SpecFlowApiTesting.Extensions;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public static class JTokenParser
    {
        public static JToken ParseJTokenFromJObject(string classPathExpression, JObject source)
        {
            JToken targetToken = source;
            foreach (var fieldPathExpression in classPathExpression.ParseList("."))
            {
                var fieldPathExpressionParamsMatch = new Regex("\\[(.*)\\]").Match(fieldPathExpression);

                if (targetToken == null)
                    throw new Exception($"В json-объекте по пути {classPathExpression} нет объекта перед {fieldPathExpression}");

                if (fieldPathExpressionParamsMatch.Success)
                {
                    var fieldPathParams = fieldPathExpressionParamsMatch.Groups[1].Value.Trim();

                    if (fieldPathParams.Contains("=")) //найдено условие
                    {
                        var fieldPathParamsKeyValue = fieldPathParams.ParseList("=");
                        var targetTokens = targetToken.SelectToken(fieldPathExpression.Minus(fieldPathExpressionParamsMatch.Value)).ToList();

                        if (targetTokens == null)
                            throw new Exception($"В json-объекте нет пути {classPathExpression}");

                        targetToken = targetTokens.FirstOrDefault(x => func(x, fieldPathParamsKeyValue));
                    }
                    else //считаем что это индекс [5]
                    {
                        var targetTokens = targetToken.SelectToken(fieldPathExpression.Minus(fieldPathExpressionParamsMatch.Value)).ToList();

                        if (targetTokens == null)
                            throw new Exception($"В json-объекте нет пути {classPathExpression}");

                        var idx = int.Parse(fieldPathParams.Trim());

                        try
                        {
                            targetToken = targetTokens.ElementAt(idx);
                        }
                        catch (Exception e)
                        {
                            throw new Exception($"В массиве {targetToken} по {classPathExpression} нет объекта с индексом {idx}");
                        }

                    }

                }
                else
                {
                    targetToken = targetToken.SelectToken(fieldPathExpression);

                    if (targetToken == null)
                        throw new Exception($"В json-объекте нет пути {classPathExpression}");

                }
            }

            return targetToken;
        }

        private static bool func(JToken token, string[] param)
        {
            var key = token[param[0]];
            var val = key.ToString();
            var exp = param[1];
            var res = val.Equals(exp);
            return res;
        }
    }
}
