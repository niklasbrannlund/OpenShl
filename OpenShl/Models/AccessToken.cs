using System.Text.Json.Serialization;

namespace OpenShl.Models
{
    public class AccessToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; }
        
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}