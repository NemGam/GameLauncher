using System;
namespace GameLauncher;

public readonly struct GameInfo
{
    readonly string gameName;
    readonly string folder;
    public GameInfo(string gameName, string folder)
	{
        this.gameName = gameName;
        this.folder = folder;
    }
}
