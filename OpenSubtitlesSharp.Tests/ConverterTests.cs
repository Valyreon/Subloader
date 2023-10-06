using OpenSubtitlesSharp.DictionaryConverters;

namespace OpenSubtitlesSharp.Tests;
public class ConverterTests
{
    [Theory]
    [InlineData(null, null)]
    [InlineData(false, "include")]
    [InlineData(true, "only")]
    public void IncludeOnlyValueConverter_Tests(bool? value, string result)
    {
        var converter = new IncludeOnlyValueConverter();
        converter.Convert(value).ShouldBe(result);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(false, "exclude")]
    [InlineData(true, "include")]
    public void ExcludeIncludeValueConverter_Tests(bool? value, string result)
    {
        var converter = new ExcludeIncludeValueConverter();
        converter.Convert(value).ShouldBe(result);
    }
}
