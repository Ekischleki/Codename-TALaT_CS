using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codename_TALaT_CS
{
    internal class UI
    {

        public static void MainMenu(Translation translation)
        {
            while (true)
            {
                Console.WriteLine(translation.Get("MainMenuHead"));
                Console.WriteLine(translation.Get("MainMenuY"));
                string command = Console.ReadLine() ?? "";
                switch (command)
                {
                    default:
                        Console.WriteLine(translation.Get("NotRecignisedCommend", new string[] { command }));
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
