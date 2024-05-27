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

        var playerData = _battleCity.GameData.Player;
        var playerTank = new PlayerModel(playerData.Speed, playerPosition, _battleCity.TanksImage[playerData.Image],
            _battleCity.CollisionDetected.HasCollision, _battleCity.BulletObjects, true, playerData.Hp, 
            playerData.BulletSpeed);

        _battleCity.PlayerControllers.Add(new PlayerController(playerTank, controlButton));
        _battleCity.PlayerViews.Add(new PlayerView(_battleCity.TanksImage[playerData.Image]));
        _battleCity.PlayersTanks.Add(playerTank);
    }

    public void ReLoadTanks(int enemyInWave, GameTime gameTime)
    {
        var enemyLevel = _battleCity.NumberOfLevel;
        _battleCity.ElapsedTime -= gameTime.ElapsedGameTime;

        if (_battleCity.EnemyTanks.Count >= enemyInWave || _battleCity.ElapsedTime >= TimeSpan.Zero ||
            _battleCity.EnemyInLevel <= 0) return;

        var playerCoordinates = _battleCity.PlayersTanks.Select(player => new Vector2()
            { X = player.GetCoordinate().X * 64, Y = player.GetCoordinate().Y * 64 });

        var random = new Random();
        var spawnCoordinate = random.NextDouble() < 0.6 ? 
            FindNearestCoordinate(playerCoordinates, _battleCity.CoordinateForEnemy) :
            _battleCity.CoordinateForEnemy[random.Next(_battleCity.CoordinateForEnemy.Count)];

        var enemyData = _battleCity.GameData.EnemyLevels.FirstOrDefault(e => e.Level == enemyLevel);

        if (enemyData != null)
        {
            var enemyTank = EnemyController.GetEnemy(enemyData.Speed, spawnCoordinate,
                _battleCity.TanksImage[enemyData.Image], _battleCity.CollisionDetected.HasCollision,
                _battleCity.BulletObjects, true, enemyData.Hp, enemyData.BulletSpeed);

            _battleCity.EnemyTanks.Add(enemyTank);
        }

    
        if (_battleCity.EnemyTanks.Count == enemyInWave)
            _battleCity.ElapsedTime = TimeSpan.FromMilliseconds(enemyData!.WaveDelay);

        _battleCity.ElapsedTime = TimeSpan.FromMilliseconds(enemyData!.SpawnDelay);
        _battleCity.EnemyInLevel -= 1;
    }

    private static Vector2 FindNearestCoordinate(IEnumerable<Vector2> playerCoordinates, List<Vector2> enemyCoordinates)
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