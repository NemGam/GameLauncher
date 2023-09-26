using System;
using System.Windows;
using System.Windows.Input;

namespace GameLauncher;

public class Game
{
    public string GameName { get;}
    readonly string folder;

    public enum State
    {
        NotInstalled,
        Installing,
        Ready,
        Running
    }

    public State CurrentState { get; private set; }

    public Game(string gameName, string folder, State state = State.NotInstalled)
	{
        this.GameName = gameName;
        this.folder = folder;
        CurrentState = state;
    }

    public override string ToString()
    {
        return $"This is {this.GameName}, located in {this.folder}";
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

    public void ButtonPress()
    {
        switch (CurrentState)
        {
            case State.NotInstalled:
                //Try to install
                break;
            case State.Installing:
                //Prevent from pressing the button
                break;
            case State.Ready:
                //Launch
                break;
            case State.Running:
                //Stop the execution
                break;
        }
        MessageBox.Show(ToString());
    }
}
