namespace Codename_TALaT_CS
{
    public class TextAdventureLauncher
    {

        public const string currentVer =
#if DEBUG
            "DEBUG";
#else
            "1.23.1306.0";
#endif


        public static readonly string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"TALaT\\Files");
        public static readonly string gamePathBase = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"TALaT");

        public static Translation translationEn;
        public static List<Translation> allTranslations = new List<Translation>();
        private static StreamWriter? instanceLock;


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
            if (File.Exists(Path.Combine(gamePathBase, "instance.lock")))
            {
                try
                {
                    File.Delete(Path.Combine(gamePathBase, "instance.lock")); //Try to delete the file, to see, wheather another instance is running

                } 
                catch
                {
                    //Can't delete file because other instance is running
                    Environment.Exit(1);
                }
            }
            instanceLock = new(Path.Combine(gamePathBase, "instance.lock"));

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
            SFM.SFM.baseDirectory = Path.Combine(gamePath, "SFM-Files");

            Log.Shared.LogL("Loading storys");

            if (File.Exists(Path.Combine(gamePath, "LauncherSave\\Storys.save")))
                GlobalManager.LoadAllStorys(DataTypeStore.Read.TopLevelRegion(
                    File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\Storys.save")).Split(";", StringSplitOptions.RemoveEmptyEntries)
                    )[0]);
            if (File.Exists(Path.Combine(gamePath, "LauncherSave\\creatorkeys.save")))
                GlobalManager.creatorKeys = new( DataTypeStore.Read.TopLevelRegion(
                    File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\creatorkeys.save")).Split(";", StringSplitOptions.RemoveEmptyEntries)
                    )[0]);


            Log.Shared.LogL("Starting Main menu UI");
            UI.MainMenu(translationEn);



        }
    }
}