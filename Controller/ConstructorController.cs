using System;
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
        _battleCity.SceneObjects = _battleCity.Constructor.ConstructorScene;
        _battleCity.CoordinateForPlayer = _battleCity.Constructor.CoordinateForPlayer;
        _battleCity.CoordinateForEnemy = _battleCity.Constructor.CoordinateForEnemy;
        _battleCity.Constructor.Copy = _battleCity.Constructor.DeepCopyArray(_battleCity.Constructor.ConstructorScene);
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
        if (currentMouseState.RightButton != ButtonState.Pressed) return;
        var position = GetMousePositionInBlocks(currentMouseState.Position);
        _updateGame.BattleCity.Constructor.UpdateChosenBlock(position);
    }

    private void UpdateConstructorMap()
    {
        var currentMouseState = Mouse.GetState();
        if (currentMouseState.LeftButton != ButtonState.Pressed) return;
        var position = GetMousePositionInBlocks(currentMouseState.Position);
        _updateGame.BattleCity.Constructor.UpdateMap(position);
    }

    private void CheckConstructorKeyInputs(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.K))
            _updateGame.BattleCity.LevelController.LoadConstructor(1);

        if (keyboardState.IsKeyDown(Keys.R)) _updateGame.BattleCity.State = StateOfGame.MainMenu;
    }

    private static Vector2 GetMousePositionInBlocks(Point mousePosition)
    {
        return new Vector2(mousePosition.X / 64, mousePosition.Y / 64);
    }
}