using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher
{
    internal class FilesModule
    {
        internal FilesModule() 
        { 
        
        }

        internal List<string> GetInstalledGames() 
        {
            
        }

        internal static bool Initilize()
        {
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/NemGam/Launcher";
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
                di.CreateSubdirectory($"./Data");
                di.CreateSubdirectory($"./Games");
            }
            return true;
        }
    }
}
