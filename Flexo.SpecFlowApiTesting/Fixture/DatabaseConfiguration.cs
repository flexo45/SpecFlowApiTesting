using System;
using System.Collections.Generic;
using System.Text;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class DatabaseConfiguration
    {
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        public IEnumerable<string> ObjectMapping { get; set; }
    }
}
