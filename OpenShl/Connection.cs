using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using OpenShl.Models;

namespace OpenShl
{
    public class Connection
    {
        private readonly ConnectionOptions _options;
        private const string BaseUrl = "https://openapi.shl.se";
        private const string Auth = "/oauth2/token";
        private DateTime _expiresAt;
        private string _token;
        private HttpClient _client;

        public Connection(ConnectionOptions options)
        {
            _options = options;
            _client = new HttpClient();
        }

        public async Task Connect()
        {
            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", "OpenShlC#");
            var res = await _client.PostAsync(BaseUrl + Auth, new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", _options.ClientId),
                new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            }));

            if (!res.IsSuccessStatusCode)
            {
                throw new Exception($"Error when trying to authenticate: {(int) res.StatusCode} {res.ReasonPhrase}");
            }

            var stringContent = await res.Content.ReadAsStringAsync();
            var accessToken = JsonSerializer.Deserialize<AccessToken>(stringContent);
            _expiresAt = DateTime.UtcNow.AddSeconds(accessToken.ExpiresIn);
            _token = accessToken.Token;
        }

        public bool IsConnected() => !string.IsNullOrEmpty(_token) && DateTime.UtcNow <= _expiresAt;

        private async Task<string> _fetch(string path)
        {
            if (!IsConnected())
                return "Accesstoken has not been fetched";

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Add("User-Agent", "OpenShlC#");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");
            var result = await _client.GetStringAsync(BaseUrl + path);
            return result;
        }

        public async Task<string> Get(string path)
        {
            if (!IsConnected() && _options.AutoConnect)
            {
                await Connect();
            }

            return await _fetch(path);
        }
    }
}