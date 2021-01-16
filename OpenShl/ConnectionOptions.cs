using System.ComponentModel.DataAnnotations;

namespace OpenShl
{
    public class ConnectionOptions
    {

        public ConnectionOptions(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        [Required] 
        public string ClientId { get; }
        [Required] 
        public string ClientSecret { get; }

        public bool AutoConnect { get; set; }
    }
}