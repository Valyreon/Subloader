using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubLoad.Views
{
    public interface IView
    {
        void ChangeCurrentControlTo(object x);
    }
}
