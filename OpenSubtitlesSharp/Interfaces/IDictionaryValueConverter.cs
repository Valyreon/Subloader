namespace OpenSubtitlesSharp.Interfaces;

internal interface IDictionaryValueConverter<T>
{
    string Convert(T value);
}
