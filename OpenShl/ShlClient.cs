using System;
using System.Threading.Tasks;

namespace OpenShl
{
    public class ShlClient
    {
        private readonly Connection _connection;

        public ShlClient(Connection connection)
        {
            _connection = connection;
        }

        public async Task<string> Teams()
        {
            return await _connection.Get("/teams");
        }
    }
}