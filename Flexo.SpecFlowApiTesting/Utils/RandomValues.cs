using System;
using System.Collections.Generic;
using System.Text;
using NLipsum.Core;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public static class RandomValues
    {
        private static Random _random = new Random();

        public static string RandomWords(int count)
        {
            return string.Join(" ", new LipsumGenerator().GenerateWords(count));
        }

        public static long RandomLong(int length = 6)
        {
            var result = "";
            for (var i = 0; i < length; i++)
            {
                result += _random.Next(9);
            }

            return long.Parse(result);
        }

        public static int RandomInt(int length = 6)
        {
            return (int)RandomLong(length);
        }

        public static decimal RandomDecimal(int length = 3)
        {
            return (decimal)_random.NextDouble() * RandomLong(length);
        }

        private static string RandomNumericString(int length)
        {
            var result = "";
            for (var i = 0; i < length; i++)
            {
                result += new Random().Next(9);
            }
            return result;
        }
    }
}
