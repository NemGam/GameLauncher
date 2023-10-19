using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO.Compression;

namespace GameLauncher
{
    internal static class FilesModule
    {
        //Path to the "Launcher" folder
        public static string? LauncherPath { get; private set; }

        internal static Dictionary<string, Game> GetInstalledGamesList()
        {
            if (LauncherPath == null) throw new Exception("FilesModule was not initialized!");

            DirectoryInfo di = new($"{LauncherPath}/Games");
            Dictionary<string, string> versions = GetVersionData();
            //Check what games are installed
            //TODO:Handle games that are not from database
            return di.GetDirectories()
                .ToDictionary(x => x.Name,
                x => new Game(x.Name, x.FullName, versions == null ? "0.0.0" : versions[x.Name], Game.State.Installed));
        }

        /// <summary>
        /// Initialize this module. Must be called first, before any other methods.
        /// </summary>
        /// <returns>0 on success, error code otherwise</returns>
        internal static int Initilize()
        {
            //No error codes yet :)
            //TODO:Add error handling
            LauncherPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/NemGam/Launcher";
            DirectoryInfo di = new(LauncherPath);
            if (!di.Exists)
            {
                di.Create();
                di.CreateSubdirectory($"./Data");
                di.CreateSubdirectory($"./Games");
            }
            return 0;
        }

        private static void UpdateVersionFile(Game game, bool shouldDelete = false)
        {
            string dataPath = $"{LauncherPath}/Data/data.json";
            var newFile = GetVersionData();
            newFile ??= new Dictionary<string, string>();

            if (shouldDelete)
            {
                throw new NotImplementedException();
            }
            else
            {
                if (!newFile.ContainsKey(game.GameName))
                {
                    newFile.Add(game.GameName, game.version);
                }
                else
                {
                    newFile[game.GameName] = game.version;
                }
                
            }
            //Check if the version file already contains this game.
            //Update the file if so. Add the new property to the file otherwise.
            
            string json = System.Text.Json.JsonSerializer.Serialize(newFile);
            File.WriteAllText(dataPath, json);
        }

        public static Dictionary<string, string>? GetVersionData()
        {
            //TODO:Handle errors
            string dataPath = $"{LauncherPath}/Data/data.json";
            
            if (!File.Exists(dataPath))
            {
                File.Create(dataPath);
                return null;
            }

            //Deserialize data
            string json = File.ReadAllText(dataPath);
            Dictionary<string, string>? result = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            return result; 
        }

        public static async Task<int> InstallGameAsync(Stream stream, Game game)
        {
            string tempPath = $"{Path.GetTempPath()}/{game.GameName}.zip";
            using (FileStream finalFileStream = new FileStream(tempPath, FileMode.Create))
            {
                UpdateVersionFile(game);
                //Extract the game from the zip
                await stream.CopyToAsync(finalFileStream);
                
            }
            ZipFile.ExtractToDirectory(tempPath, $"{LauncherPath}/Games/{game.GameName}", true);
            File.Delete(tempPath);
            //On success return 0
            return 0;
        }
    }
}
