using System.Collections.Generic;
using BattleCity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class MenuModel
{
    private readonly Vector2 _startPosition;
    public Vector2 Position;
    public GameMode GameModeState;
    private int _displacement;
    private readonly BattleCity _battleCity;

    private readonly Dictionary<int, GameMode> _gameModeStates = new()
    {
        { 0, GameMode.OnePlayer },
        { 1, GameMode.TwoPlayer },
        { 2, GameMode.Constructor },
        { 3, GameMode.Settings }
    };


    public MenuModel(Vector2 position, BattleCity battleCity)
    {
        _startPosition = position;
        Position = position;
        _battleCity = battleCity;
    }

    public void Update(GameTime gameTime)
    {
        GameModeState = _gameModeStates[_displacement];
    }

    public void CursorMoveUp()
    {
        _displacement = (_displacement + 3) % 4;
        Position = _startPosition + new Vector2(0, 90 * _displacement);
    }

    public void CursorMoveDown()
    {
        _displacement = (_displacement + 1) % 4;
        Position = _startPosition + new Vector2(0, 90 * _displacement);
    }

    public void StartGame()
    {
        _battleCity.NumberOfLevel = 1;
        _battleCity.State = StateOfGame.LoadLevel;
    }
}