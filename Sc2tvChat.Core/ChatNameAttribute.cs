using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Core {
     [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ChatNameAttribute: Attribute {
        public ChatNameAttribute( string Name ) {
            this.Name = Name;
        }

        public string Name { get; set; }

        public static ChatNameAttribute GetAttribute( Type type ) {
            foreach (ChatNameAttribute da in type.GetCustomAttributes(typeof(ChatNameAttribute), true))
                return da;
            return null;
        }
    }
}
