using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Core {
    public class RatChatException: Exception {
        public RatChatException( string Message )
            : base(Message) {
        }
    }
}
