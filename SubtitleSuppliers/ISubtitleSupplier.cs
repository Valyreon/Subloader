using System.Collections.Generic;
using System.Threading.Tasks;

namespace SubtitleSuppliers
{
    public interface ISubtitleSupplier
    {
        Task<IList<ISubtitleResultItem>> SearchAsync(string path);
    }
}
