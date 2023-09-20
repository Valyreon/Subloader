using OpenSubtitlesSharp.DictionaryConverters;

namespace OpenSubtitlesSharp.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class DictionaryValueAttribute : Attribute
{
    public DictionaryValueAttribute(string customName, Type converterType = null)
    {
        if (string.IsNullOrWhiteSpace(customName))
        {
            throw new ArgumentException("You have to provide the custom name.", nameof(customName));
        }

        if (converterType != null && !converterType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionaryValueConverter<>)))
        {
            throw new ArgumentException("Converter type must implement IDictionaryValueConverter.", nameof(converterType));
        }

        CustomName = customName;
        ConverterType = converterType;
    }

    public Type ConverterType { get; }
    public string CustomName { get; }
}
