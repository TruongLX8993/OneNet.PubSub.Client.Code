using Newtonsoft.Json;

namespace OneNet.PubSub.Client.Models
{
    public class Topic
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("ownerName")] public string OwnerName { get; set; }
        [JsonProperty("ownerConnectionId")] public string OwnerConnectionId { get; set; }
    }
}