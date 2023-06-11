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
                    case "save":
                        File.WriteAllText(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\Storys.save"), GlobalManager.SaveAllStorys.RegionSaveString);
                        break;

                    case "y":
                        DisplayAllStorys();
                        break;
                    case "update":
                        Task[] update = new Task[GlobalManager.allStories.Count];
                        for (int i = 0; i < update.Length; i++) 
                        {
                            GlobalManager.allStories[i].UpdateStory();
                            //update[i] = new(() => { GlobalManager.allStories[i].UpdateStory(); });
                            //update[i].Start();
                        }
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
        public static void DisplayAllStorys()
        {
            Console.Clear();
            for (int i = 0; i < GlobalManager.allStories.Count; i++)
            {
                Story story = GlobalManager.allStories[i];
                Console.WriteLine($"-----{i}: {story.storyName}-----");
                Console.WriteLine(story.descriptions.First(x => x.path == story.storyDefaultLangVal).Content);
            }
            Console.WriteLine("---------------------\nEnter the number of the story you want to start.");
            if (!int.TryParse(Console.ReadLine(), out int startStory))
            {
                Console.WriteLine("That story number is invalid.");
                return;
            }
            Console.Clear();
            GlobalManager.allStories[startStory].RunStory();
        }
    }
}
