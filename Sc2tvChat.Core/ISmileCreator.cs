using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RatChat.Core {
    public interface ISmileCreator {
        bool CreateSmile( string SmileId, WrapPanel TextPanel );
    }
}
