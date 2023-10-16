using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Windows;
using System.Diagnostics;
using MongoDB.Bson.Serialization;

namespace GameLauncher
{
    internal class NetworkModule
    {
        private static HttpClient? httpClient;
        private static MongoClient? dbClient;

        //Might wanna move out from the open source, however has only permission to read one and only collection
        //so not that scary
        const string dbUsername = "readeronly";
        const string dbPassword = "testtest";
        public static int Initialize() 
        {


            httpClient = new HttpClient();
            if (httpClient == null)
            {
                MessageBox.Show("Failed to create an HttpClient.", "ERROR");
                throw new Exception("Failed to create an HttpClient.");
            }

            dbClient = new MongoClient($"mongodb+srv://{dbUsername}:{dbPassword}@gamestorage.fwevppr.mongodb.net");
            
            if (dbClient == null)
            {
                MessageBox.Show("Failed to create a database client.", "ERROR");
                throw new Exception("Failed to create a MongoClient.");
            }

            GetAvailableGamesList();



            if (CheckForLauncherUpdates())
            {
                UpdateLauncher();
            }
            
            

            return 0;
        }

        private async static void GetAvailableGamesList()
        {
            var db = dbClient!.GetDatabase("GamesList");
            var colls = db.GetCollection<Game>("Games");

            try
            {
                //Fetching available games from the database
                var availableGames = await colls.Find(Builders<Game>.Filter.Empty)
                    .Project(Builders<Game>.Projection.Exclude("_id"))
                    .As<Game>()
                    .ToListAsync();

                Debug.WriteLine($"The list of games on the server is:");
                foreach (var game in availableGames)
                {
                    Debug.WriteLine($"info: {game}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
            }
        }

        //TODO:Should be async method to self update
        private static void UpdateLauncher()
        {
            throw new NotImplementedException();
        }

        private static bool CheckForLauncherUpdates()
        {
            //TODO: Self update from github 
            return false;
        }

        //https://github.com/USER/PROJECT/releases/latest/download/PACKAGE_NAME
        //Download latest release of the project.
        //Requires the gameName to match the project and the zip names!

        public static async void DownloadGame(string gameName)
        {
            if (httpClient == null) { throw new Exception("No HttpClient Available"); };
            var stream = await httpClient.GetStreamAsync($"https://github.com/NemGam/{gameName}/releases/latest/download/{gameName}.zip");
            FilesModule.InstallGame(stream, gameName);
        }
    }
}
