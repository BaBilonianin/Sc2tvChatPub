using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat.Core {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ConfigValueAttribute : Attribute {
        public static ConfigValueAttribute[] GetAttribute( Type type ) {
            List<ConfigValueAttribute> cva = new List<ConfigValueAttribute>();
            foreach (ConfigValueAttribute da in type.GetCustomAttributes(typeof(ConfigValueAttribute), true))
                cva.Add(da);
            return cva.ToArray();
        }

        public ConfigValueAttribute( string Name, string DefaultValue, string Caption ) {
            this.Name = Name;
            this.Caption = Caption;
            this.DefaultValue = DefaultValue;
        }

        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public string Caption { get; set; }
    }
}
