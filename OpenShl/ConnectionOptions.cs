using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OpenShl
{
    public class ConnectionOptions
    {

        public ConnectionOptions() { }
        public ConnectionOptions(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            AutoConnect = true;
        }

        [Required]
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        
        [Required]
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }

        [JsonPropertyName("grant_type")]
        public string GrantType => "client_credentials";
        
        [JsonIgnore]
        public bool AutoConnect { get; set; }
    }
}