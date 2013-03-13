using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.CybergameTV {

    //[{"id":"428041","room":"vendetto44rus","timestamp":"2013-03-03 11:38:54",
    //"deleted":"0","unix_timestamp":"1362296334",
    //"alias":"oxlamon","md5email":"a94ab0a253c80ea9f897e2a271cfc239",
    //"status":"1","message":"sasd","timestring":"3.3 - 11:38:54"}]

    internal class Message {
        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "alias")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "message")]
        public string Text { get; set; }

        [JsonProperty(PropertyName = "unix_timestamp")]
        public long Date { get; set; }

        public int NeedToDelete { get; set; }
    }

   
}
