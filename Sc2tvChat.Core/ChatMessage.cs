using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Core {
    public class ChatMessage {
        public DateTime Date { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }
    }
}
