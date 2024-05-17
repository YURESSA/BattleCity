namespace BattleCity;

public enum StateOfGame
{
    MainMenu,
    Game,
    WinLevel,
    DefeatLevel,
    LoadLevel,
    Constructor,
    Pause
}
public enum GameMode
{
    OnePlayer,
    TwoPlayer,
    Constructor
}
public enum State
{
    Empty,
    Wall,
    Visited
}