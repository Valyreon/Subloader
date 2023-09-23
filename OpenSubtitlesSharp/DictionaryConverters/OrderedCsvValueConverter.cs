using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.DictionaryConverters;

internal class OrderedCsvValueConverter : IDictionaryValueConverter<IEnumerable<string>>
{
    public string Convert(IEnumerable<string> value)
    {
        var filteredOrderedValues = value?.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.ToLowerInvariant()).OrderBy(v => v).ToList();
        if (filteredOrderedValues == null || filteredOrderedValues.Count == 0)
        {
            return null;
        }

        return string.Join(',', filteredOrderedValues);
    }
}
