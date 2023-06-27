using DataTypeStore;
using RSAKeygenLib;
using SFM;
using System.Diagnostics;
using System.Numerics;

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
                        break;
                    case "save":
                        File.WriteAllText(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\Storys.save"), GlobalManager.SaveAllStorys.RegionSaveString);
                        if (GlobalManager.creatorKeys != null)
                            File.WriteAllText(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\creatorkeys.save"), GlobalManager.creatorKeys.Save.RegionSaveString);
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
                        //GeneralInstaller.GeneralInstaller.Main();
                        break;
                    case "lupdate":
                        Installer.PackageDownloader.InstallIfUpdate(TextAdventureLauncher.gamePathBase);
                        break;
                    case "dev":
                        DevMode();
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine(translation.Get("NotRecignisedCommend", new string[] { command }));
                        SFM.SFM.SaveFile(command);
                        Console.ReadKey();
                        break;
                }
            }



        }

        public static void DevMode(Story? currentDevStory = null)
        {
            Log.Shared.LogL("Checking keys");
            if (GlobalManager.creatorKeys == null)
            {
                Log.Shared.LogL("Creating dev keys\nThis might take a few seconds");
                GlobalManager.creatorKeys = Keygen.GenerateRSAKeypair(500);
            }
            Log.Shared.LogL("Creating a new template");
            string devPath = Path.Combine(TextAdventureLauncher.gamePath, "StoryDev");
            if (Directory.Exists(devPath))
                Directory.Delete(devPath, true);
            Directory.CreateDirectory(devPath);

            Directory.CreateDirectory(Path.Combine(devPath, "Code"));
            if (currentDevStory == null) //Story setup
            {
                currentDevStory = new Story();
                Console.Clear();
                Console.WriteLine("Enter the storys name");
                currentDevStory.storyName = Console.ReadLine() ?? "NullError";
                Console.Clear();
                Console.WriteLine("Enter the storys start file name (in root dir)");
                string startFile = Console.ReadLine() ?? "NullError";
                currentDevStory.codeFiles.Add(new($"\\{startFile}", $"name main;\ntype generic;\nstart\n{{\n#Place your starting code in here.\n}};"));
                currentDevStory.storyStart = currentDevStory.codeFiles[0];

                //Automatic apply
                currentDevStory.languages.Add(new(new List<string>() { "english", "eng" }, "English", "0"));
                currentDevStory.storyDefaultLangVal = "0";
                currentDevStory.descriptions.Add(new("0", $"!!!This is the default description generated by the TALaT dev mode.\nYou've created a story with the name of \"{currentDevStory.storyName}\"\n and a default start file of: {currentDevStory.storyStart.path}.\nYou can edit this description in near furute (probably) or directly after export.\nFor this, you can just go into\nStory>Meta>Description>0 and edit it however you like.\nThanks for using TALaT\n-Ekischleki"));



            }
            Log.Shared.LogL("Creating code files.");
            foreach (InternalFileEmulation internalFile in currentDevStory.codeFiles)
            {
                File.WriteAllText(Path.Combine(Path.Combine(devPath, "Code"), internalFile.path.Substring(1)), internalFile.Content);

            }
            Process.Start("explorer.exe", devPath);

            PackageStory(currentDevStory);




        }

        public static void PackageStory(Story story)
        {
            string packagePath = Path.Combine(Path.GetTempPath(), $"Packge-{Random.Shared.Next()}");

            Directory.CreateDirectory(packagePath);
            string storyPath = Path.Combine(packagePath, "Story");
            Directory.CreateDirectory(storyPath);
            Directory.CreateDirectory(Path.Combine(storyPath, "Code"));
            Directory.CreateDirectory(Path.Combine(storyPath, "Meta"));
            foreach (InternalFileEmulation internalFile in story.codeFiles)
            {
                File.WriteAllText(Path.Combine(Path.Combine(storyPath, "Code"), internalFile.path.Substring(1)), internalFile.Content);

            }
            Directory.CreateDirectory(Path.Combine(storyPath, "Meta\\Description"));
            foreach (InternalFileEmulation internalFile in story.descriptions)
            {
                File.WriteAllText(Path.Combine(Path.Combine(storyPath, "Meta\\Description"), internalFile.path), internalFile.Content);

            }
            StreamWriter supportedLangsStream = new(Path.Combine(storyPath, "Meta\\SupportedLanguages"));
            foreach (SupportedLanguage supportedLanguage in story.languages)
            {
                supportedLangsStream.WriteLine($"{string.Join(',', supportedLanguage.launcherLanguage)}={supportedLanguage.storyLanguage}:{supportedLanguage.internalReference};");
            }
            supportedLangsStream.Flush();
            supportedLangsStream.Dispose();
            File.WriteAllText(Path.Combine(storyPath, "Meta\\Launcher"), $"StoryName={story.storyName};StoryStart={story.storyStart.path};StoryDefaultLangVal={story.storyDefaultLangVal};StoryUseLauncherFunctions={story.storyUseLauncherFunctions};UpdatesVer={story.updatesVer};UpdatesPackage={story.updatesPackage};UpdatesRandomStoryID={Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()}-{Random.Shared.Next()};");
            string hashResult = HashFolderUtils.HashFolder(new string[] { Path.Combine(storyPath, "Code"), Path.Combine(storyPath, "Meta") });
            BigInteger hashBigIntiger = BigInteger.Parse(hashResult, System.Globalization.NumberStyles.HexNumber);
            Log.Shared.LogE("Signing hash: " + hashBigIntiger);


            Region signatureRegion = new("Signature");
            signatureRegion.SubRegions.Add(GlobalManager.creatorKeys.PublicKey.Save);
            signatureRegion.directValues.Add(new("signedHash", GlobalManager.creatorKeys.PrivateKey.CryptUsingKeypair(hashBigIntiger).ToString(), false));

            File.WriteAllText(Path.Combine(storyPath, "Signature"), signatureRegion.RegionSaveString);

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
