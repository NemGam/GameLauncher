using GameLauncher.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace GameLauncher.ViewModels;
[BsonIgnoreExtraElements]
public class Game : INotifyPropertyChanged
{

    [BsonElement("GameName")]
    public string GameName { get; }
    private readonly string? localFolder;

    [BsonElement("URL")]
    public string? URL { get; }

    [BsonElement("Version")]
    public readonly string version;

    public string DownloadButtonString { get; private set; }
    public enum State
    {
        NotInstalled,
        Installing,
        CanBeUpdated,
        Installed,
        Running
    }

    private State _currentState;

    public State CurrentState
    {
        get { return _currentState; }
        private set
        {
            _currentState = value;
            DownloadButtonString = GetNewDownloadButtonText(value);
            RaisePropertyChanged(nameof(DownloadButtonString));
        }
    }
    [BsonConstructor]
    public Game(string GameName, string URL, string version)
    {
        this.GameName = GameName;
        this.URL = URL;
        this.version = version;
        CurrentState = State.NotInstalled;
    }

    /// <summary>
    /// This constructor creates a Game object that references an installed game.
    /// </summary>
    /// <param name="gameName">Name of the game. Must be Unique</param>
    /// <param name="localFolder">Where the installed game is located.</param>
    /// <param name="state">State of the game, usually Ready.</param>
    public Game(string gameName, string localFolder, string version, State state = State.Installed)
    {
        GameName = gameName;
        this.localFolder = localFolder;
        CurrentState = state;
        this.version = version;
    }

    public override string ToString()
    {
        return $"This is {GameName}, located in {localFolder}, version is: {version}";
    }

    public void SetUpdateAvailable()
    {
        CurrentState = State.CanBeUpdated;
    }


    public void OnFinishedExecution()
    {
        this.CurrentState = State.Installed;
        Debug.WriteLine("I've FINISHED");
    }

    /// <summary>
    /// Get new text for the game button based on the current state of the game.
    /// </summary>
    /// <param name="newState">Current state</param>
    /// <returns></returns>
    private static string GetNewDownloadButtonText(State state)
    {
        return state switch
        {
            State.NotInstalled => "Download",
            State.Installing => "Downloading",
            State.CanBeUpdated => "Update",
            State.Installed => "Play",
            State.Running => "Playing",
            _ => "ERROR"
        };
    }

    public async void ButtonPress()
    {
        switch (CurrentState)
        {
            case State.NotInstalled:
                CurrentState = State.Installing;
                await NetworkModule.DownloadGame(this);
                CurrentState = State.Installed;
                break;
            case State.Installing:
                //Prevent from pressing the button
                break;
            case State.CanBeUpdated:
                //Update the game
                break;
            case State.Installed:
                GameLauncher.LaunchGame(this);
                CurrentState = State.Running;
                //Launch
                break;
            case State.Running:
                //Stop the execution
                break;
        }
    }

    private ICommand? _clickCommand;
    public ICommand ClickCommand => _clickCommand ??= new CommandHandler(() => ButtonPress(), () => CanExecute);
    public bool CanExecute
    {
        get
        {
            // check if execution is allowed, i.e., validate, check if a process is running, etc. 
            return CurrentState != State.Installing;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
