using System.Collections.Generic;
using System.Threading.Tasks;

namespace SuppliersLibrary
{
    public interface ISubtitleSupplier
    {
        Task<IReadOnlyList<ISubtitleResultItem>> SearchForFileAsync(string filePath, object[] parameters = null);
        Task<IReadOnlyList<ISubtitleResultItem>> SearchAsync(string token);
    }
}
