using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class ConstructorController
{
    private readonly UpdateGame _updateGame;
    private readonly BattleCity _battleCity;

    public ConstructorController(UpdateGame updateGame, BattleCity battleCity)
    {
        _updateGame = updateGame;
        _battleCity = battleCity;
    }

    private void Update()
    {
        _battleCity.SceneObjects = _battleCity.construtor.ConstructorScene;
        _battleCity._coordinateForPlayer = _battleCity.construtor.CoordinateForPlayer;
        _battleCity._coordinateForEnemy = _battleCity.construtor.CoordinateForEnemy;
        _battleCity.construtor.Copy = _battleCity.construtor.DeepCopyArray(_battleCity.construtor.ConstructorScene);
    }

    public void UpdateConstructor(KeyboardState keyboardState)
    {
        UpdateConstructorBlocks();
        UpdateConstructorMap();
        CheckConstructorKeyInputs(keyboardState);
        Update();
    }

    private void UpdateConstructorBlocks()
    {
        var currentMouseState = Mouse.GetState();
        if (currentMouseState.RightButton == ButtonState.Pressed)
        {
            var position = GetMousePositionInBlocks(currentMouseState.Position);
            _updateGame._battleCity.construtor.UpdateChosenBlock(position);
        }
    }

    private void UpdateConstructorMap()
    {
        var currentMouseState = Mouse.GetState();
        if (currentMouseState.LeftButton == ButtonState.Pressed)
        {
            var position = GetMousePositionInBlocks(currentMouseState.Position);
            _updateGame._battleCity.construtor.UpdateMap(position);
        }
    }

    private void CheckConstructorKeyInputs(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.K)) _updateGame._battleCity.LoadConstructor(1);
        if (keyboardState.IsKeyDown(Keys.R)) _updateGame._battleCity.State = StateOfGame.MainMenu;
    }

    private static Vector2 GetMousePositionInBlocks(Point mousePosition)
    {
        return new Vector2(mousePosition.X / 64, mousePosition.Y / 64);
    }
}