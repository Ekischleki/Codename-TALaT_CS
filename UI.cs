using SickFileManager;

namespace Codename_TALaT_CS
{
    internal class UI
    {

        public static void MainMenu(Translation translation)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(translation.Get("MainMenuHead"));
                Console.WriteLine(translation.Get("MainMenuY"));
                Console.WriteLine(translation.Get("MainMenuImport"));

                string command = Console.ReadLine() ?? "";
                switch (command.ToLower())
                {
                    case "import":
                        Console.Clear();
                        Console.WriteLine(translation.Get("ImportPath", new string[] { translation.Get("CommandExit") }));
                        command = Console.ReadLine() ?? "";
                        if (command.ToLower() == translation.Get("CommandExit")) continue;
                        try
                        {
                            GlobalManager.allStories.Add(new(command));
                        }
                        catch (Exception ex)
                        {
                            Log.Shared.LogN("Story import failed with: " + ex.Message);
                            Console.Clear();
                            Console.WriteLine("The story import failed. This is probably because the package is corrupt.");
                            Console.ReadKey();
                        }

                        Console.WriteLine(GlobalManager.allStories.Last().SaveStory.RegionSaveString);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine(translation.Get("NotRecignisedCommend", new string[] { command }));
                        SFM.SaveFile(command);
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
