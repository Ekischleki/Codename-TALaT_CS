using NUnit.Framework.Internal;
using SFM;
using System.Collections.ObjectModel;

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
                DataTypeStoreLib.Read.TopLevelRegion(File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\TranslationSave")).Split(';')).ForEach(x =>
                    allTranslations.Add(new(x))
                );


            }
            Log.Shared.LogL("Preparing internal non-save files manager (SFM)");
            SFM.SFMEngine.baseDirectory = Path.Combine(gamePath, "SFM-Files");
            if (!Directory.Exists(SFMEngine.baseDirectory))
            {
                Directory.CreateDirectory(SFMEngine.baseDirectory);
                SFMEngine.FirstTimeInitializeDirectory();
            }
            

            Log.Shared.LogL("Loading storys");

            if (File.Exists(Path.Combine(gamePath, "LauncherSave\\Storys.save")))
                GlobalManager.LoadAllStorys(DataTypeStoreLib.Read.TopLevelRegion(
                    File.ReadAllText(Path.Combine(gamePath, "LauncherSave\\Storys.save")).Split(";", StringSplitOptions.RemoveEmptyEntries)
                    )[0]);
            

            Log.Shared.LogL("Clearing unused files");
            InternalFileEmulation.RemoveUnused();

            ObservableCollection<Story> storys;

            storys = DataTypeStoreLib.Automatic.ConvertRegion( DataTypeStoreLib.Automatic.Object(GlobalManager.allStories , "STORYS")) as ObservableCollection<Story>;



            Log.Shared.LogL("Starting Main menu UI");
            UI.MainMenu(translationEn);



        }
    }
}