namespace Flexo.SpecFlowApiTesting.Entities
{
    public abstract class AbstractAssertableJToken : IAssertableJToken
    {
        protected AbstractAssertableJToken(string pathName)
        {
            PathName = pathName;
        }

        public abstract void AssertContains(ExpectedJToken expectation);

        public abstract void AssertCondition(AssertCondition condition);

        public abstract void AssertEquals(ExpectedJToken expectation, params AssertionOptions[] options);

        protected string PathName { get; }
    }
}
