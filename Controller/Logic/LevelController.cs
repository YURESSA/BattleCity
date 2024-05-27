using System;
using System.Collections.Generic;
using System.Linq;
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
        _battleCity.PlayerViews = new List<PlayerView>();
        _battleCity.PlayerControllers = new List<PlayerController>();
        _battleCity.EnemyInLevel = enemyCount;

        SetupPlayer(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, _battleCity.CoordinateForPlayer[0]);

        _battleCity.State = StateOfGame.Game;
        MusicController.PlayStartMusic();
        MusicController.StartLevelMusic();
    }

    public void LoadLevel(string fileName, int enemyCount)
    {
        if (_battleCity.MainMenu.GameModeState == GameMode.Settings) _battleCity.State = StateOfGame.Settings;
        _battleCity.PlayerViews = new List<PlayerView>();
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

        if (_battleCity.MainMenu.GameModeState == GameMode.Constructor)
        {
            _battleCity.State = StateOfGame.Constructor;
        }
        else if (_battleCity.MainMenu.GameModeState == GameMode.Settings)
        {
            _battleCity.State = StateOfGame.Settings;
        }
        else
        {
            _battleCity.State = StateOfGame.Game;
            MusicController.PlayStartMusic();
            MusicController.StartLevelMusic();
        }
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
            _battleCity.CollisionDetected.HasCollision, _battleCity.BulletObjects, true, 2);

        _battleCity.PlayerControllers.Add(new PlayerController(playerTank, controlButton));
        _battleCity.PlayerViews.Add(new PlayerView(_battleCity.PlayerImage));
        _battleCity.PlayersTanks.Add(playerTank);
    }

    public void ReLoadTanks(int enemyInWave, GameTime gameTime)
    {
        _battleCity.ElapsedTime -= gameTime.ElapsedGameTime;

        if (_battleCity.EnemyTanks.Count >= enemyInWave || _battleCity.ElapsedTime >= TimeSpan.Zero ||
            _battleCity.EnemyInLevel <= 0) return;
        var playerCoordinates = _battleCity.PlayersTanks.Select(player => new Vector2()
            { X = player.GetCoordinate().X * 64, Y = player.GetCoordinate().Y * 64 });
        var nearestCoordinate = FindNearestCoordinate(playerCoordinates, _battleCity.CoordinateForEnemy);

        var enemyTank = EnemyController.GetEnemy(0.08f,
            nearestCoordinate,
            _battleCity.EnemyImage, _battleCity.CollisionDetected.HasCollision, _battleCity.BulletObjects, true,
            1);

        _battleCity.EnemyTanks.Add(enemyTank);

        if (_battleCity.EnemyTanks.Count == enemyInWave)
            _battleCity.ElapsedTime = TimeSpan.FromMilliseconds(10000);

        _battleCity.ElapsedTime = TimeSpan.FromMilliseconds(3000);
        _battleCity.EnemyInLevel -= 1;
    }

    private Vector2 FindNearestCoordinate(IEnumerable<Vector2> playerCoordinates, List<Vector2> enemyCoordinates)
    {
        var nearestCoordinate = Vector2.Zero;
        var shortestDistance = float.MaxValue;

        var coordinates = playerCoordinates.ToList();
        foreach (var enemyCoordinate in enemyCoordinates)
        foreach (var distance in coordinates.Select(playerCoordinate => Vector2.Distance(enemyCoordinate, playerCoordinate)).Where(distance => distance < shortestDistance))
        {
            shortestDistance = distance;
            nearestCoordinate = enemyCoordinate;
        }

        return nearestCoordinate;
    }
}