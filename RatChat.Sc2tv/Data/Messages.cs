using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Sc2tv {
    internal class Messages {
        [JsonProperty(PropertyName = "messages")]
        internal Message[] Content { get; set; }
    }
}
