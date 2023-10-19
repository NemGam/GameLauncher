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
using System.Runtime.CompilerServices;
using GameLauncher.ViewModels;

namespace GameLauncher.Models
{
    internal class NetworkModule
    {
        private static HttpClient? httpClient;
        private static MongoClient? dbClient;

        //Should move out from the open source, however has only permission to read one and only collection
        //so not that scary
        private const string dbUsername = "readeronly";
        private const string dbPassword = "testtest";

        private static bool isInitialized;
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




            //if (CheckForLauncherUpdates().Result)
            //{
            //   UpdateLauncher();
            //}

            isInitialized = true;

            return 0;
        }

        public async static Task<Dictionary<string, Game>?> GetAvailableGamesList()
        {
            if (!isInitialized) throw new Exception("Network Module has not been initialized yet!");
            var db = dbClient!.GetDatabase("GamesList");

            Debug.WriteLine("Network Module: Getting collection");
            var colls = db.GetCollection<Game>("Games");

            Debug.WriteLine("Network Module: got collection");

            try
            {
                Debug.WriteLine("Network Module: getting data from collections");
                //Fetching available games from the database
                var gameList = await colls.Find(Builders<Game>.Filter.Empty)
                    .Project(Builders<Game>.Projection.Exclude("_id"))
                    .As<Game>()
                    .ToListAsync().ConfigureAwait(false);
                Debug.WriteLine("Network Module: got data from collections");

                Dictionary<string, Game> availableGames = gameList.ToDictionary(x => x.GameName, x => x);

                Debug.WriteLine($"The list of games on the server is:");
                foreach (var game in availableGames)
                {
                    Debug.WriteLine($"info: {game.Key}");
                }
                return availableGames;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR");
                return null;
            }
        }

        /*
        //TODO:Should be async method to self update
        private async static void UpdateLauncher()
        {
            throw new NotImplementedException();
        }

        private async static Task<bool> CheckForLauncherUpdates()
        {
            //TODO: Self update from github, MUST BLOCK GAMES CHECK TO PREVENT ISSUES
            return false;
        }
        */
        //https://github.com/USER/PROJECT/releases/latest/download/PACKAGE_NAME
        //Download latest release of the project.
        //Requires the gameName to match the project and the zip names!

        public static async Task<int> DownloadGame(Game game)
        {
            if (httpClient == null) { throw new Exception("No HttpClient Available"); };
            //var stream = await httpClient.GetStreamAsync($"https://github.com/NemGam/{game.GameName}/releases/latest/download/{game.GameName}.zip");
            using (System.IO.Stream stream = await httpClient.GetStreamAsync(game.URL))
            {
                return await FilesModule.InstallGameAsync(stream, game);
            }

        }
    }
}
