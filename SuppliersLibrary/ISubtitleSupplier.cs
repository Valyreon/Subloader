using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuppliersLibrary
{
    public interface ISubtitleSupplier
    {
        Task<IList<ISubtitleResultItem>> SearchAsync(string path);
    }
}
