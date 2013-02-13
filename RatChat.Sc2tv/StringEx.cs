using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatChat {
    public static class StringEx {
        public static int CountSubstring( this string s, string Subs ) {
            int c = 0;

            int i = -1;
            do {
                i = s.IndexOf(Subs, i + 1);
                if (i >= 0)
                    c++;
            } while (i >= 0);

            return c;
        }
    }
}
