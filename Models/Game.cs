using GameLauncher.ViewModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Windows;
using System.Windows.Input;

namespace GameLauncher;
[BsonIgnoreExtraElements]
public class Game
{

    [BsonElement("GameName")]
    public string GameName { get;}
    private bool isInstalled;
    readonly string? localFolder;

    [BsonElement("URL")]
    readonly string? URL;

    [BsonElement("Version")]
    public readonly string version;

    public enum State
    {
        NotInstalled,
        Installing,
        CanBeUpdated,
        Installed,
        Running
    }

    public State CurrentState { get; private set; }

    [BsonConstructor]
    public Game(string GameName, string URL, string version)
    {
        this.GameName = GameName;
        this.URL = URL;
        this.version = version;
        this.isInstalled = false;
    }

    /// <summary>
    /// This constructor creates a Game object that references an installed game.
    /// </summary>
    /// <param name="gameName">Name of the game. Must be Unique</param>
    /// <param name="localFolder">Where the installed game is located.</param>
    /// <param name="state">State of the game, usually Ready.</param>
    public Game(string gameName, string localFolder, string version, State state = State.Installed)
	{
        this.GameName = gameName;
        this.localFolder = localFolder;
        this.CurrentState = state;
        this.isInstalled = true;
        this.version = version;
    }

    public override string ToString()
    {
        return $"This is {this.GameName}, located in {this.localFolder}, version is: {this.version}";
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

    public void SetUpdateAvailable()
    {
        CurrentState = State.CanBeUpdated;
    }

    public void ButtonPress()
    {
        switch (CurrentState)
        {
            case State.NotInstalled:
                NetworkModule.DownloadGame(this);
                CurrentState = State.Installing;
                break;
            case State.Installing:
                //Prevent from pressing the button
                break;
            case State.CanBeUpdated: 
                //Update the game
                break;
            case State.Installed:
                //Launch
                break;
            case State.Running:
                //Stop the execution
                break;
        }
        MessageBox.Show(ToString());
    }
}
