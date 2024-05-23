using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class UpdateGame
{
    public readonly BattleCity BattleCity;
    private readonly ConstructorController _constructorController;

    public UpdateGame(BattleCity battleCity)
    {
        BattleCity = battleCity;
        _constructorController = new ConstructorController(this, battleCity);
    }

    public void Updating(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        switch (BattleCity.State)
        {
            case StateOfGame.MainMenu:
                UpdateMainMenu(gameTime, keyboardState);
                break;
            case StateOfGame.Pause:
                UpdatePauseState(keyboardState);
                break;
            case StateOfGame.LoadLevel:
                LoadLevelState();
                break;
            case StateOfGame.Game:
                UpdateGameState(gameTime, keyboardState);
                break;
            case StateOfGame.DefeatLevel:
                UpdateDefeatLevelState(keyboardState);
                break;
            case StateOfGame.WinLevel:
                UpdateWinLevelState();
                break;
            case StateOfGame.Constructor:
                _constructorController.UpdateConstructor(keyboardState);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }


    private void UpdateMainMenu(GameTime gameTime, KeyboardState keyboardState)
    {
        BattleCity.MainMenuController.Update(gameTime);

        if (keyboardState.IsKeyDown(Keys.Escape)) BattleCity.Exit();
    }

    private void UpdatePauseState(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter)) BattleCity.State = StateOfGame.Game;
    }

    private void LoadLevelState()
    {
        BattleCity.LevelController.LoadLevel(BattleCity.FileNameDictionary[BattleCity.NumberOfLevel], 5);
    }

    private void UpdateGameState(GameTime gameTime, KeyboardState keyboardState)
    {
        UpdateObjects(gameTime);
        RemoveNotAliveObjects();

        if (keyboardState.IsKeyDown(Keys.P)) BattleCity.State = StateOfGame.Pause;
        if (keyboardState.IsKeyDown(Keys.R))
        {
            Restart();
            BattleCity.State = StateOfGame.MainMenu;
            BattleCity.Constructor.LoadStartMap();
        }
    }

    private void UpdateDefeatLevelState(KeyboardState keyboardState)
    {
        if (!keyboardState.IsKeyDown(Keys.P)) return;
        Restart();
        BattleCity.State = StateOfGame.MainMenu;
        BattleCity.Constructor.LoadStartMap();
    }

    private void UpdateWinLevelState()
    {
        BattleCity.EnemyTanks.Clear();
        BattleCity.PlayersTanks.Clear();
        BattleCity.BulletObjects.Clear();
        if (BattleCity.NumberOfLevel < 3)
        {
            BattleCity.NumberOfLevel += 1;
            BattleCity.State = StateOfGame.LoadLevel;
        }
        else
        {
            BattleCity.State = StateOfGame.MainMenu;
        }
    }

    private void Restart()
    {
        BattleCity.EnemyTanks.Clear();
        BattleCity.PlayersTanks.Clear();
        BattleCity.BulletObjects.Clear();
        BattleCity.BangModels.Clear();
        BattleCity.NumberOfLevel = 1;
    }

    private void RemoveNotAliveObjects()
    {
        BattleCity.BulletObjects.RemoveWhere(element => element.ShotModel.IsAlive == false);
        BattleCity.PlayersTanks.RemoveWhere(element => element.IsAlive == false);
        BattleCity.EnemyTanks.RemoveWhere(element => element.EnemyModel.IsAlive == false);
        BattleCity.BangModels.RemoveWhere(element => element.IsAlive == false);
        if (BattleCity.PlayersTanks.Count == 0 && BattleCity.State != StateOfGame.Constructor)
            BattleCity.State = StateOfGame.DefeatLevel;
        if (BattleCity.EnemyTanks.Count == 0 && BattleCity.EnemyInLevel == 0)
            BattleCity.State = StateOfGame.WinLevel;
    }

    private void UpdateObjects(GameTime gameTime)
    {
        foreach (var controller in BattleCity.BangModels.Select(bang => new BangController(bang)))
            controller.Update(gameTime);

        foreach (var playerController in BattleCity.PlayerControllers) playerController.Update(gameTime);

        var listCoordinate = BattleCity.PlayersTanks.Select(tanks => tanks.GetCoordinate()).ToList();

        BattleCity.LevelController.ReLoadTanks(3, gameTime);
        foreach (var enemyTank in BattleCity.EnemyTanks)
            enemyTank.Update(gameTime, BattleCity.SceneObjects, listCoordinate, BattleCity.CoordinateOfStaff);

        foreach (var bullet in BattleCity.BulletObjects)
            bullet.Update(gameTime);

        foreach (var scene in BattleCity.SceneObjects)
            scene.Update(gameTime);
    }
}