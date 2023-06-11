using DataTypeStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TASI;

namespace Codename_TALaT_CS
{
    
    public class GlobalManager
    {
        public static List<Translation> allTranslations;
        public static List<Story> allStories = new();

        public static void LoadAllStorys(Region region)
        {
            region.FindSubregionWithNameArray("Story").ToList().ForEach(x => allStories.Add(new(x)));
        }
        public static Region SaveAllStorys
        {
            get
            {
                Region result = new("AllStorys");
                allStories.ForEach(x => result.SubRegions.Add(x.SaveStory));
                return result;
            }
        }

    }

}
