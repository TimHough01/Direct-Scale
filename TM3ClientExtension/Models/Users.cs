using Newtonsoft.Json;

namespace TM3ClientExtension.Models
{
    public class Users
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
