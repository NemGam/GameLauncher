using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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

            //Check what games are installed
            return di.GetDirectories()
                .ToDictionary(x => x.Name, x => new Game(x.Name, x.FullName, Game.State.Installed));
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


            //Serialize data TODO: Move to the NetworkModule
            //string json = System.Text.Json.JsonSerializer.Serialize(values);
            //File.WriteAllText(dataPath, json);


        }

        public static async void InstallGame(Stream stream, string gameName)
        {
            FileStream finalFileStream = new FileStream($"{LauncherPath}/Games/{gameName}.zip", FileMode.Create);
            await stream.CopyToAsync(finalFileStream);
        }
    }
}
