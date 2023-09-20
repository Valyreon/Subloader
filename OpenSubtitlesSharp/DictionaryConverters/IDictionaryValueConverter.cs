namespace OpenSubtitlesSharp.DictionaryConverters;

internal interface IDictionaryValueConverter<T>
{
    string Convert(T value);
}
