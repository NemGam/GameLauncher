using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Shapes;

namespace GameLauncher.ViewModels
{
    public class GameLauncher : INotifyPropertyChanged
    {
        public ObservableCollection<Game> AvailableGames { get; set; }
        private Dictionary<string, Game>? InstalledGames;
        private Dictionary<string, Game>? DownloadableGames;
        //public ObservableCollection<Game> InstalledGames { get; set; }


        public GameLauncher()
        {
            this.Initialize();
            Debug.WriteLine("Initialized");
            AvailableGames = new ObservableCollection<Game>(GetGames().Result);
            Debug.WriteLine("Got all the games");
        }

        /// <summary>
        /// Checks 
        /// </summary>
        /// <returns></returns>
        private async Task<List<Game>> GetGames()
        {
            Debug.WriteLine("Getting games");
            //Get installed games
            InstalledGames = FilesModule.GetInstalledGamesList();
            Debug.WriteLine("Got installed games");
            DownloadableGames = await NetworkModule.GetAvailableGamesList().ConfigureAwait(false);
            Debug.WriteLine("Got downloadable games");

            if (DownloadableGames is null)
            {
                return InstalledGames.Values.ToList();
            } 
            else if (InstalledGames is null)
            {
                return DownloadableGames.Values.ToList();
            }

            List<Game> games = new();

            foreach (KeyValuePair<string, Game> game in DownloadableGames)
            {
                if (InstalledGames.ContainsKey(game.Key))
                {
                    //if version in db != local version -> set ready to update
                    if (InstalledGames[game.Key].version != DownloadableGames[game.Key].version)
                        InstalledGames[game.Key].SetUpdateAvailable();
                    games.Add(InstalledGames[game.Key]);
                }
                else
                {
                    games.Add(game.Value);
                }
            }

            //That should never add any new games, but useful for testing
            foreach (KeyValuePair<string, Game> game in InstalledGames)
            {
                if (DownloadableGames.ContainsKey(game.Key)) continue;
                else games.Add(game.Value);
            }
            return games;
        }

        private void Initialize()
        {
            FilesModule.Initilize();
            NetworkModule.Initialize();
        }

        public void LaunchGame(Game game)
        {
            var process = new Process();
            process.StartInfo = new ProcessStartInfo(
                $"{FilesModule.LauncherPath}/Games/{game.GameName}/{game.GameName}.exe");

            //Here are 2 lines that you need
            process.EnableRaisingEvents = true;
            //Just used LINQ for short, usually would use method as event handler
            process.Exited += (s, a) =>
            {
                process.Exited -= (s, a) => { };
                Debug.WriteLine("EXITED");
            };
            process.Start();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
