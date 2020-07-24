using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dapper;
using TechTalk.SpecFlow;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class SimpleDataBaseFixture : IDisposable
    {
        private ScenarioContext scenatioContext;

        private IDictionary<string, SqlDataBaseClient> dataBaseConnectionMap;
        public SimpleDataBaseFixture(ScenarioContext scenatioContext, TestConfiguration testConfiguration)
        {
            this.scenatioContext = scenatioContext;
            dataBaseConnectionMap = testConfiguration.DatabaseConfigurationList
                .ToDictionary(x => x.Name, x => new SqlDataBaseClient(x.ConnectionString));
        }

        public IDictionary<string, object> Select(string table, string field, string where, string mapName = "default")
        {
            var sqlClient = mapName.Equals("default") ? dataBaseConnectionMap.FirstOrDefault().Value : dataBaseConnectionMap[mapName];

            return sqlClient.GetConnection.QuerySingle($"SELECT TOP(1) {field} FROM {table} WHERE {where}");
        }

        public IEnumerable<dynamic> SelectMany(string table, string field, string where, string mapName = "default")
        {
            var sqlClient = mapName.Equals("default") ? dataBaseConnectionMap.FirstOrDefault().Value : dataBaseConnectionMap[mapName];

            return sqlClient.GetConnection.Query($"SELECT {field} FROM {table} WHERE {where}");
        }

        public IDictionary<string, object> Select(string query, string mapName = "default")
        {
            var sqlClient = mapName.Equals("default") ? dataBaseConnectionMap.FirstOrDefault().Value : dataBaseConnectionMap[mapName];

            return sqlClient.GetConnection.QuerySingle(query);
        }

        public IEnumerable<dynamic> SelectMany(string query, string mapName = "default")
        {
            var sqlClient = mapName.Equals("default") ? dataBaseConnectionMap.FirstOrDefault().Value : dataBaseConnectionMap[mapName];

            return sqlClient.GetConnection.Query(query);
        }

        public void Dispose()
        {
            foreach (var clientMap in dataBaseConnectionMap)
            {
                clientMap.Value.Dispose();
            }
        }
    }
}
