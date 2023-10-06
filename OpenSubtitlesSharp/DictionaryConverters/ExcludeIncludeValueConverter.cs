using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.DictionaryConverters;

internal class ExcludeIncludeValueConverter : IDictionaryValueConverter<bool?>
{
    public string Convert(bool? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return value == true ? "include" : "exclude";
    }
}
