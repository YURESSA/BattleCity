using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class LevelController
{
    private readonly BattleCity _battleCity;

    public LevelController(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }

    public void LoadConstructor(int enemyCount)
    {
        _battleCity._playerViews = new List<PlayerView>();
        _battleCity.PlayerControllers = new List<PlayerController>();
        _battleCity.EnemyInLevel = enemyCount;

        SetupPlayer(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, _battleCity.CoordinateForPlayer[0]);

        _battleCity.State = StateOfGame.Game;
    }

    public void LoadLevel(string fileName, int enemyCount)
    {
        _battleCity._playerViews = new List<PlayerView>();
        _battleCity.PlayerControllers = new List<PlayerController>();
        _battleCity.EnemyInLevel = enemyCount;

        _battleCity.SceneObjects = ReaderOfMap.MapReader(_battleCity.SceneDictionary, BattleCity.CellSize, fileName);
        _battleCity.CoordinateForPlayer = ReaderOfMap.GetPlayerCoordinate();
        _battleCity.CoordinateForEnemy = ReaderOfMap.GetEnemyCoordinate();
        _battleCity.CoordinateOfStaff = ReaderOfMap.GetCoordinateOfStaff();

        if (_battleCity.MainMenu.GameModeState is GameMode.OnePlayer or GameMode.TwoPlayer)
            SetupPlayer(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, _battleCity.CoordinateForPlayer[0]);

        if (_battleCity.MainMenu.GameModeState == GameMode.TwoPlayer)
            SetupPlayer(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.L, _battleCity.CoordinateForPlayer[1]);

        _battleCity.State = _battleCity.MainMenu.GameModeState == GameMode.Constructor ? StateOfGame.Constructor : StateOfGame.Game;
    }

    private void SetupPlayer(Keys up, Keys down, Keys left, Keys right, Keys shoot, Vector2 playerPosition)
    {
        var controlButton = new ControlButton
        {
            Up = up,
            Down = down,
            Left = left,
            Right = right,
            Shoot = shoot
        };

        var playerTank = new PlayerModel(0.1f, playerPosition, _battleCity.PlayerImage, 
            _battleCity._collisionDetected.HasCollision, _battleCity.BulletObjects, true, 2);

        _battleCity.PlayerControllers.Add(new PlayerController(playerTank, controlButton));
        _battleCity._playerViews.Add(new PlayerView(_battleCity.PlayerImage));
        _battleCity.PlayersTanks.Add(playerTank);
    }

    public void ReLoadTanks(int enemyInWave, GameTime gameTime)
    {
        _battleCity._elapsedTime -= gameTime.ElapsedGameTime;
        var random = new Random();

        if (_battleCity.EnemyTanks.Count < enemyInWave && _battleCity._elapsedTime < TimeSpan.Zero && _battleCity.EnemyInLevel > 0)
        {
            var enemyTank = EnemyController.GetEnemy(0.08f, _battleCity.CoordinateForEnemy[random.Next(_battleCity.CoordinateForEnemy.Count)], _battleCity.EnemyImage, _battleCity._collisionDetected.HasCollision, _battleCity.BulletObjects, true, 1);

            _battleCity.EnemyTanks.Add(enemyTank);

            if (_battleCity.EnemyTanks.Count == enemyInWave) _battleCity._elapsedTime = TimeSpan.FromMilliseconds(10000);

            _battleCity._elapsedTime = TimeSpan.FromMilliseconds(3000);
            _battleCity.EnemyInLevel -= 1;
        }
    }
}