namespace OpenSubtitlesSharp.DictionaryConverters;

internal class IncludeOnlyValueConverter : IDictionaryValueConverter<bool?>
{
    public string Convert(bool? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        return value == true ? "only" : "include";
    }
}
