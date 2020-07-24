using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class TestConfiguration
    {
        private IConfigurationRoot _config;

        public IEnumerable<DatabaseConfiguration> DatabaseConfigurationList { get; }
        public TestConfiguration()
        {
            _config = new ConfigurationBuilder()
                .AddJsonFile("testsettings.json")
                .AddEnvironmentVariables()
                .Build();

            DatabaseConfigurationList = _config.GetSection("Databases").GetChildren().Select(x => x.Get<DatabaseConfiguration>());
        }
    }
}
