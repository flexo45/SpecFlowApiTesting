using System;
using System.Collections.Generic;
using System.Text;

namespace Flexo.SpecFlowApiTesting.Entities
{
    public interface IAssertableJToken
    {
        void AssertEquals(ExpectedJToken expectation, params AssertionOptions[] options);

        void AssertContains(ExpectedJToken expectation);

        void AssertCondition(AssertCondition condition);
    }

    public enum AssertionOptions
    {
        AssertOrder
    }

    public enum AssertCondition
    {
        Empty,
        NoEmpty,
        Exist
    }
}
