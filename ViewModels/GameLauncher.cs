using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows.Shapes;

namespace GameLauncher.ViewModels
{
    public class GameLauncher : INotifyPropertyChanged
    {
        public ObservableCollection<Game>? AvailableGames { get; set; }
        //public ObservableCollection<Game> InstalledGames { get; set; }

        private FilesModule filesModule;

        public GameLauncher()
        {
            filesModule = new FilesModule();
            Initialize();
            AvailableGames = GetGamesList();

            //Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)
            //GetCachedData();
            //AvailableGames = await GetAvailableGamesAsync();
            //InstalledGames = new ObservableCollection<GameInfo>();
        }

        private ObservableCollection<Game>? GetGamesList()
        {
            ObservableCollection<Game> games = new();
            FillInstalledGames(ref games);
            return games;
        }

        /// <summary>
        /// Looks in Launcher folder for already installed games.
        /// </summary>
        /// <returns>Collection of installed games</returns>
        private void FillInstalledGames(ref ObservableCollection<Game> listToFill)
        {
            DirectoryInfo di = new($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/NemGam/Launcher/Games");
            //Check what games are installed
            foreach (var game in di.GetDirectories())
            {
                listToFill.Add(new Game(game.Name, game.FullName, Game.State.Ready));
            }
        }
        private async void GetAvailableGamesAsync(ref ObservableCollection<Game> listToFill)
        {
            ObservableCollection<Game> games = new();


        }

        private void Initialize()
        {
            //Check if all the folders exist already
            //TODO:Handle directory integrity

            FilesModule.Initilize();
        }

        private void GetCachedData()
        { 
            //TODO:Handle errors
            string path = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/NemGam/Launcher/Data";
            FileInfo fi = new FileInfo($"{path}/data.json");
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                di.Create();
            }

            if (!fi.Exists)
            {
                fi.Create().Dispose();
            }
            

            StreamWriter sw = new($"{path}/data.json");
            
            
            sw.Close();
        }




        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
