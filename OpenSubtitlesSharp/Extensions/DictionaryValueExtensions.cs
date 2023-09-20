using System.Data;
using System.Reflection;
using OpenSubtitlesSharp.Attributes;
using OpenSubtitlesSharp.DictionaryConverters;

namespace OpenSubtitlesSharp.Extensions
{
    internal static class DictionaryValueExtensions
    {
        internal static Dictionary<string, string> ObjectToDictionary<T>(this T obj)
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

                var isEnum = prop.PropertyType.IsEnum || (prop.PropertyType.IsGenericType &&
                                      prop.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                                      prop.PropertyType.GetGenericArguments()[0].IsEnum);

                if (attribute.ConverterType == null && !isEnum)
                {
                    result.Add(attribute.CustomName, prop.GetValue(obj).ToString());
                }
                else if (attribute.ConverterType == null && isEnum)
                {
                    result.Add(attribute.CustomName, prop.GetValue(obj).ToString().ToLowerInvariant());
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
                    result.Add(attribute.CustomName, methodInfo.Invoke(converter, new object[] { prop.GetValue(obj) }) as string);
                }
            }

            return result;
        }
    }
}
