namespace SuppliersLibrary
{
    public interface ISubtitleResultItem
    {
        string Language { get; }

        string Name { get; }

        string Format { get; }

        string LanguageID { get; }

        void Download(string path);
    }
}
