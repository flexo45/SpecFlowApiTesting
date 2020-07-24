using System;
using System.Collections.Generic;
using System.Text;

namespace Flexo.SpecFlowApiTesting.Extensions
{
    public static class TypeExtension
    {
        public static object ExtractStaticProperty(this Type source, string property)
        {
            var staticProperyInfo = source.GetProperty(property.ToUpperCaseFirstChar());

            if (staticProperyInfo == null)
                throw new Exception($"Свойство {property} не найдено в типе {source}");

            return staticProperyInfo.GetValue(null);
        }
    }
}
