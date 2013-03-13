using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.CybergameTV {
    internal class Messages {
        [JsonProperty(PropertyName = "quick_chat_messages")]
        internal Message[] Content { get; set; }

        [JsonProperty(PropertyName = "quick_chat_no_participation")]
        internal int NoParticipation { get; set; }

        [JsonProperty(PropertyName = "quick_chat_success")]
        internal int Success { get; set; }

        [JsonProperty(PropertyName = "quick_chat_update_messages_nonce")]
        internal string MessagesNonce { get; set; }
    }
}
