using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Flexo.SpecFlowApiTesting.Fixture
{
    public class SqlDataBaseClient : IDisposable
    {
        private readonly SqlConnection _connection;

        public SqlDataBaseClient(string connectionString)
        {
            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public SqlConnection GetConnection => _connection;

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
