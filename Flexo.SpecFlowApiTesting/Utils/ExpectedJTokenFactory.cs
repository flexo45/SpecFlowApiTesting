using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Flexo.SpecFlowApiTesting.Entities;
using Flexo.SpecFlowApiTesting.Extensions;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Utils
{
    public class ExpectedJTokenFactory
    {
        private readonly SpecFlowContextUtils specFlowContextUtils;

        public ExpectedJTokenFactory(SpecFlowContextUtils specFlowContextUtils)
        {
            this.specFlowContextUtils = specFlowContextUtils;
        }

        public ExpectedJToken CreateExpectedJToken(string expectedExpressionString)
        {
            expectedExpressionString = specFlowContextUtils.ResolveDataGenerationExpression(expectedExpressionString);

            expectedExpressionString = specFlowContextUtils.ResolveExtractorExpression(expectedExpressionString);

            JToken expectedToken = JToken.Parse(expectedExpressionString);

            return new ExpectedJToken(expectedToken);

        }
    }
}
