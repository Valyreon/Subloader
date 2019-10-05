namespace SubtitleSuppliers
{
    public interface ISubtitleResultItem
    {
        void Download(string path);
        string Language { get; }
        string Name { get; }
        string Format { get; }
    }
}
