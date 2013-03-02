using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.EmpireTv {
    internal class Message {
        //[JsonProperty(PropertyName = "c")]
        //public string ChannelId { get; set; }

        [JsonProperty(PropertyName = "c")]
        public long UnixDate { get; set; }


        [JsonProperty(PropertyName = "i")]
        public string Id { get; set; }

        //[JsonProperty(PropertyName = "u")]
        //public int Uid { get; set; }

        [JsonProperty(PropertyName = "m")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "n")]
        public string Name { get; set; }

        public int NeedToDelete { get; set; }
    }

   
}
