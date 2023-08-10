using ObjectStoreE;
using RSAKeygenLib;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Codename_TALaT_CS
{

    public static class GlobalManager
    {

        static GlobalManager()
        {

        }

        private static readonly NotifyCollectionChangedAction[] saveImportantCollectionChangteEvents = new NotifyCollectionChangedAction[] { NotifyCollectionChangedAction.Remove, NotifyCollectionChangedAction.Add, NotifyCollectionChangedAction.Replace, NotifyCollectionChangedAction.Reset };
        private static async void DynamicSaveStorys(object? sender, NotifyCollectionChangedEventArgs e)
        {
            
            if (saveImportantCollectionChangteEvents.Contains(e.Action))
            {
                await File.WriteAllTextAsync(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\Storys.save"), SaveAllStorys.RegionSaveString);

            }
        }

        public static List<Translation> allTranslations;

        public static ObservableCollection<Story> allStories = new();
        public static List<Story> developmentStorys = new();
        private static PublicPrivateKeypair? creatorKeys = null;

        public static PublicPrivateKeypair CreatorKeys
        {
            get
            { 
                if (creatorKeys == null)
                {
                    if (File.Exists(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\creatorkeys.save")))
                        creatorKeys = new(Read.TopLevelRegion(
                            File.ReadAllText(Path.Combine(TextAdventureLauncher.gamePath, "LauncherSave\\creatorkeys.save")).Split(";", StringSplitOptions.RemoveEmptyEntries)
                            )[0]);
                    Log.Shared.LogL("Creating dev keys\nThis might take a few seconds");
                    creatorKeys = Keygen.GenerateRSAKeypair(256);
                }

                return creatorKeys;
            }

        }

        
        public static void DynamicSave()
        {
            
        }
        public static void LoadAllStorys(Region region)
        {
            region.FindSubregionWithName("Imported").FindSubregionWithNameArray("Story").ToList().ForEach(x => allStories.Add(new(x)));
            region.FindSubregionWithName("Dev").FindSubregionWithNameArray("Story").ToList().ForEach(x => developmentStorys.Add(new(x)));


        }
        public static Region SaveAllStorys
        {
            get
            {
                Region result = new("AllStorys");
                result.SubRegions.Add(new("Imported"));
                result.SubRegions.Add(new("Dev"));

                allStories.ToList().ForEach(x => result.SubRegions[0].SubRegions.Add(x.SaveStory));
                return result;
            }
        }

    }

}
