using Newtonsoft.Json;
using PowerMacros.Entities;
using System.Collections.Generic;

namespace PowerMacros.Utils
{
    public static class MacroLoader
    {
        public static List<Macro> LoadMacrosFromSettings()
        {
            if (!string.IsNullOrWhiteSpace(UserSettings.Default.Macros?.ToString()))
            {
                var macrosList = JsonConvert.DeserializeObject<List<Macro>>(UserSettings.Default.Macros?.ToString());
                return macrosList;
            }

            return new List<Macro>();
        }

        public static void SaveMacrosToSettings(List<Macro> macros)
        {
            var json = JsonConvert.SerializeObject(macros);
            UserSettings.Default.Macros = json;
            UserSettings.Default.Save();
        }
    }
}
