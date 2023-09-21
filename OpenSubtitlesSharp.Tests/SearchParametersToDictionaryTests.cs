using OpenSubtitlesSharp.Extensions;
using Shouldly;

namespace OpenSubtitlesSharp.Tests
{
    public class SearchParametersToDictionaryTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(Filter.Exclude, "exclude")]
        [InlineData(Filter.Include, "include")]
        [InlineData(Filter.Only, "only")]
        public void ForeignPartsOnly_DictionaryValueTest(Filter? value, string expected)
        {
            var p = new SearchParameters
            {
                ForeignPartsOnly = value
            };

            var dict = p.ObjectToDictionary();

            if(expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["foreign_parts_only"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void EpisodeNumber_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                EpisodeNumber = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["episode_number"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(true, "include")]
        [InlineData(false, "exclude")]
        public void IncludeAiTranslated_DictionaryValueTest(bool? value, string expected)
        {
            var p = new SearchParameters
            {
                IncludeAiTranslated = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["ai_translated"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("zv,EN,Ab", "ab,en,zv")]
        [InlineData("en", "en")]
        public void Languages_DictionaryValueTest(string value, string expected)
        {
            var p = new SearchParameters
            {
                Languages = value?.Split(",")
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["languages"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(Filter.Include, "include")]
        [InlineData(Filter.Exclude, "exclude")]
        [InlineData(Filter.Only, "only")]
        public void HearingImpaired_DictionaryValueTest(Filter? value, string expected)
        {
            var p = new SearchParameters
            {
                HearingImpaired = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["hearing_impaired"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void Id_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                Id = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void ImdbId_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                ImdbId = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["imdb_id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void TmdbId_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                TmdbId = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["tmdb_id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void TmdbIdParentId_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                ParentTmdbId = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["parent_tmdb_id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void ParentImdbId_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                ParentImdbId = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["parent_imdb_id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void SeasonNumber_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                SeasonNumber = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["season_number"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(1998, "1998")]
        public void Year_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                Year = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["year"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void Page_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                Page = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["page"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(FileTypeFilter.All, "all")]
        [InlineData(FileTypeFilter.Episode, "episode")]
        [InlineData(FileTypeFilter.Movie, "movie")]
        public void Type_DictionaryValueTest(FileTypeFilter? value, string expected)
        {
            var p = new SearchParameters
            {
                Type = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["type"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(true, "only")]
        [InlineData(false, "include")]
        public void IncludeOnlyFromTrustedSources_DictionaryValueTest(bool? value, string expected)
        {
            var p = new SearchParameters
            {
                IncludeOnlyFromTrustedSources = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["trusted_sources"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(true, "include")]
        [InlineData(false, "exclude")]
        public void IncludeMachineTranslated_DictionaryValueTest(bool? value, string expected)
        {
            var p = new SearchParameters
            {
                IncludeMachineTranslated = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["machine_translated"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(true, "only")]
        [InlineData(false, "include")]
        public void OnlyMovieHashMatch_DictionaryValueTest(bool? value, string expected)
        {
            var p = new SearchParameters
            {
                OnlyMovieHashMatch = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["moviehash_match"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("afdg5", "afdg5")]
        public void MovieHash_DictionaryValueTest(string value, string expected)
        {
            var p = new SearchParameters
            {
                MovieHash = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["moviehash"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("af dg5", "af dg5")]
        public void Query_DictionaryValueTest(string value, string expected)
        {
            var p = new SearchParameters
            {
                Query = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["query"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(5, "5")]
        public void ParentFeatureId_DictionaryValueTest(int? value, string expected)
        {
            var p = new SearchParameters
            {
                ParentFeatureId = value
            };

            var dict = p.ObjectToDictionary();

            if (expected != null)
            {
                dict.Count.ShouldBe(1);
                dict["parent_feature_id"].ShouldBe(expected);
            }
            else
            {
                dict.ShouldBeEmpty();
            }
        }
    }
}
