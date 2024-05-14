using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public enum StateOfGame
{
    MainMenu,
    Game,
    Level1,
    Level2,
    Level3,
    WinLevel,
    DefeatLevel,
    Final,
    Pause
}

public class BattleCity : Game
{
    private const int CellSize = 64;
    private readonly HashSet<Shot> _bulletObjects = new();
    private readonly GraphicsDeviceManager _graphics;
    private HashSet<Enemy> _enemyTanks;
    private Defeat _gameDefeat;
    private Menu _mainMenu;
    private HashSet<PlayerModel> _playersTanks;
    private Scene[,] _sceneObjects;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;
    private StateOfGame _currentLevel = StateOfGame.Level1;
    private TimeSpan _elapsedTime;
    private Texture2D _enemyImage;
    private int _enemyInLevel;

    private Texture2D _playerImage;
    private Dictionary<TypeOfObject, Texture2D> _sceneDictionary;
    private PlayerView PlayerView;
    private PlayerController PlayerController;
    public BattleCity()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = 960;
        _graphics.PreferredBackBufferWidth = 1100;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu = new Menu(Content.Load<Texture2D>("MainMenu"));
        _gameDefeat = new Defeat(Content.Load<Texture2D>("gameOver"));
        _playersTanks = new HashSet<PlayerModel>();
        _enemyTanks = new HashSet<Enemy>();
        Shot._texture = Content.Load<Texture2D>("bullet");
        _playerImage = Content.Load<Texture2D>("player");
        _enemyImage = Content.Load<Texture2D>("tank1");


        _sceneDictionary = new Dictionary<TypeOfObject, Texture2D>
        {
            { TypeOfObject.None, Content.Load<Texture2D>("none") },
            { TypeOfObject.Bricks, Content.Load<Texture2D>("bricks") },
            { TypeOfObject.Concrete, Content.Load<Texture2D>("concrete") },
            { TypeOfObject.Leaves, Content.Load<Texture2D>("leaves") },
            { TypeOfObject.Water, Content.Load<Texture2D>("water") },
            { TypeOfObject.Staff, Content.Load<Texture2D>("staff") },
            { TypeOfObject.Wall, Content.Load<Texture2D>("wall") }
        };
    }

    private void LoadLevel(string fileName, int playersCount, int enemyCount)
    {
        _enemyInLevel = enemyCount;
        var playersTank = new PlayerModel(0.1f, new Vector2(320, 832), _playerImage, CellSize,
            HasCollision, _bulletObjects, true, 2);
        ControlButton controlButton = new ControlButton
        {
            Up = Keys.W,
            Down = Keys.S,
            Left = Keys.A,
            Right = Keys.D,
            Shoot = Keys.Space
        };
        PlayerController = new PlayerController(playersTank, controlButton);
        PlayerView = new PlayerView(_playerImage);
        _playersTanks.Add(playersTank);
        _sceneObjects = ReaderOfMap.Reader(_sceneDictionary, CellSize, fileName);
    }

    private void ReLoadTanks(int enemyInWave, GameTime gameTime)
    {
        _elapsedTime -= gameTime.ElapsedGameTime;
        var coordinateToSpawn = new List<Vector2> { new(68, 68), new(836, 68), new(452, 68)};
        var random = new Random();
        if (_enemyTanks.Count < enemyInWave && _elapsedTime < TimeSpan.Zero && _enemyInLevel > 0)
        {
            var enemyTank = new Enemy(0.08f, coordinateToSpawn[random.Next(coordinateToSpawn.Count)],
                _enemyImage, CellSize, HasCollision, _bulletObjects, true, 1);
            _enemyTanks.Add(enemyTank);
            if (_enemyTanks.Count == enemyInWave)
                _elapsedTime = TimeSpan.FromMilliseconds(10000);
            _elapsedTime = TimeSpan.FromMilliseconds(3000);
            _enemyInLevel -= 1;
        }
    }


    protected override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case StateOfGame.MainMenu:
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    _state = _currentLevel;

                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                break;
            case StateOfGame.Pause:
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    _state = StateOfGame.Game;
                break;
            case StateOfGame.Level1:
                LoadLevel("input.txt", 1, 10);
                _state = StateOfGame.Game;
                break;
            case StateOfGame.Game:

                UpdateObjects(gameTime);
                RemoveNotAliveObjects();

                if (Keyboard.GetState().IsKeyDown(Keys.P))
                    _state = StateOfGame.Pause;
                break;
            case StateOfGame.DefeatLevel:
                if (Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    _enemyTanks.Clear();
                    _playersTanks.Clear();
                    _bulletObjects.Clear();
                    _state = StateOfGame.MainMenu;
                }

                break;
        }

        base.Update(gameTime);
    }

    private void RemoveNotAliveObjects()
    {
        _bulletObjects.RemoveWhere(element => element.ShotModel.ShotHasCollisions);
        _bulletObjects.RemoveWhere(element => element.ShotModel.IsAlive == false);
        _playersTanks.RemoveWhere(element => element.IsAlive == false);
        _enemyTanks.RemoveWhere(element => element._enemyModel.IsAlive == false);
        if (_playersTanks.Count == 0)
            _state = StateOfGame.DefeatLevel;
    }

    private void UpdateObjects(GameTime gameTime)
    {
        var userCoordinate = Vector2.One;
        PlayerController.Update(gameTime);
        foreach (var tanks in _playersTanks)
        {
            userCoordinate = tanks.GetCoordinate();
        }

        ReLoadTanks(3, gameTime);
        foreach (var enemyTank in _enemyTanks)
            enemyTank.Update(gameTime, _sceneObjects, userCoordinate);

        foreach (var bullet in _bulletObjects)
            bullet.Update(gameTime);

        foreach (var scene in _sceneObjects)
            scene.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        switch (_state)
        {
            case StateOfGame.MainMenu:
                GraphicsDevice.Clear(Color.Black);
                _mainMenu.Draw(_spriteBatch);
                break;
            
            case StateOfGame.Pause:
                GraphicsDevice.Clear(Color.Black);
                _mainMenu.Draw(_spriteBatch);
                break;
            case StateOfGame.Game:
                GraphicsDevice.Clear(Color.Gray);
                foreach (var scenic in _sceneObjects)
                    scenic.Draw(_spriteBatch);
                foreach (var tank in _playersTanks)
                    PlayerView.Draw(_spriteBatch, tank);
                foreach (var tank in _enemyTanks)
                    tank.Draw(_spriteBatch);
                foreach (var bulletObject in _bulletObjects)
                    bulletObject.Draw(_spriteBatch);
                break;
            case StateOfGame.DefeatLevel:
                _gameDefeat.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool HasCollision(MovedObject obj)
    {
        foreach (var scene in _sceneObjects)
            if (scene.SceneModel.Intersect(obj) && scene.SceneModel.Type != TypeOfObject.None)
            {
                if ((scene.SceneModel.Type == TypeOfObject.Bricks ||
                     scene.SceneModel.Type == TypeOfObject.Staff) && obj is ShotModel)
                {
                    if (scene.SceneModel.Type == TypeOfObject.Staff)
                        _state = StateOfGame.DefeatLevel;
                    scene.SceneModel.IsAlive = false;
                }

                return true;
            }


        foreach (var shot in _bulletObjects)
            if (obj.Intersect(shot.ShotModel) && obj is ShotModel && obj != shot.ShotModel)
            {
                shot.ShotModel.IsAlive = false;
                return true;
            }


        foreach (var playersTank in _playersTanks)
        {
            if (!obj.Intersect(playersTank) ||
                obj == playersTank || obj.Parent == playersTank) continue;
            playersTank.Kill();
            return true;
        }

        foreach (var enemyTank in _enemyTanks)
        {
            if (!obj.Intersect(enemyTank._enemyModel) ||
                obj == enemyTank._enemyModel || obj.Parent == enemyTank._enemyModel || obj is EnemyModel) continue;
            enemyTank._enemyModel.Kill();
            return true;
        }
        

        return false;
    }
}