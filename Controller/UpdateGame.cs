using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class UpdateGame
{
    private readonly BattleCity _battleCity;

    public UpdateGame(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }

    public void Updating(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        switch (_battleCity.State)
        {
            case StateOfGame.MainMenu:
                UpdateMainMenu(keyboardState);
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
        }
    }

    private void UpdateMainMenu(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            _battleCity.NumberOfLevel = 1;
            _battleCity.State = StateOfGame.LoadLevel;
        }

        if (keyboardState.IsKeyDown(Keys.Escape)) _battleCity.Exit();
    }

    private void UpdatePauseState(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter)) _battleCity.State = StateOfGame.Game;
    }

    private void LoadLevelState()
    {
        _battleCity.LoadLevel(_battleCity.FileNameDictionary[_battleCity.NumberOfLevel], 1, 3);
        _battleCity.State = StateOfGame.Game;
    }

    private void UpdateGameState(GameTime gameTime, KeyboardState keyboardState)
    {
        UpdateObjects(gameTime);
        RemoveNotAliveObjects();

        if (keyboardState.IsKeyDown(Keys.P)) _battleCity.State = StateOfGame.Pause;
        if (keyboardState.IsKeyDown(Keys.R))
        {
            Restart();
            _battleCity.State = StateOfGame.MainMenu;
        }
    }

    private void UpdateDefeatLevelState(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.P))
        {
            Restart();
            _battleCity.State = StateOfGame.MainMenu;
        }
    }

    private void UpdateWinLevelState()
    {
        _battleCity.EnemyTanks.Clear();
        _battleCity.PlayersTanks.Clear();
        _battleCity.BulletObjects.Clear();
        if (_battleCity.NumberOfLevel < 3)
        {
            _battleCity.NumberOfLevel += 1;
            _battleCity.State = StateOfGame.LoadLevel;
        }
        else
        {
            _battleCity.State = StateOfGame.MainMenu;
        }
    }

    private void Restart()
    {
        _battleCity.EnemyTanks.Clear();
        _battleCity.PlayersTanks.Clear();
        _battleCity.BulletObjects.Clear();
        _battleCity.NumberOfLevel = 1;
    }

    private void RemoveNotAliveObjects()
    {
        _battleCity.BulletObjects.RemoveWhere(element => element.ShotModel.IsAlive == false);
        _battleCity.PlayersTanks.RemoveWhere(element => element.IsAlive == false);
        _battleCity.EnemyTanks.RemoveWhere(element => element.EnemyModel.IsAlive == false);
        if (_battleCity.PlayersTanks.Count == 0)
            _battleCity.State = StateOfGame.DefeatLevel;
        if (_battleCity.EnemyTanks.Count == 0 && _battleCity.EnemyInLevel == 0)
            _battleCity.State = StateOfGame.WinLevel;
    }

    private void UpdateObjects(GameTime gameTime)
    {
        var userCoordinate = Vector2.One;
        _battleCity.PlayerController.Update(gameTime);
        foreach (var tanks in _battleCity.PlayersTanks) userCoordinate = tanks.GetCoordinate();

        _battleCity.ReLoadTanks(3, gameTime);
        foreach (var enemyTank in _battleCity.EnemyTanks)
            enemyTank.Update(gameTime, _battleCity.SceneObjects, userCoordinate);

        foreach (var bullet in _battleCity.BulletObjects)
            bullet.Update(gameTime);

        foreach (var scene in _battleCity.SceneObjects)
            scene.Update(gameTime);
    }
}