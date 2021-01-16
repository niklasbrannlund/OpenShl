using System;

namespace OpenShl
{
    public class ShlClient
    {
        private readonly Connection _connection;

        public ShlClient(Connection connection)
        {
            _connection = connection;
        }
    }
}