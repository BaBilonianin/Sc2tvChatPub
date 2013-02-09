using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc2tvChat {
    public class Message {
        [JsonProperty(PropertyName = "channelId")]
        public int ChannelId { get; set; }

        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "uid")]
        public int Uid { get; set; }

        [JsonProperty(PropertyName = "rid")]
        public int Rid { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

   
}
