using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Flexo.SpecFlowApiTesting.Entities
{
    public class ExpectedJToken
    {
        public ExpectedJToken(JToken jToken)
        {
            this.JToken = jToken;
        }
        public JToken JToken { get; }

        public object TokenObject => JToken.ToObject<object>();
    }
}
