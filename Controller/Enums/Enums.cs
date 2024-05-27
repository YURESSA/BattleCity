namespace BattleCity;

public enum StateOfGame
{
    MainMenu,
    Game,
    WinLevel,
    DefeatLevel,
    LoadLevel,
    Constructor,
    Pause,
    Settings
}

public enum GameMode
{
    OnePlayer,
    TwoPlayer,
    Constructor,
    Settings
}

public enum State
{
    Empty,
    Wall
}

public enum TypeOfObject
{
    None,
    Bricks,
    Concrete,
    Leaves,
    Water,
    Staff,
    Wall,
    Player,
    Enemy
}