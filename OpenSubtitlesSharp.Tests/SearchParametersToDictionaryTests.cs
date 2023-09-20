using OpenSubtitlesSharp.Extensions;
using Shouldly;

namespace OpenSubtitlesSharp.Tests
{
    public class SearchParametersToDictionaryTests
    {
        [Fact]
        public void SetEnumPropertyWithoutConverter_DictionaryContainsEnumString()
        {
            var p = new SearchParameters
            {
                ForeignPartsOnly = Filter.Exclude
            };

            var dict = p.ObjectToDictionary();
            dict.Count.ShouldBe(1);

            dict["foreign_parts_only"].ShouldBe("exclude");
        }

        [Fact]
        public void SetOnlyEpisodeNumber_DictionaryOnlyContainsEpisodeNumber()
        {
            var p = new SearchParameters
            {
                EpisodeNumber = 5
            };

            var dict = p.ObjectToDictionary();
            dict.Count.ShouldBe(1);

            dict["episode_number"].ShouldBe("5");
        }

        [Fact]
        public void SetPropertyWithExcludeIncludeConverter_ValueIsConverted()
        {
            var p = new SearchParameters
            {
                IncludeAiTranslated = true
            };

            var dict = p.ObjectToDictionary();
            dict.Count.ShouldBe(1);

            dict["ai_translated"].ShouldBe("include");
        }

        [Fact]
        public void SetLanguages_ValueIsConverted()
        {
            var p = new SearchParameters
            {
                Languages = new List<string>() { "zv", "EN", "Ab" }
            };

            var dict = p.ObjectToDictionary();
            dict.Count.ShouldBe(1);

            dict["languages"].ShouldBe("ab,en,zv");
        }
    }
}
