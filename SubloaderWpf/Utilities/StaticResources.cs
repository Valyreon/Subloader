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
        allLanguages =
        [
            new() { Code = "af", Name = "Afrikaans" },
            new() { Code = "sq", Name = "Albanian" },
            new() { Code = "ar", Name = "Arabic" },
            new() { Code = "an", Name = "Aragonese" },
            new() { Code = "hy", Name = "Armenian" },
            new() { Code = "at", Name = "Asturian" },
            new() { Code = "eu", Name = "Basque" },
            new() { Code = "be", Name = "Belarusian" },
            new() { Code = "bn", Name = "Bengali" },
            new() { Code = "bs", Name = "Bosnian" },
            new() { Code = "br", Name = "Breton" },
            new() { Code = "bg", Name = "Bulgarian" },
            new() { Code = "my", Name = "Burmese" },
            new() { Code = "ca", Name = "Catalan" },
            new() { Code = "zh-cn", Name = "Chinese (simplified)" },
            new() { Code = "cs", Name = "Czech" },
            new() { Code = "da", Name = "Danish" },
            new() { Code = "nl", Name = "Dutch" },
            new() { Code = "en", Name = "English" },
            new() { Code = "eo", Name = "Esperanto" },
            new() { Code = "et", Name = "Estonian" },
            new() { Code = "fi", Name = "Finnish" },
            new() { Code = "fr", Name = "French" },
            new() { Code = "ka", Name = "Georgian" },
            new() { Code = "de", Name = "German" },
            new() { Code = "gl", Name = "Galician" },
            new() { Code = "el", Name = "Greek" },
            new() { Code = "he", Name = "Hebrew" },
            new() { Code = "hi", Name = "Hindi" },
            new() { Code = "hr", Name = "Croatian" },
            new() { Code = "hu", Name = "Hungarian" },
            new() { Code = "is", Name = "Icelandic" },
            new() { Code = "id", Name = "Indonesian" },
            new() { Code = "it", Name = "Italian" },
            new() { Code = "ja", Name = "Japanese" },
            new() { Code = "kk", Name = "Kazakh" },
            new() { Code = "km", Name = "Khmer" },
            new() { Code = "ko", Name = "Korean" },
            new() { Code = "lv", Name = "Latvian" },
            new() { Code = "lt", Name = "Lithuanian" },
            new() { Code = "lb", Name = "Luxembourgish" },
            new() { Code = "mk", Name = "Macedonian" },
            new() { Code = "ml", Name = "Malayalam" },
            new() { Code = "ms", Name = "Malay" },
            new() { Code = "ma", Name = "Manipuri" },
            new() { Code = "mn", Name = "Mongolian" },
            new() { Code = "no", Name = "Norwegian" },
            new() { Code = "oc", Name = "Occitan" },
            new() { Code = "fa", Name = "Persian" },
            new() { Code = "pl", Name = "Polish" },
            new() { Code = "pt-pt", Name = "Portuguese" },
            new() { Code = "ru", Name = "Russian" },
            new() { Code = "sr", Name = "Serbian" },
            new() { Code = "si", Name = "Sinhalese" },
            new() { Code = "sk", Name = "Slovak" },
            new() { Code = "sl", Name = "Slovenian" },
            new() { Code = "es", Name = "Spanish" },
            new() { Code = "sw", Name = "Swahili" },
            new() { Code = "sv", Name = "Swedish" },
            new() { Code = "sy", Name = "Syriac" },
            new() { Code = "ta", Name = "Tamil" },
            new() { Code = "te", Name = "Telugu" },
            new() { Code = "tl", Name = "Tagalog" },
            new() { Code = "th", Name = "Thai" },
            new() { Code = "tr", Name = "Turkish" },
            new() { Code = "uk", Name = "Ukrainian" },
            new() { Code = "ur", Name = "Urdu" },
            new() { Code = "uz", Name = "Uzbek" },
            new() { Code = "vi", Name = "Vietnamese" },
            new() { Code = "ro", Name = "Romanian" },
            new() { Code = "pt-br", Name = "Portuguese (Brazilian)" },
            new() { Code = "me", Name = "Montenegrin" },
            new() { Code = "zh-tw", Name = "Chinese (traditional)" },
            new() { Code = "ze", Name = "Chinese bilingual" },
            new() { Code = "nb", Name = "Norwegian Bokmal" },
            new() { Code = "se", Name = "Northern Sami" }
        ];
    }
}
