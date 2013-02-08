using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sc2tvChat {
    public class Achievement {
        public int PekaCount { get; set; }
        public void Update( string SmileId ) {
            if (SmileId == ":peka:")
                PekaCount++;
        }
    }
}
