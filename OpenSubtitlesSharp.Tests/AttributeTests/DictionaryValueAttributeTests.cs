using OpenSubtitlesSharp.Attributes;

namespace OpenSubtitlesSharp.Tests.AttributeTests;

public class DictionaryValueAttributeTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  \t")]
    public void DictionaryValueConstructor_WhenCustomNameEmpty_ThrowsArgumentException(string name)
    {
        var task = Task.Run(() => new DictionaryValueAttribute(name));
        task.ShouldThrow<ArgumentException>();
    }

    [Fact]
    public void DictionaryValueConstructor_WhenConverterTypeIsNotDictionaryValueConverter_ThrowsArgumentException()
    {
        var task = Task.Run(() => new DictionaryValueAttribute("custom_name", typeof(SearchParameters)));
        task.ShouldThrow<ArgumentException>();
    }
}
