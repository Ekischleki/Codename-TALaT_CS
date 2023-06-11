using SickFileManager;

namespace Codename_TALaT_CS
{
    public class TextAdventureLauncher
    {
        public static readonly string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TALaT");
        public static Translation translationEn;
        public static List<Translation> allTranslations = new List<Translation>();



        public static void Main()
        {
            if (!File.Exists(gamePath))
            {
                Directory.CreateDirectory(gamePath);
            }
            if (!File.Exists(Path.Combine(gamePath, "logs")))
            {
                Directory.CreateDirectory(Path.Combine(gamePath, "logs"));
            }

            Log.Shared = new(Path.Combine(gamePath, "logs\\MainRuntime.log"), true);
            Log.Shared.LogE("Enabled logger");

            Log.Shared.LogL("Initialising base language");
            translationEn = new();
            Log.Shared.LogL("Checking save path");
           

            if (!File.Exists(Path.Combine(gamePath, "LauncherSave")))
            {
                Directory.CreateDirectory(Path.Combine(gamePath, "LauncherSave"));
            }
            else
            {
                Log.Shared.LogL("Loading translations");
                DataTypeStore.Read.TopLevelRegion(File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\TranslationSave")).Split(';')).ForEach(x =>
                    allTranslations.Add(new(x))
                );


            }
            Log.Shared.LogL("Preparing internal non-save files manager (SFM)");
            if (!Directory.Exists(Path.Combine(gamePath, "SFM-Files")))
            {
                Directory.CreateDirectory(Path.Combine(gamePath, "SFM-Files"));
            }
            SFM.baseDirectory = Path.Combine(gamePath, "SFM-Files");

            Log.Shared.LogL("Loading storys");
            GlobalManager.LoadAllStorys( DataTypeStore.Read.TopLevelRegion(
                File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\Storys.save")).Split(";", StringSplitOptions.RemoveEmptyEntries)
                )[0]);


            Log.Shared.LogL("Starting Main menu UI");
            UI.MainMenu(translationEn);



        }
    }
}