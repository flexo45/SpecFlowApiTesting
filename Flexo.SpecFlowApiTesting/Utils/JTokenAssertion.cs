using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Flexo.SpecFlowApiTesting.Entities;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using AssertionOptions = Flexo.SpecFlowApiTesting.Entities.AssertionOptions;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public static class JTokenAssertion
    {
        public static void AssertJTokenCollectionContains(IEnumerable<JToken> tokenCollection, ExpectedJToken expectedJToken, string pathName)
        {
            var collection = tokenCollection.ToList();

            switch (expectedJToken.JToken.Type)
            {
                case JTokenType.Array:
                    collection.Select(t => t.ToObject<object>())
                        .Should().Contain(expectedJToken.JToken.ToObject<IEnumerable<object>>(), because: $"В массиве {pathName} ожидалось, что содержится подмножество");
                    break;
                default:
                    for (var i = 0; i < collection.Count(); i++)
                    {
                        AssertJTokenContains(collection.ElementAt(i), expectedJToken, pathName);
                    }
                    break;
            }
        }

        public static void AssertJTokenContains(JToken token, ExpectedJToken expectedJToken, string pathName)
        {
            switch (expectedJToken.JToken.Type)
            {
                case JTokenType.Array:
                    token.Type.Should().Be(JTokenType.Array, $"В {pathName} ожидаелся массив");

                    token.ToObject<IEnumerable<object>>().Should()
                        .Contain(expectedJToken.JToken.ToObject<IEnumerable<object>>(), because: $"В массиве {pathName} ожидалось, что содержится подмножество");
                    break;
                case JTokenType.String:
                    token.ToObject<object>()
                        .ToString().Should().Contain(expectedJToken.JToken.ToObject<string>(), $"В {pathName} ожидалась, что содержится строка");
                    break;
                default:
                    if (token.Type == JTokenType.Array)
                    {
                        token.ToObject<IEnumerable<object>>().Should().Contain(expectedJToken.TokenObject, $"В массиве {pathName} ожидалось, что содержится объект");
                    }
                    else
                    {
                        throw new Exception($"Неподдерживаемый тип токена {expectedJToken.JToken.Type}");
                    }
                    break;

            }
        }

        public static void AssertJTokenCollectionEquals(IEnumerable<JToken> tokenCollection, ExpectedJToken expectedJToken, string pathName, params AssertionOptions[] options)
        {
            switch (expectedJToken.JToken.Type)
            {
                case JTokenType.Array:
                    var expectedCollection = expectedJToken.JToken.ToObject<IEnumerable<object>>();
                    if (options.Contains(AssertionOptions.AssertOrder))
                    {
                        tokenCollection.Select(x => x.ToObject<object>()).Should().Equal(expectedCollection,
                            $"Ошибка в составе массива {pathName}, с фиксированной сортировкой");
                    }
                    else
                    {
                        tokenCollection.Select(x => x.ToObject<object>()).Should().BeEquivalentTo(expectedCollection,
                            $"Ошибка в составе массива {pathName}");
                    }
                    break;
                default:
                    tokenCollection.Select(x => x.ToObject<object>())
                        .Should().OnlyContain(x => x.Equals(expectedJToken.TokenObject),
                            $"Ошибка в составе массива {pathName}, все значения должны быть одинаковыми");
                    break;
            }
        }

        public static void AssertJTokenEquals(JToken token, ExpectedJToken expectedJToken, string pathName, params AssertionOptions[] options)
        {
            switch (expectedJToken.JToken.Type)
            {
                case JTokenType.Array:
                    token.Type.Should().Be(JTokenType.Array, $"В {pathName} ожидаелся массив");

                    token.ToObject<IEnumerable<object>>().Should()
                        .Equal(expectedJToken.JToken.ToObject<IEnumerable<object>>(), because: $"Ошибка в составе массива {pathName}");
                    break;
                case JTokenType.Null:
                    token.ToObject<object>().Should().BeNull($"В {pathName} ожидался null");
                    break;
                default:

                    if (expectedJToken.JToken.Type == JTokenType.String && expectedJToken.JToken.ToObject<string>().ToLower().Equals("не null"))
                    {
                        token.ToObject<object>().Should().NotBeNull($"В {pathName} ожидался не null");
                    }

                    if (token.Type == JTokenType.Array)
                    {
                        token.ToObject<IEnumerable<object>>().Should().OnlyContain(
                            x => x.Equals(expectedJToken.TokenObject),
                            $"Ошибка в составе массива {pathName}, все значения должны быть одинаковыми");
                    }
                    token.ToObject<object>().Should().Be(expectedJToken.TokenObject, $"Ошибка в значении {pathName}");
                    break;
            }
        }

        public static void AssertJTokenCondition(JToken token, AssertCondition condition, string pathName)
        {
            switch (condition)
            {
                case AssertCondition.Empty:
                    switch (token.Type)
                    {
                        case JTokenType.Array:
                            token.Values<object>().Should().BeEmpty($"{pathName} должен быть пустым массивом");
                            break;
                        case JTokenType.String:
                            token.Value<string>().Should().BeEmpty($"Строка {pathName} должна быть пустой");
                            break;
                    }

                    break;
                case AssertCondition.NoEmpty:
                    switch (token.Type)
                    {
                        case JTokenType.Array:
                            token.Values<object>().Should().NotBeEmpty($"{pathName} не должен быть пустым массивом");
                            break;
                        case JTokenType.String:
                            token.Value<string>().Should().NotBeEmpty($"Строка {pathName} не должна быть пустой");
                            break;
                    }

                    break;
            }
        }
    }
}
