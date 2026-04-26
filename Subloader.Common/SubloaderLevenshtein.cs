using Fastenshtein;
using OpenSubtitlesSharp;

namespace Subloader.Common;

public class SubloaderLevenshtein
{
    private readonly Levenshtein originalLeven;
    private readonly Levenshtein rootLeven;
    private readonly bool seasonEpisodeFound;

    public SubloaderLevenshtein(string original)
    {
        originalLeven = new(original);

        var seasonEpisodeInfo = OpenSubtitlesClient.SeasonEpisodeRegex().Match(original);
        if (seasonEpisodeInfo.Success)
        {
            seasonEpisodeFound = true;
            var originRoot = original[..(seasonEpisodeInfo.Index + seasonEpisodeInfo.Length)];
            rootLeven = new(originRoot);
        }
        else
        {
            var cleaned = OpenSubtitlesClient.InfoCleanRegex().Replace(original, "");
            rootLeven = new(cleaned);
        }
    }

    public int DistanceFrom(string str)
    {
        var cleanedStr = GetRootString(str);
        return 2 * rootLeven.DistanceFrom(cleanedStr) + originalLeven.DistanceFrom(str);
    }

    private string GetRootString(string str)
    {
        if (seasonEpisodeFound)
        {
            var seasonEpisodeInfo = OpenSubtitlesClient.SeasonEpisodeRegex().Match(str);
            if (seasonEpisodeInfo.Success)
            {
                return str[..(seasonEpisodeInfo.Index + seasonEpisodeInfo.Length)];
            }
        }

        return OpenSubtitlesClient.InfoCleanRegex().Replace(str, "");
    }
}
