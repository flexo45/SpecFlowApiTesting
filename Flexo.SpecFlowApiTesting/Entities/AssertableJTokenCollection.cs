using System;
using System.Collections.Generic;
using System.Text;
using Flexo.SpecFlowApiTesting.Utils;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Entities
{
    public class AssertableJTokenCollection : AbstractAssertableJToken
    {

        public AssertableJTokenCollection(IEnumerable<JToken> tokenCollection, string pathName) : base(pathName)
        {
            TokenCollection = tokenCollection;
        }
        public IEnumerable<JToken> TokenCollection { get; }

        public override void AssertContains(ExpectedJToken expectation)
        {
            JTokenAssertion.AssertJTokenCollectionContains(TokenCollection, expectation, PathName);
        }

        public override void AssertCondition(AssertCondition condition)
        {
            foreach (var token in TokenCollection)
            {
                JTokenAssertion.AssertJTokenCondition(token, condition, PathName);
            }
        }

        public override void AssertEquals(ExpectedJToken expectation, params AssertionOptions[] options)
        {
            JTokenAssertion.AssertJTokenCollectionEquals(TokenCollection, expectation, PathName, options);
        }
    }
}
