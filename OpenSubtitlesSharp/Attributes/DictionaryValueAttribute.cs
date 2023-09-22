using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.Attributes;

[AttributeUsage(AttributeTargets.Property)]
internal class DictionaryValueAttribute : Attribute
{
    /// <summary>
    /// Attribute to be used for converting an object into a IDictionary<string,string> type.
    /// </summary>
    /// <param name="customName">When dictionary is generated, this will be the key of the property.</param>
    /// <param name="converterType">If specified will be used to convert the properties value into string.</param>
    /// <param name="ignoreValue">If specified, when object is converted to dictionary and the property has this value it will not be included.</param>
    /// <exception cref="ArgumentException"></exception>
    public DictionaryValueAttribute(string customName, Type converterType = null, object ignoreValue = null)
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
        IgnoreValue = ignoreValue;
    }

    public Type ConverterType { get; }
    public string CustomName { get; }
    public object IgnoreValue { get; }
}
