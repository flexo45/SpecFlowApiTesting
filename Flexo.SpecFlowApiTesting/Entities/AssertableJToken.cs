using System;
using System.Collections.Generic;
using System.Text;
using Flexo.SpecFlowApiTesting.Utils;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Entities
{
    public class AssertableJToken : AbstractAssertableJToken
    {
        public AssertableJToken(JToken token, string pathName) : base(pathName)
        {
            Token = token;
        }

        public JToken Token { get; }

        public override void AssertContains(ExpectedJToken expectation)
        {
            JTokenAssertion.AssertJTokenContains(Token, expectation, PathName);
        }

        public override void AssertCondition(AssertCondition condition)
        {
            JTokenAssertion.AssertJTokenCondition(Token, condition, PathName);
        }

        public override void AssertEquals(ExpectedJToken expectation, params AssertionOptions[] options)
        {
            JTokenAssertion.AssertJTokenEquals(Token, expectation, PathName, options);
        }
    }
}
