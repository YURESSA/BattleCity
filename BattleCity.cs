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
    Constructor,
    LoadLevel,
    Final,
    Pause
}

public class BattleCity : Game
{
    private const int CellSize = 64;
    private readonly HashSet<Shot> _bulletObjects = new();
    private readonly GraphicsDeviceManager _graphics;
    private StateOfGame _currentLevel = StateOfGame.Level1;
    private TimeSpan _elapsedTime;
    private Texture2D _enemyImage;
    private int _enemyInLevel;
    private HashSet<Enemy> _enemyTanks;
    private Dictionary<int, string> _fileNameDictionary;
    private Defeat _gameDefeat;
    private Menu _mainMenu;
    private Texture2D _playerImage;
    private HashSet<PlayerModel> _playersTanks;
    private Dictionary<TypeOfObject, Texture2D> _sceneDictionary;
    private Scene[,] _sceneObjects;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;
    private Dictionary<int, StateOfGame> _stateDictionary;
    private int numberOfLevel;
    private PlayerController PlayerController;
    private PlayerView PlayerView;

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
        _fileNameDictionary = new Dictionary<int, string>
        {
            { 1, "levels/level1.txt" },
            { 2, "levels/level2.txt" },
            { 3, "levels/level3.txt" },
            { 0, "levels/custom.txt" }
        };
        _stateDictionary = new Dictionary<int, StateOfGame>
        {
            { 1, StateOfGame.Level1 },
            { 2, StateOfGame.Level2 },
            { 3, StateOfGame.Level3 }
        };
    }

    private void LoadLevel(string fileName, int playersCount, int enemyCount)
    {
        _enemyInLevel = enemyCount;
        var playersTank = new PlayerModel(0.1f, new Vector2(320, 832), _playerImage, CellSize,
            HasCollision, _bulletObjects, true, 2);
        var controlButton = new ControlButton
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
        var coordinateToSpawn = new List<Vector2> { new(68, 68), new(836, 68), new(452, 68) };
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
        var keyboardState = Keyboard.GetState();

        switch (_state)
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

        base.Update(gameTime);
    }

    private void UpdateMainMenu(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            numberOfLevel = 1;
            _state = StateOfGame.LoadLevel;
        }

        if (keyboardState.IsKeyDown(Keys.Escape))
            Exit();
    }

    private void UpdatePauseState(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Enter))
            _state = StateOfGame.Game;
    }

    private void LoadLevelState()
    {
        LoadLevel(_fileNameDictionary[numberOfLevel], 1, 3);
        _state = StateOfGame.Game;
    }

    private void UpdateGameState(GameTime gameTime, KeyboardState keyboardState)
    {
        UpdateObjects(gameTime);
        RemoveNotAliveObjects();

        if (keyboardState.IsKeyDown(Keys.P))
            _state = StateOfGame.Pause;
        if (keyboardState.IsKeyDown(Keys.R))
        {
            Restart();
            _state = StateOfGame.MainMenu;
        }
    }

    private void UpdateDefeatLevelState(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.P))
        {
            Restart();
            _state = StateOfGame.MainMenu;
        }
    }

    private void UpdateWinLevelState()
    {
        _enemyTanks.Clear();
        _playersTanks.Clear();
        _bulletObjects.Clear();
        if (numberOfLevel < 3)
        {
            numberOfLevel += 1;
            _state = StateOfGame.LoadLevel;
        }
        else
        {
            _state = StateOfGame.MainMenu;
        }
    }


    private void Restart()
    {
        _enemyTanks.Clear();
        _playersTanks.Clear();
        _bulletObjects.Clear();
        numberOfLevel = 1;
    }

    private void RemoveNotAliveObjects()
    {
        _bulletObjects.RemoveWhere(element => element.ShotModel.IsAlive == false);
        _playersTanks.RemoveWhere(element => element.IsAlive == false);
        _enemyTanks.RemoveWhere(element => element._enemyModel.IsAlive == false);
        if (_playersTanks.Count == 0)
            _state = StateOfGame.DefeatLevel;
        if (_enemyTanks.Count == 0 && _enemyInLevel == 0) _state = StateOfGame.WinLevel;
    }

    private void UpdateObjects(GameTime gameTime)
    {
        var userCoordinate = Vector2.One;
        PlayerController.Update(gameTime);
        foreach (var tanks in _playersTanks) userCoordinate = tanks.GetCoordinate();

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
                break;
            case StateOfGame.Game:
                GraphicsDevice.Clear(Color.Black);
                foreach (var tank in _playersTanks)
                    PlayerView.Draw(_spriteBatch, tank);
                foreach (var tank in _enemyTanks)
                    tank.Draw(_spriteBatch);
                foreach (var scenic in _sceneObjects)
                    if (scenic.SceneModel.Type is TypeOfObject.Water)
                        scenic.Draw(_spriteBatch);
                foreach (var bulletObject in _bulletObjects)
                    bulletObject.Draw(_spriteBatch);
                foreach (var scenic in _sceneObjects)
                    if (scenic.SceneModel.Type is not TypeOfObject.Water)
                        scenic.Draw(_spriteBatch);
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
        if (CheckSceneObjectCollisions(obj)) return true;
        if (CheckBulletCollisions(obj)) return true;

        return false;
    }

    private bool CheckSceneObjectCollisions(MovedObject obj)
    {
        foreach (var scene in _sceneObjects)
            if (scene.SceneModel.Intersect(obj) && scene.SceneModel.Type != TypeOfObject.None
                                                && scene.SceneModel.Type != TypeOfObject.Leaves)
            {
                HandleSceneCollision(scene.SceneModel, obj);
                return true;
            }

        return false;
    }

    private void HandleSceneCollision(SceneObjectsModel sceneModel, MovedObject obj)
    {
        if ((sceneModel.Type == TypeOfObject.Bricks || sceneModel.Type == TypeOfObject.Staff) && obj is ShotModel)
        {
            if (sceneModel.Type == TypeOfObject.Staff)
                _state = StateOfGame.DefeatLevel;

            sceneModel.IsAlive = false;
            obj.Kill();
        }
        else if (sceneModel.Type != TypeOfObject.Bricks && sceneModel.Type != TypeOfObject.Water && obj is ShotModel)
        {
            obj.Kill();
        }
    }

    private bool CheckBulletCollisions(MovedObject obj)
    {
        foreach (var bullet in _bulletObjects)
        {
            if (CheckBulletCollisionWithEnemy(obj, bullet)) break;
            if (CheckBulletCollisionWithPlayer(obj, bullet)) break;
            if (CheckBulletCollisionWithShot(obj, bullet)) break;
        }

        return false;
    }

    private bool CheckBulletCollisionWithEnemy(MovedObject obj, Shot bullet)
    {
        if (obj is EnemyModel && bullet.ShotModel.Parent is PlayerModel && obj.Intersect(bullet.ShotModel))
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }

    private bool CheckBulletCollisionWithPlayer(MovedObject obj, Shot bullet)
    {
        if (obj is PlayerModel && bullet.ShotModel.Parent is EnemyModel && obj.Intersect(bullet.ShotModel))
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }

    private bool CheckBulletCollisionWithShot(MovedObject obj, Shot bullet)
    {
        if (obj is ShotModel && obj.Intersect(bullet.ShotModel) && obj != bullet.ShotModel)
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }
}