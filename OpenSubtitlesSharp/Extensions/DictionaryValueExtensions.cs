using System.Data;
using System.Reflection;
using OpenSubtitlesSharp.Attributes;
using OpenSubtitlesSharp.Interfaces;

namespace OpenSubtitlesSharp.Extensions;

internal static class DictionaryValueExtensions
{
    internal static IReadOnlyDictionary<string, string> ObjectToDictionary<T>(this T obj)
    {
        var type = typeof(T);
        var properties = type.GetProperties()
            .Where(prop => prop.GetValue(obj) != null && prop.GetCustomAttribute<DictionaryValueAttribute>() != null);

        var result = new Dictionary<string, string>();

        if (!properties.Any())
        {
            return result;
        }

        foreach (var prop in properties)
        {
            var attribute = prop.GetCustomAttribute<DictionaryValueAttribute>();
            var value = prop.GetValue(obj);

            var x = attribute.IgnoreValue != null;
            var y = value == attribute.IgnoreValue;

            if (attribute.IgnoreValue != null && value.Equals(attribute.IgnoreValue))
            {
                continue;
            }

            var isEnum = prop.PropertyType.IsEnum || (prop.PropertyType.IsGenericType &&
                                  prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                  prop.PropertyType.GetGenericArguments()[0].IsEnum);

            if (attribute.ConverterType == null && !isEnum)
            {
                result.Add(attribute.CustomName, value.ToString());
            }
            else if (attribute.ConverterType == null && isEnum)
            {
                result.Add(attribute.CustomName, value.ToString().ToLowerInvariant());
            }
            else
            {
                var converterType = typeof(IDictionaryValueConverter<>).MakeGenericType(prop.PropertyType);
                var converter = Activator.CreateInstance(attribute.ConverterType);
                var d = converter.GetType().GetInterfaces()[0].UnderlyingSystemType;
                if (d != converterType)
                {
                    throw new InvalidOperationException("Converter's generic type parameter has to be same as the property type.");
                }

                var methodInfo = converterType.GetMethod("Convert");
                var convertedValue = methodInfo.Invoke(converter, new object[] { value });

                if(convertedValue == null)
                {
                    continue;
                }

                result.Add(attribute.CustomName, convertedValue as string);
            }
        }

        return result;
    }
}
