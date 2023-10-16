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
using System.Windows.Shapes;

namespace GameLauncher.ViewModels
{
    public class GameLauncher : INotifyPropertyChanged
    {
        public ObservableCollection<Game> AvailableGames { get; set; }
        //public ObservableCollection<Game> InstalledGames { get; set; }


        public GameLauncher()
        {
            AvailableGames = new ObservableCollection<Game>();
            this.Initialize();
            //Get installed games
            foreach (var game in FilesModule.GetInstalledGamesList())
            {
                AvailableGames.Add(new Game(game, $"{FilesModule.LauncherPath}/{game}", Game.State.Ready));
            }
            
        }

        private void Initialize()
        {
            FilesModule.Initilize();
            NetworkModule.Initialize();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
