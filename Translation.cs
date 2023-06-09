using DataTypeStore;
using NUnit.Framework;

namespace Codename_TALaT_CS
{


    public class Translation
    {
        public string language;
        public Dictionary<string, string> translation;
        public Translation(Region? loadFromRegion = null)
        {

            translation = StandartTranslation();
            if (loadFromRegion == null)
            {
                language = "english";
                return;
            }
            language = loadFromRegion.FindDirectValue("language").value;
            loadFromRegion.FindSubregionWithNameArray("T").ToList().ForEach(x =>
            {
                string id = x.FindDirectValue("id").value;
                if (!translation.ContainsKey(id)) throw new Exception($"Translation.Invalid_LoadID: For language {language}");
                translation[id] = x.FindDirectValue("t").value;
            });


        }

        /// <summary>
        /// Loads all standart translations in the english language to be either a basis for another language, or be used as default language.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> StandartTranslation()
        {
            return new()
            {
                { "MainMenuHead", "Main Menu" },
                { "", "" },
                { "MainMenuY", "Press y to start a story" },
                { "NoStorys", "Whoops, quite empty in here... Try to import some storys first!" },
                { "NotRecignisedCommend", "\"%0%\" isn't a recognised command?!" },


            };
        }
        /// <summary>
        /// Get the translation value for initial and replace all escape codes (%index%) with their corrosponding indexes.
        /// </summary>
        /// <param name="initial"></param>
        /// <param name="usableValues"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string Get(string initial, string[]? usableValues = null)
        {

            if (!translation.TryGetValue(initial, out var value)) throw new Exception($"Internal.Unknown_Translation: The translation for \"{initial}\" doesn't exist");
            if (usableValues == null) return value;
            string result = "";
            bool inEscape = false;
            string escapingValue = "";
            foreach (char c in value)
            {
                if (c == '%')
                {
                    if (inEscape)
                    {
                        if (int.TryParse(escapingValue, out int parsed) || parsed > usableValues.Length) throw new Exception("Translation.Invalid_Escape_Value");
                        result += usableValues[parsed];
                        inEscape = false;
                        escapingValue = "";
                        continue;
                    }
                    inEscape = true;
                    continue;

                }
                if (inEscape)
                {
                    escapingValue += c;
                    continue;
                }
                result += c;
            }
            return result;
        }
    }
}
