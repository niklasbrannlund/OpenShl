using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OpenShl
{
    public class ShlClient
    {
        private readonly Connection _connection;

        public ShlClient(string clientId, string clientSecret)
        {
            _connection = new Connection(new ConnectionOptions(clientId, clientSecret), new HttpClient());
        }

        public async Task<string> Teams()
        {
            return await _connection.Get("/teams");
        }

        public async Task<string> Team(string teamId)
        {
            return await _connection.Get($"/teams/{teamId}");
        }

        public async Task<string> Videos()
        {
            return await _connection.Get($"/videos");
        }

        public async Task<string> Articles()
        {
            return await _connection.Get($"/articles");
        }

        public async Task<string> Games(int season)
        {
            return await _connection.Get($"/seasons/{season}/games");
        }
        
        public async Task<string> Game(int season, string gameId)
        {
            return await _connection.Get($"/seasons/{season}/games/{gameId}");
        }
    }
}