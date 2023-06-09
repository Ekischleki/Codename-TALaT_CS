namespace Codename_TALaT_CS
{
    public class TextAdventureLauncher
    {
        public static readonly string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TALaT");
        public static Translation translationEn;



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
            Log.Shared.LogL("Loading translations...");



        }
    }
}