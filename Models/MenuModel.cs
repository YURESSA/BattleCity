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
    public int _displacement;
    private readonly BattleCity _battleCity;

    public Dictionary<int, GameMode> GameModeStates = new()
    {
        {0, GameMode.OnePlayer},
        {1, GameMode.TwoPlayer},
        {2, GameMode.Constructor}
    };


    public MenuModel(Vector2 position, BattleCity battleCity)
    {
        _startPosition = position;
        Position = position;
        _battleCity = battleCity;
    }

    public void Update(GameTime gameTime)
    {
        GameModeState = GameModeStates[_displacement];
    }

    public void CursorMoveUp()
    {
        _displacement = (_displacement + 2) % 3;
        Position = _startPosition + new Vector2(0, 90 * _displacement);
    }

    public void CursorMoveDown()
    {
        _displacement = (_displacement + 1) % 3;
        Position = _startPosition + new Vector2(0, 90 * _displacement);
    }

    public void StartGame()
    {
        _battleCity.NumberOfLevel = 1;
        _battleCity.State = StateOfGame.LoadLevel;
    }
}
