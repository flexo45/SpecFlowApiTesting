using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Flexo.SpecFlowApiTesting.Extensions
{
    public static class StringExtension
    {
        public static bool ContainsRegex(this string source, string value)
        {
            var normalized = source.Contains("?") ? source.Split("?")[0] : source;

            if (value.Contains("*"))
            {
                var pattern = new Regex(value.Replace("*", "[A-Za-z0-9_-]+") + "$");
                var match = pattern.Match(normalized);
                return match.Success;
            }
            else
            {
                return normalized.Equals(value);
            }
        }

        public static bool Contains(this string source, params string[] values)
        {
            foreach (var v in values)
            {
                var result = source.Contains(v);
                if (result)
                    return true;
            }

            return false;
        }

        public static bool Contains(this string source, params string[][] values)
        {
            var singleList = new List<string>();
            foreach (var v in values)
            {
                singleList.AddRange(v);
            }

            return source.Contains(singleList.ToArray());
        }

        public static string ToLowerCaseFirstChar(this string source)
        {
            if (source != string.Empty && char.IsUpper(source[0]))
            {
                source = char.ToLower(source[0]) + source.Substring(1);
            }
            return source;
        }

        public static string ToUpperCaseFirstChar(this string source)
        {
            if (source != string.Empty && char.IsLower(source[0]))
            {
                source = char.ToUpper(source[0]) + source.Substring(1);
            }
            return source;
        }

        public static int ParseInteger(this string source)
        {
            return int.Parse(source);
        }

        public static string[] ParseList(this string source, string separator = ",")
        {
            return source.Split(separator).Select(x => x.Trim()).ToArray();
        }

        public static string[] ParseList(this string source, string[] separators)
        {
            var possibleSeparator = separators.FirstOrDefault(x => source.Contains((string)x));
            return source.ParseList(possibleSeparator);
        }

        public static bool ContainsAndReplase(this string source, string value)
        {
            var isContain = source.Contains(value);
            if (isContain)
                source = source.Minus(value);

            return isContain;
        }

        public static string Minus(this string source, params string[] textParams)
        {
            var result = source;
            foreach (var text in textParams)
            {
                result = result.Replace(text, "");
            }
            return result;
        }
    }
}
