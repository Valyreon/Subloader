using System.Collections.Generic;

namespace SubloaderWpf.Utilities
{
    public class SubtitleLanguage
    {
        public SubtitleLanguage(string name, string code)
        {
            Name = name;
            Code = code;
        }

        public SubtitleLanguage()
        {
        }

        public static IEnumerable<SubtitleLanguage> AllLanguages { get; } = new List<SubtitleLanguage>()
        {
            new SubtitleLanguage("Abkhazian", "abk"),
            new SubtitleLanguage("Achinese", "ace"),
            new SubtitleLanguage("Acoli", "ach"),
            new SubtitleLanguage("Adangme", "ada"),
            new SubtitleLanguage("Adygh", "ady"),
            new SubtitleLanguage("Afar", "aar"),
            new SubtitleLanguage("Afrihili", "afh"),
            new SubtitleLanguage("Afrikaans", "afr"),
            new SubtitleLanguage("Afro-Asiatic", "afa"),
            new SubtitleLanguage("Ainu", "ain"),
            new SubtitleLanguage("Akan", "aka"),
            new SubtitleLanguage("Akkadian", "akk"),
            new SubtitleLanguage("Albanian", "alb"),
            new SubtitleLanguage("Aleut", "ale"),
            new SubtitleLanguage("Algonquian languages", "alg"),
            new SubtitleLanguage("Altaic", "tut"),
            new SubtitleLanguage("Amharic", "amh"),
            new SubtitleLanguage("Apache languages", "apa"),
            new SubtitleLanguage("Arabic", "ara"),
            new SubtitleLanguage("Aragonese", "arg"),
            new SubtitleLanguage("Aramaic", "arc"),
            new SubtitleLanguage("Arapaho", "arp"),
            new SubtitleLanguage("Araucanian", "arn"),
            new SubtitleLanguage("Arawak", "arw"),
            new SubtitleLanguage("Armenian", "arm"),
            new SubtitleLanguage("Aromanian", "rup"),
            new SubtitleLanguage("Artificial", "art"),
            new SubtitleLanguage("Assamese", "asm"),
            new SubtitleLanguage("Asturian", "ast"),
            new SubtitleLanguage("Athapascan languages", "ath"),
            new SubtitleLanguage("Australian languages", "aus"),
            new SubtitleLanguage("Austronesian", "map"),
            new SubtitleLanguage("Avaric", "ava"),
            new SubtitleLanguage("Avestan", "ave"),
            new SubtitleLanguage("Awadhi", "awa"),
            new SubtitleLanguage("Aymara", "aym"),
            new SubtitleLanguage("Azerbaijani", "aze"),
            new SubtitleLanguage("Balinese", "ban"),
            new SubtitleLanguage("Baltic", "bat"),
            new SubtitleLanguage("Baluchi", "bal"),
            new SubtitleLanguage("Bambara", "bam"),
            new SubtitleLanguage("Bamileke languages", "bai"),
            new SubtitleLanguage("Banda", "bad"),
            new SubtitleLanguage("Bantu", "bnt"),
            new SubtitleLanguage("Basa", "bas"),
            new SubtitleLanguage("Bashkir", "bak"),
            new SubtitleLanguage("Basque", "baq"),
            new SubtitleLanguage("Batak", "btk"),
            new SubtitleLanguage("Beja", "bej"),
            new SubtitleLanguage("Belarusian", "bel"),
            new SubtitleLanguage("Bemba", "bem"),
            new SubtitleLanguage("Bengali", "ben"),
            new SubtitleLanguage("Berber", "ber"),
            new SubtitleLanguage("Bhojpuri", "bho"),
            new SubtitleLanguage("Bihari", "bih"),
            new SubtitleLanguage("Bikol", "bik"),
            new SubtitleLanguage("Bini", "bin"),
            new SubtitleLanguage("Bislama", "bis"),
            new SubtitleLanguage("Blin", "byn"),
            new SubtitleLanguage("Bosnian", "bos"),
            new SubtitleLanguage("Braj", "bra"),
            new SubtitleLanguage("Breton", "bre"),
            new SubtitleLanguage("Buginese", "bug"),
            new SubtitleLanguage("Bulgarian", "bul"),
            new SubtitleLanguage("Buriat", "bua"),
            new SubtitleLanguage("Burmese", "bur"),
            new SubtitleLanguage("Caddo", "cad"),
            new SubtitleLanguage("Carib", "car"),
            new SubtitleLanguage("Catalan", "cat"),
            new SubtitleLanguage("Caucasian", "cau"),
            new SubtitleLanguage("Cebuano", "ceb"),
            new SubtitleLanguage("Celtic", "cel"),
            new SubtitleLanguage("Central American", "cai"),
            new SubtitleLanguage("Chagatai", "chg"),
            new SubtitleLanguage("Chamic languages", "cmc"),
            new SubtitleLanguage("Chamorro", "cha"),
            new SubtitleLanguage("Chechen", "che"),
            new SubtitleLanguage("Cherokee", "chr"),
            new SubtitleLanguage("Cheyenne", "chy"),
            new SubtitleLanguage("Chibcha", "chb"),
            new SubtitleLanguage("Chichewa", "nya"),
            new SubtitleLanguage("Chinese bilingual", "zhe"),
            new SubtitleLanguage("Chinese", "chi"),
            new SubtitleLanguage("Chinese", "zht"),
            new SubtitleLanguage("Chinook jargon", "chn"),
            new SubtitleLanguage("Chipewyan", "chp"),
            new SubtitleLanguage("Choctaw", "cho"),
            new SubtitleLanguage("Chuukese", "chk"),
            new SubtitleLanguage("Chuvash", "chv"),
            new SubtitleLanguage("Classical Newari", "nwc"),
            new SubtitleLanguage("Coptic", "cop"),
            new SubtitleLanguage("Cornish", "cor"),
            new SubtitleLanguage("Corsican", "cos"),
            new SubtitleLanguage("Cree", "cre"),
            new SubtitleLanguage("Creek", "mus"),
            new SubtitleLanguage("Creoles and", "cpe"),
            new SubtitleLanguage("Creoles and", "cpf"),
            new SubtitleLanguage("Creoles and", "cpp"),
            new SubtitleLanguage("Creoles and", "crp"),
            new SubtitleLanguage("Crimean Tatar", "crh"),
            new SubtitleLanguage("Croatian", "hrv"),
            new SubtitleLanguage("Cushitic", "cus"),
            new SubtitleLanguage("Czech", "cze"),
            new SubtitleLanguage("Dakota", "dak"),
            new SubtitleLanguage("Danish", "dan"),
            new SubtitleLanguage("Dargwa", "dar"),
            new SubtitleLanguage("Dayak", "day"),
            new SubtitleLanguage("Delaware", "del"),
            new SubtitleLanguage("Dinka", "din"),
            new SubtitleLanguage("Divehi", "div"),
            new SubtitleLanguage("Dogri", "doi"),
            new SubtitleLanguage("Dogrib", "dgr"),
            new SubtitleLanguage("Dravidian", "dra"),
            new SubtitleLanguage("Duala", "dua"),
            new SubtitleLanguage("Dutch", "dut"),
            new SubtitleLanguage("Dutch, Middle", "dum"),
            new SubtitleLanguage("Dyula", "dyu"),
            new SubtitleLanguage("Dzongkha", "dzo"),
            new SubtitleLanguage("Efik", "efi"),
            new SubtitleLanguage("Egyptian", "egy"),
            new SubtitleLanguage("Ekajuk", "eka"),
            new SubtitleLanguage("Elamite", "elx"),
            new SubtitleLanguage("English", "eng"),
            new SubtitleLanguage("English, Middle", "enm"),
            new SubtitleLanguage("Erzya", "myv"),
            new SubtitleLanguage("Esperanto", "epo"),
            new SubtitleLanguage("Estonian", "est"),
            new SubtitleLanguage("Ewe", "ewe"),
            new SubtitleLanguage("Ewondo", "ewo"),
            new SubtitleLanguage("Extremaduran", "ext"),
            new SubtitleLanguage("Fang", "fan"),
            new SubtitleLanguage("Fanti", "fat"),
            new SubtitleLanguage("Faroese", "fao"),
            new SubtitleLanguage("Fijian", "fij"),
            new SubtitleLanguage("Filipino", "fil"),
            new SubtitleLanguage("Finnish", "fin"),
            new SubtitleLanguage("Finno-Ugrian", "fiu"),
            new SubtitleLanguage("Fon", "fon"),
            new SubtitleLanguage("French", "fre"),
            new SubtitleLanguage("French, Middle", "frm"),
            new SubtitleLanguage("French, Old", "fro"),
            new SubtitleLanguage("Frisian", "fry"),
            new SubtitleLanguage("Friulian", "fur"),
            new SubtitleLanguage("Fulah", "ful"),
            new SubtitleLanguage("Ga", "gaa"),
            new SubtitleLanguage("Gaelic", "gla"),
            new SubtitleLanguage("Galician", "glg"),
            new SubtitleLanguage("Ganda", "lug"),
            new SubtitleLanguage("Gayo", "gay"),
            new SubtitleLanguage("Gbaya", "gba"),
            new SubtitleLanguage("Geez", "gez"),
            new SubtitleLanguage("Georgian", "geo"),
            new SubtitleLanguage("German", "ger"),
            new SubtitleLanguage("German, Middle", "gmh"),
            new SubtitleLanguage("German, Old", "goh"),
            new SubtitleLanguage("Germanic", "gem"),
            new SubtitleLanguage("Gilbertese", "gil"),
            new SubtitleLanguage("Gondi", "gon"),
            new SubtitleLanguage("Gorontalo", "gor"),
            new SubtitleLanguage("Gothic", "got"),
            new SubtitleLanguage("Grebo", "grb"),
            new SubtitleLanguage("Greek", "ell"),
            new SubtitleLanguage("Greek, Ancient", "grc"),
            new SubtitleLanguage("Guarani", "grn"),
            new SubtitleLanguage("Gujarati", "guj"),
            new SubtitleLanguage("Gwich", "gwi"),
            new SubtitleLanguage("Haida", "hai"),
            new SubtitleLanguage("Haitian", "hat"),
            new SubtitleLanguage("Hausa", "hau"),
            new SubtitleLanguage("Hawaiian", "haw"),
            new SubtitleLanguage("Hebrew", "heb"),
            new SubtitleLanguage("Herero", "her"),
            new SubtitleLanguage("Hiligaynon", "hil"),
            new SubtitleLanguage("Himachali", "him"),
            new SubtitleLanguage("Hindi", "hin"),
            new SubtitleLanguage("Hiri Motu", "hmo"),
            new SubtitleLanguage("Hittite", "hit"),
            new SubtitleLanguage("Hmong", "hmn"),
            new SubtitleLanguage("Hungarian", "hun"),
            new SubtitleLanguage("Hupa", "hup"),
            new SubtitleLanguage("Iban", "iba"),
            new SubtitleLanguage("Icelandic", "ice"),
            new SubtitleLanguage("Ido", "ido"),
            new SubtitleLanguage("Igbo", "ibo"),
            new SubtitleLanguage("Ijo", "ijo"),
            new SubtitleLanguage("Iloko", "ilo"),
            new SubtitleLanguage("Inari Sami", "smn"),
            new SubtitleLanguage("Indic", "inc"),
            new SubtitleLanguage("Indo-European", "ine"),
            new SubtitleLanguage("Indonesian", "ind"),
            new SubtitleLanguage("Ingush", "inh"),
            new SubtitleLanguage("Interlingua", "ina"),
            new SubtitleLanguage("Interlingue", "ile"),
            new SubtitleLanguage("Inuktitut", "iku"),
            new SubtitleLanguage("Inupiaq", "ipk"),
            new SubtitleLanguage("Iranian", "ira"),
            new SubtitleLanguage("Irish", "gle"),
            new SubtitleLanguage("Irish, Middle", "mga"),
            new SubtitleLanguage("Irish, Old", "sga"),
            new SubtitleLanguage("Iroquoian languages", "iro"),
            new SubtitleLanguage("Italian", "ita"),
            new SubtitleLanguage("Japanese", "jpn"),
            new SubtitleLanguage("Javanese", "jav"),
            new SubtitleLanguage("Judeo-Arabic", "jrb"),
            new SubtitleLanguage("Judeo-Persian", "jpr"),
            new SubtitleLanguage("Kabardian", "kbd"),
            new SubtitleLanguage("Kabyle", "kab"),
            new SubtitleLanguage("Kachin", "kac"),
            new SubtitleLanguage("Kalaallisut", "kal"),
            new SubtitleLanguage("Kalmyk", "xal"),
            new SubtitleLanguage("Kamba", "kam"),
            new SubtitleLanguage("Kannada", "kan"),
            new SubtitleLanguage("Kanuri", "kau"),
            new SubtitleLanguage("Kara-Kalpak", "kaa"),
            new SubtitleLanguage("Karachay-Balkar", "krc"),
            new SubtitleLanguage("Karen", "kar"),
            new SubtitleLanguage("Kashmiri", "kas"),
            new SubtitleLanguage("Kashubian", "csb"),
            new SubtitleLanguage("Kawi", "kaw"),
            new SubtitleLanguage("Kazakh", "kaz"),
            new SubtitleLanguage("Khasi", "kha"),
            new SubtitleLanguage("Khmer", "khm"),
            new SubtitleLanguage("Khoisan", "khi"),
            new SubtitleLanguage("Khotanese", "kho"),
            new SubtitleLanguage("Kikuyu", "kik"),
            new SubtitleLanguage("Kimbundu", "kmb"),
            new SubtitleLanguage("Kinyarwanda", "kin"),
            new SubtitleLanguage("Kirghiz", "kir"),
            new SubtitleLanguage("Klingon", "tlh"),
            new SubtitleLanguage("Komi", "kom"),
            new SubtitleLanguage("Kongo", "kon"),
            new SubtitleLanguage("Konkani", "kok"),
            new SubtitleLanguage("Korean", "kor"),
            new SubtitleLanguage("Kosraean", "kos"),
            new SubtitleLanguage("Kpelle", "kpe"),
            new SubtitleLanguage("Kru", "kro"),
            new SubtitleLanguage("Kuanyama", "kua"),
            new SubtitleLanguage("Kumyk", "kum"),
            new SubtitleLanguage("Kurdish", "kur"),
            new SubtitleLanguage("Kurukh", "kru"),
            new SubtitleLanguage("Kutenai", "kut"),
            new SubtitleLanguage("Ladino", "lad"),
            new SubtitleLanguage("Lahnda", "lah"),
            new SubtitleLanguage("Lamba", "lam"),
            new SubtitleLanguage("Lao", "lao"),
            new SubtitleLanguage("Latin", "lat"),
            new SubtitleLanguage("Latvian", "lav"),
            new SubtitleLanguage("Lezghian", "lez"),
            new SubtitleLanguage("Limburgan", "lim"),
            new SubtitleLanguage("Lingala", "lin"),
            new SubtitleLanguage("Lithuanian", "lit"),
            new SubtitleLanguage("Low German", "nds"),
            new SubtitleLanguage("Lozi", "loz"),
            new SubtitleLanguage("Luba-Katanga", "lub"),
            new SubtitleLanguage("Luba-Lulua", "lua"),
            new SubtitleLanguage("Luiseno", "lui"),
            new SubtitleLanguage("Lule Sami", "smj"),
            new SubtitleLanguage("Lunda", "lun"),
            new SubtitleLanguage("Luo", "luo"),
            new SubtitleLanguage("lushai", "lus"),
            new SubtitleLanguage("Luxembourgish", "ltz"),
            new SubtitleLanguage("Macedonian", "mac"),
            new SubtitleLanguage("Madurese", "mad"),
            new SubtitleLanguage("Magahi", "mag"),
            new SubtitleLanguage("Maithili", "mai"),
            new SubtitleLanguage("Makasar", "mak"),
            new SubtitleLanguage("Malagasy", "mlg"),
            new SubtitleLanguage("Malay", "may"),
            new SubtitleLanguage("Malayalam", "mal"),
            new SubtitleLanguage("Maltese", "mlt"),
            new SubtitleLanguage("Manchu", "mnc"),
            new SubtitleLanguage("Mandar", "mdr"),
            new SubtitleLanguage("Mandingo", "man"),
            new SubtitleLanguage("Manipuri", "mni"),
            new SubtitleLanguage("Manobo languages", "mno"),
            new SubtitleLanguage("Manx", "glv"),
            new SubtitleLanguage("Maori", "mao"),
            new SubtitleLanguage("Marathi", "mar"),
            new SubtitleLanguage("Mari", "chm"),
            new SubtitleLanguage("Marshallese", "mah"),
            new SubtitleLanguage("Marwari", "mwr"),
            new SubtitleLanguage("Masai", "mas"),
            new SubtitleLanguage("Mayan languages", "myn"),
            new SubtitleLanguage("Mende", "men"),
            new SubtitleLanguage("Mi", "mic"),
            new SubtitleLanguage("Minangkabau", "min"),
            new SubtitleLanguage("Mirandese", "mwl"),
            new SubtitleLanguage("Miscellaneous languages", "mis"),
            new SubtitleLanguage("Mohawk", "moh"),
            new SubtitleLanguage("Moksha", "mdf"),
            new SubtitleLanguage("Moldavian", "mol"),
            new SubtitleLanguage("Mon-Khmer", "mkh"),
            new SubtitleLanguage("Mongo", "lol"),
            new SubtitleLanguage("Mongolian", "mon"),
            new SubtitleLanguage("Montenegrin", "mne"),
            new SubtitleLanguage("Mossi", "mos"),
            new SubtitleLanguage("Multiple languages", "mul"),
            new SubtitleLanguage("Munda languages", "mun"),
            new SubtitleLanguage("Nahuatl", "nah"),
            new SubtitleLanguage("Nauru", "nau"),
            new SubtitleLanguage("Navajo", "nav"),
            new SubtitleLanguage("Ndebele, North", "nde"),
            new SubtitleLanguage("Ndebele, South", "nbl"),
            new SubtitleLanguage("Ndonga", "ndo"),
            new SubtitleLanguage("Neapolitan", "nap"),
            new SubtitleLanguage("Nepal Bhasa", "new"),
            new SubtitleLanguage("Nepali", "nep"),
            new SubtitleLanguage("Nias", "nia"),
            new SubtitleLanguage("Niger-Kordofanian", "nic"),
            new SubtitleLanguage("Nilo-Saharan", "ssa"),
            new SubtitleLanguage("Niuean", "niu"),
            new SubtitleLanguage("Nogai", "nog"),
            new SubtitleLanguage("North American", "nai"),
            new SubtitleLanguage("Northern Sami", "sme"),
            new SubtitleLanguage("Northern Sotho", "nso"),
            new SubtitleLanguage("Norwegian Bokmal", "nob"),
            new SubtitleLanguage("Norwegian Nynorsk", "nno"),
            new SubtitleLanguage("Norwegian", "nor"),
            new SubtitleLanguage("Nubian languages", "nub"),
            new SubtitleLanguage("Nyamwezi", "nym"),
            new SubtitleLanguage("Nyankole", "nyn"),
            new SubtitleLanguage("Nyoro", "nyo"),
            new SubtitleLanguage("Nzima", "nzi"),
            new SubtitleLanguage("Occitan", "oci"),
            new SubtitleLanguage("Ojibwa", "oji"),
            new SubtitleLanguage("Oriya", "ori"),
            new SubtitleLanguage("Oromo", "orm"),
            new SubtitleLanguage("Osage", "osa"),
            new SubtitleLanguage("Ossetian", "oss"),
            new SubtitleLanguage("Otomian languages", "oto"),
            new SubtitleLanguage("Pahlavi", "pal"),
            new SubtitleLanguage("Palauan", "pau"),
            new SubtitleLanguage("Pali", "pli"),
            new SubtitleLanguage("Pampanga", "pam"),
            new SubtitleLanguage("Pangasinan", "pag"),
            new SubtitleLanguage("Panjabi", "pan"),
            new SubtitleLanguage("Papiamento", "pap"),
            new SubtitleLanguage("Papuan", "paa"),
            new SubtitleLanguage("Persian", "per"),
            new SubtitleLanguage("Persian, Old", "peo"),
            new SubtitleLanguage("Philippine", "phi"),
            new SubtitleLanguage("Phoenician", "phn"),
            new SubtitleLanguage("Pohnpeian", "pon"),
            new SubtitleLanguage("Polish", "pol"),
            new SubtitleLanguage("Brazilian Portuguese", "pob"),
            new SubtitleLanguage("Portuguese", "por"),
            new SubtitleLanguage("Prakrit languages", "pra"),
            new SubtitleLanguage("Proven", "pro"),
            new SubtitleLanguage("Pushto", "pus"),
            new SubtitleLanguage("Quechua", "que"),
            new SubtitleLanguage("Raeto-Romance", "roh"),
            new SubtitleLanguage("Rajasthani", "raj"),
            new SubtitleLanguage("Rapanui", "rap"),
            new SubtitleLanguage("Rarotongan", "rar"),
            new SubtitleLanguage("Romance", "roa"),
            new SubtitleLanguage("Romanian", "rum"),
            new SubtitleLanguage("Romany", "rom"),
            new SubtitleLanguage("Rundi", "run"),
            new SubtitleLanguage("Russian", "rus"),
            new SubtitleLanguage("Salishan languages", "sal"),
            new SubtitleLanguage("Samaritan Aramaic", "sam"),
            new SubtitleLanguage("Sami languages", "smi"),
            new SubtitleLanguage("Samoan", "smo"),
            new SubtitleLanguage("Sandawe", "sad"),
            new SubtitleLanguage("Sango", "sag"),
            new SubtitleLanguage("Sanskrit", "san"),
            new SubtitleLanguage("Santali", "sat"),
            new SubtitleLanguage("Sardinian", "srd"),
            new SubtitleLanguage("Sasak", "sas"),
            new SubtitleLanguage("Scots", "sco"),
            new SubtitleLanguage("Selkup", "sel"),
            new SubtitleLanguage("Semitic", "sem"),
            new SubtitleLanguage("Serbian", "scc"),
            new SubtitleLanguage("Serer", "srr"),
            new SubtitleLanguage("Shan", "shn"),
            new SubtitleLanguage("Shona", "sna"),
            new SubtitleLanguage("Sichuan Yi", "iii"),
            new SubtitleLanguage("Sicilian", "scn"),
            new SubtitleLanguage("Sidamo", "sid"),
            new SubtitleLanguage("Sign Languages", "sgn"),
            new SubtitleLanguage("Siksika", "bla"),
            new SubtitleLanguage("Sindhi", "snd"),
            new SubtitleLanguage("Sinhalese", "sin"),
            new SubtitleLanguage("Sino-Tibetan", "sit"),
            new SubtitleLanguage("Siouan languages", "sio"),
            new SubtitleLanguage("Skolt Sami", "sms"),
            new SubtitleLanguage("Slave", "den"),
            new SubtitleLanguage("Slavic", "sla"),
            new SubtitleLanguage("Slovak", "slo"),
            new SubtitleLanguage("Slovenian", "slv"),
            new SubtitleLanguage("Sogdian", "sog"),
            new SubtitleLanguage("Somali", "som"),
            new SubtitleLanguage("Songhai", "son"),
            new SubtitleLanguage("Soninke", "snk"),
            new SubtitleLanguage("Sorbian languages", "wen"),
            new SubtitleLanguage("Sotho, Southern", "sot"),
            new SubtitleLanguage("South American", "sai"),
            new SubtitleLanguage("Southern Altai", "alt"),
            new SubtitleLanguage("Southern Sami", "sma"),
            new SubtitleLanguage("Spanish", "spa"),
            new SubtitleLanguage("Sukuma", "suk"),
            new SubtitleLanguage("Sumerian", "sux"),
            new SubtitleLanguage("Sundanese", "sun"),
            new SubtitleLanguage("Susu", "sus"),
            new SubtitleLanguage("Swahili", "swa"),
            new SubtitleLanguage("Swati", "ssw"),
            new SubtitleLanguage("Swedish", "swe"),
            new SubtitleLanguage("Syriac", "syr"),
            new SubtitleLanguage("Tagalog", "tgl"),
            new SubtitleLanguage("Tahitian", "tah"),
            new SubtitleLanguage("Tai", "tai"),
            new SubtitleLanguage("Tajik", "tgk"),
            new SubtitleLanguage("Tamashek", "tmh"),
            new SubtitleLanguage("Tamil", "tam"),
            new SubtitleLanguage("Tatar", "tat"),
            new SubtitleLanguage("Telugu", "tel"),
            new SubtitleLanguage("Tereno", "ter"),
            new SubtitleLanguage("Tetum", "tet"),
            new SubtitleLanguage("Thai", "tha"),
            new SubtitleLanguage("Tibetan", "tib"),
            new SubtitleLanguage("Tigre", "tig"),
            new SubtitleLanguage("Tigrinya", "tir"),
            new SubtitleLanguage("Timne", "tem"),
            new SubtitleLanguage("Tiv", "tiv"),
            new SubtitleLanguage("Tlingit", "tli"),
            new SubtitleLanguage("Tok Pisin", "tpi"),
            new SubtitleLanguage("Tokelau", "tkl"),
            new SubtitleLanguage("Tonga", "tog"),
            new SubtitleLanguage("Tonga", "ton"),
            new SubtitleLanguage("Tsimshian", "tsi"),
            new SubtitleLanguage("Tsonga", "tso"),
            new SubtitleLanguage("Tswana", "tsn"),
            new SubtitleLanguage("Tumbuka", "tum"),
            new SubtitleLanguage("Tupi languages", "tup"),
            new SubtitleLanguage("Turkish", "tur"),
            new SubtitleLanguage("Turkish, Ottoman", "ota"),
            new SubtitleLanguage("Turkmen", "tuk"),
            new SubtitleLanguage("Tuvalu", "tvl"),
            new SubtitleLanguage("Tuvinian", "tyv"),
            new SubtitleLanguage("Twi", "twi"),
            new SubtitleLanguage("Udmurt", "udm"),
            new SubtitleLanguage("Ugaritic", "uga"),
            new SubtitleLanguage("Uighur", "uig"),
            new SubtitleLanguage("Ukrainian", "ukr"),
            new SubtitleLanguage("Umbundu", "umb"),
            new SubtitleLanguage("Undetermined", "und"),
            new SubtitleLanguage("Urdu", "urd"),
            new SubtitleLanguage("Uzbek", "uzb"),
            new SubtitleLanguage("Vai", "vai"),
            new SubtitleLanguage("Venda", "ven"),
            new SubtitleLanguage("Vietnamese", "vie"),
            new SubtitleLanguage("Volap", "vol"),
            new SubtitleLanguage("Votic", "vot"),
            new SubtitleLanguage("Wakashan languages", "wak"),
            new SubtitleLanguage("Walamo", "wal"),
            new SubtitleLanguage("Walloon", "wln"),
            new SubtitleLanguage("Waray", "war"),
            new SubtitleLanguage("Washo", "was"),
            new SubtitleLanguage("Welsh", "wel"),
            new SubtitleLanguage("Wolof", "wol"),
            new SubtitleLanguage("Xhosa", "xho"),
            new SubtitleLanguage("Yakut", "sah"),
            new SubtitleLanguage("Yao", "yao"),
            new SubtitleLanguage("Yapese", "yap"),
            new SubtitleLanguage("Yiddish", "yid"),
            new SubtitleLanguage("Yoruba", "yor"),
            new SubtitleLanguage("Yupik languages", "ypk"),
            new SubtitleLanguage("Zande", "znd"),
            new SubtitleLanguage("Zapotec", "zap"),
            new SubtitleLanguage("Zenaga", "zen"),
            new SubtitleLanguage("Zhuang", "zha"),
            new SubtitleLanguage("Zulu", "zul"),
            new SubtitleLanguage("Zuni", "zun"),
        };

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
