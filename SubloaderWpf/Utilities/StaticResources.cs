using System.Collections.Generic;
using OpenSubtitlesSharp;

namespace SubloaderWpf.Utilities;

public static class StaticResources
{
    private static IReadOnlyList<SubtitleLanguage> allLanguages;

    public static IReadOnlyList<SubtitleLanguage> AllLanguages
    {
        get
        {
            if(allLanguages == null)
            {
                InitializeList();
            }

            return allLanguages;
        }
    }

    private static void InitializeList()
    {
        allLanguages = new SubtitleLanguage[]
        {
            new SubtitleLanguage { Code = "af", Name = "Afrikaans" },
            new SubtitleLanguage { Code = "sq", Name = "Albanian" },
            new SubtitleLanguage { Code = "ar", Name = "Arabic" },
            new SubtitleLanguage { Code = "an", Name = "Aragonese" },
            new SubtitleLanguage { Code = "hy", Name = "Armenian" },
            new SubtitleLanguage { Code = "at", Name = "Asturian" },
            new SubtitleLanguage { Code = "eu", Name = "Basque" },
            new SubtitleLanguage { Code = "be", Name = "Belarusian" },
            new SubtitleLanguage { Code = "bn", Name = "Bengali" },
            new SubtitleLanguage { Code = "bs", Name = "Bosnian" },
            new SubtitleLanguage { Code = "br", Name = "Breton" },
            new SubtitleLanguage { Code = "bg", Name = "Bulgarian" },
            new SubtitleLanguage { Code = "my", Name = "Burmese" },
            new SubtitleLanguage { Code = "ca", Name = "Catalan" },
            new SubtitleLanguage { Code = "zh-cn", Name = "Chinese (simplified)" },
            new SubtitleLanguage { Code = "cs", Name = "Czech" },
            new SubtitleLanguage { Code = "da", Name = "Danish" },
            new SubtitleLanguage { Code = "nl", Name = "Dutch" },
            new SubtitleLanguage { Code = "en", Name = "English" },
            new SubtitleLanguage { Code = "eo", Name = "Esperanto" },
            new SubtitleLanguage { Code = "et", Name = "Estonian" },
            new SubtitleLanguage { Code = "fi", Name = "Finnish" },
            new SubtitleLanguage { Code = "fr", Name = "French" },
            new SubtitleLanguage { Code = "ka", Name = "Georgian" },
            new SubtitleLanguage { Code = "de", Name = "German" },
            new SubtitleLanguage { Code = "gl", Name = "Galician" },
            new SubtitleLanguage { Code = "el", Name = "Greek" },
            new SubtitleLanguage { Code = "he", Name = "Hebrew" },
            new SubtitleLanguage { Code = "hi", Name = "Hindi" },
            new SubtitleLanguage { Code = "hr", Name = "Croatian" },
            new SubtitleLanguage { Code = "hu", Name = "Hungarian" },
            new SubtitleLanguage { Code = "is", Name = "Icelandic" },
            new SubtitleLanguage { Code = "id", Name = "Indonesian" },
            new SubtitleLanguage { Code = "it", Name = "Italian" },
            new SubtitleLanguage { Code = "ja", Name = "Japanese" },
            new SubtitleLanguage { Code = "kk", Name = "Kazakh" },
            new SubtitleLanguage { Code = "km", Name = "Khmer" },
            new SubtitleLanguage { Code = "ko", Name = "Korean" },
            new SubtitleLanguage { Code = "lv", Name = "Latvian" },
            new SubtitleLanguage { Code = "lt", Name = "Lithuanian" },
            new SubtitleLanguage { Code = "lb", Name = "Luxembourgish" },
            new SubtitleLanguage { Code = "mk", Name = "Macedonian" },
            new SubtitleLanguage { Code = "ml", Name = "Malayalam" },
            new SubtitleLanguage { Code = "ms", Name = "Malay" },
            new SubtitleLanguage { Code = "ma", Name = "Manipuri" },
            new SubtitleLanguage { Code = "mn", Name = "Mongolian" },
            new SubtitleLanguage { Code = "no", Name = "Norwegian" },
            new SubtitleLanguage { Code = "oc", Name = "Occitan" },
            new SubtitleLanguage { Code = "fa", Name = "Persian" },
            new SubtitleLanguage { Code = "pl", Name = "Polish" },
            new SubtitleLanguage { Code = "pt-pt", Name = "Portuguese" },
            new SubtitleLanguage { Code = "ru", Name = "Russian" },
            new SubtitleLanguage { Code = "sr", Name = "Serbian" },
            new SubtitleLanguage { Code = "si", Name = "Sinhalese" },
            new SubtitleLanguage { Code = "sk", Name = "Slovak" },
            new SubtitleLanguage { Code = "sl", Name = "Slovenian" },
            new SubtitleLanguage { Code = "es", Name = "Spanish" },
            new SubtitleLanguage { Code = "sw", Name = "Swahili" },
            new SubtitleLanguage { Code = "sv", Name = "Swedish" },
            new SubtitleLanguage { Code = "sy", Name = "Syriac" },
            new SubtitleLanguage { Code = "ta", Name = "Tamil" },
            new SubtitleLanguage { Code = "te", Name = "Telugu" },
            new SubtitleLanguage { Code = "tl", Name = "Tagalog" },
            new SubtitleLanguage { Code = "th", Name = "Thai" },
            new SubtitleLanguage { Code = "tr", Name = "Turkish" },
            new SubtitleLanguage { Code = "uk", Name = "Ukrainian" },
            new SubtitleLanguage { Code = "ur", Name = "Urdu" },
            new SubtitleLanguage { Code = "uz", Name = "Uzbek" },
            new SubtitleLanguage { Code = "vi", Name = "Vietnamese" },
            new SubtitleLanguage { Code = "ro", Name = "Romanian" },
            new SubtitleLanguage { Code = "pt-br", Name = "Portuguese (Brazilian)" },
            new SubtitleLanguage { Code = "me", Name = "Montenegrin" },
            new SubtitleLanguage { Code = "zh-tw", Name = "Chinese (traditional)" },
            new SubtitleLanguage { Code = "ze", Name = "Chinese bilingual" },
            new SubtitleLanguage { Code = "nb", Name = "Norwegian Bokmal" },
            new SubtitleLanguage { Code = "se", Name = "Northern Sami" }
        };
    }
}
