using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubtitleSuppliers.OpenSubtitles
{
    public class OpenSubtitlesHashComparer : IEqualityComparer<OSItem>
    {
        public bool Equals(OSItem g1, OSItem g2)
        {
            return g1.SubHash == g2.SubHash;
        }

        public int GetHashCode(OSItem obj)
        {
            return obj.GetHashCode();
        }
    }
}
