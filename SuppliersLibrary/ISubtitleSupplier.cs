using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuppliersLibrary
{
    public interface ISubtitleSupplier
    {
        Task<IReadOnlyList<ISubtitleResultItem>> SearchAsync(string path, object[] parameters = null);
    }
}
