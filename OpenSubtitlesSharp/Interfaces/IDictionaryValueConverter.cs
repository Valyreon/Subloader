namespace OpenSubtitlesSharp.Interfaces;

internal interface IDictionaryValueConverter<T>
{
    public string Convert(T value);
}
