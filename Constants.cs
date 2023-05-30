using System.Collections.Generic;

namespace ezrSquared.Constants
{
    public static class constants
    {
        public const string VERSION = "C-1.3.2.0.0";
        public const string VERSION_DATE = "29.05.2023";

        public const string ALPHABET = "ഀaഁaംaഃaഄaഅaആaഇaഈaഉaഊaഋaഌaഎaഏaഐaഒaഓaഔaകaഖaഗaഘaങaചaഛaജaഝaഞaടaഠaഡaഢaണaതaഥaദaധaനaഩaപaഫaബaഭaമaയaരaറaലaളaഴaവaശaഷaസaഹaഺa഻a഼aഽaാaിaീaുaൂaൃaൄaെaേaൈaൊaോaൌa്aൎa൏aൔaൕaൖaൗa൘a൙a൚a൛a൜a൝a൞aൟaൠaൡaൢaൣa൦a൧a൨a൩a൪a൫a൬a൭a൮a൯a൰a൱a൲a൳a൴a൵a൶a൷a൸a൹aൺaൻaർaൽaൾaൿ";

        public static readonly string[] KEYWORDS = { "ഇനം", "ഉം", "അല്ലെങ്കിൽ", "വിപരീതം", "എങ്കിൽ", "വേറെ", "ചെയ്യുക", "എണ്ണുക", "നിന്ന്", "പോലെ", "വരെ", "ഘട്ടം", "എന്നാൽ", "പ്രവർത്തനം", "പ്രത്യേകം", "കൂടെ", "അവസാനം", "കൊടുക്കുക", "തുടരുക", "നിർത്തൂ", "ശ്രമിക്കുക", "പിശക്", "ൽ", "വസ്തു", "ലോകം", "ഉൾപ്പെടുന്നു" };
        public static readonly string[] QEYWORDS = { "f", "l", "e", "c", "t", "n", "w", "fd", "sd", "od", "i", "s", "d", "g", "v" };

        public static readonly Dictionary<string, string[]> SPECIALS = new Dictionary<string, string[]>()
        {
            { "താരതമ്യം_തുല്യമാണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_അസമമമെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_കുറവാണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_വലുതാണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_കുറവോ_തുല്യമോ_ആണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_വലുതോ_തുല്യമോ_ആണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_എല്ലാം_സത്യമാണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "താരതമ്യം_ഏതെങ്കിലും_സത്യമാണെങ്കിൽr", new string[1] { "മറ്റുള്ളവ" } },

            { "bitwise_or", new string[1] { "മറ്റുള്ളവ" } },
            { "bitwise_xor", new string[1] { "മറ്റുള്ളവ" } },
            { "bitwise_and", new string[1] { "മറ്റുള്ളവ" } },
            { "bitwise_left_shift", new string[1] { "മറ്റുള്ളവ" } },
            { "bitwise_right_shift", new string[1] { "മറ്റുള്ളവ" } },
            { "bitwise_not", new string[0] },

            { "കൂട്ടൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "കുറയ്ക്കൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "ഗുണനം", new string[1] { "മറ്റുള്ളവ" } },
            { "ഹരണം", new string[1] { "മറ്റുള്ളവ" } },
            { "അവശേഷം", new string[1] { "മറ്റുള്ളവ" } },
            { "കൃത്യങ്കങ്ങൾ", new string[1] { "മറ്റുള്ളവ" } },

            { "വിപരീതം", new string[0] },
            { "അകത്തുണ്ടെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },

            { "തുല്യമാണെങ്കിൽ", new string[1] { "മറ്റുള്ളവ" } },
            { "സത്യമാണെങ്കിൽ", new string[0] },
            { "ക്രമഫലം", new string[0] },
        };

        public const string RT_DEFAULT = "ഏതെങ്കിലും";
        public const string RT_OVERFLOW = "കവിഞ്ഞൊഴുകുന്നു";
        public const string RT_ILLEGALOP = "നിയമവിരുദ്ധമായ-പ്രവർത്തനം";
        public const string RT_UNDEFINED = "നിർവചിക്കാത്ത-ഇനം";
        public const string RT_KEY = "നിഘണ്ടു";
        public const string RT_INDEX = "സൂചിക";
        public const string RT_ARGS = "വാദം";
        public const string RT_TYPE = "തരം";
        public const string RT_MATH = "കണക്ക്";
        public const string RT_LEN = "നീളം";
        public const string RT_RUN = "പ്രവർത്തിക്കുന്ന";
        public const string RT_IO = "ഇടുക-എടുക്കുക";
    }
}