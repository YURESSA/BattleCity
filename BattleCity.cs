using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class BattleCity : Game
{
    private const int CellSize = 64;
    public readonly HashSet<Shot> BulletObjects = new();
    private readonly GraphicsDeviceManager _graphics;
    private TimeSpan _elapsedTime;
    public Texture2D EnemyImage;
    public int EnemyInLevel;
    public HashSet<Enemy> EnemyTanks;
    public HashSet<BangModel> BangModels = new();
    public Dictionary<int, string> FileNameDictionary;
    public Defeat GameDefeat;
    public MenuModel MainMenu;
    public Menu Menu;
    public Texture2D PlayerImage;
    public HashSet<PlayerModel> PlayersTanks;
    public Dictionary<TypeOfObject, Texture2D> SceneDictionary;
    public Dictionary<int, Texture2D> FrameDictionary;
    public SceneView[,] SceneObjects;
    public SpriteBatch SpriteBatch;
    public StateOfGame State = StateOfGame.MainMenu;
    public readonly ConstructorModel Constructor;
    public int NumberOfLevel;
    public List<PlayerController> PlayerControllers;
    private List<PlayerView> _playerViews;
    private readonly CollisionDetected _collisionDetected;
    private readonly UpdateGame _updateGame;
    private readonly DrawGame _drawGame;
    public List<Vector2> CoordinateForEnemy;
    public List<Vector2> CoordinateForPlayer;
    public MenuController MainMenuController;
    public Vector2 CoordinateOfStaff;

    public BattleCity()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        IsMouseVisible = true;
        _collisionDetected = new CollisionDetected(this);
        _updateGame = new UpdateGame(this);
        _drawGame = new DrawGame(this);
        Constructor = new ConstructorModel(this);
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
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        LoadGameTextures();
        LoadSceneTextures();
        LoadFrameTextures();
        InitializeGameObjects();

        FileNameDictionary = new Dictionary<int, string>
        {
            { 1, "levels/level1.txt" },
            { 2, "levels/level2.txt" },
            { 3, "levels/level3.txt" },
            { 0, "levels/custom.txt" }
        };
        Constructor.LoadStartMap();
    }

    private void LoadGameTextures()
    {
        MainMenu = new MenuModel(new Vector2(270, 495), this);
        MainMenuController = new MenuController(MainMenu);
        GameDefeat = new Defeat(Content.Load<Texture2D>("gameOver"));
        Shot._texture = Content.Load<Texture2D>("bullet");
        DrawGame.Vision = Content.Load<Texture2D>("tank");
        DrawGame.LevelIcon = Content.Load<Texture2D>("flag");
        DrawGame.textBlock = Content.Load<SpriteFont>("mytext1");
        DrawGame.FirstPlayerHp = Content.Load<Texture2D>("hp1Tank");
        DrawGame.SecondPlayerHp = Content.Load<Texture2D>("hp2Tank");
        PlayerImage = Content.Load<Texture2D>("player");
        EnemyImage = Content.Load<Texture2D>("tank1");
        Menu = new Menu(Content.Load<Texture2D>("MainMenu"), Content.Load<Texture2D>("cursor"));
    }

    private void LoadSceneTextures()
    {
        SceneDictionary = new Dictionary<TypeOfObject, Texture2D>
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
    
    private void LoadFrameTextures()
    {
        
        FrameDictionary = new Dictionary<int, Texture2D>
        {
            { 0, Content.Load<Texture2D>("bang1") },
            { 1, Content.Load<Texture2D>("bang2") },
            { 2, Content.Load<Texture2D>("bang3") }
        };
        BangView.TextureOfFrame = FrameDictionary;
    }

    private void InitializeGameObjects()
    {
        PlayersTanks = new HashSet<PlayerModel>();
        EnemyTanks = new HashSet<Enemy>();
    }

    public void LoadConstructor(int enemyCount)
    {
        _playerViews = new List<PlayerView>();
        PlayerControllers = new List<PlayerController>();
        EnemyInLevel = enemyCount;

        SetupPlayer(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, CoordinateForPlayer[0]);

        State = StateOfGame.Game;
    }

    public void LoadLevel(string fileName, int enemyCount)
    {
        _playerViews = new List<PlayerView>();
        PlayerControllers = new List<PlayerController>();
        EnemyInLevel = enemyCount;

        SceneObjects = ReaderOfMap.MapReader(SceneDictionary, CellSize, fileName);
        CoordinateForPlayer = ReaderOfMap.GetPlayerCoordinate();
        CoordinateForEnemy = ReaderOfMap.GetEnemyCoordinate();
        CoordinateOfStaff = ReaderOfMap.GetCoordinateOfStaff();

        if (MainMenu.GameModeState is GameMode.OnePlayer or GameMode.TwoPlayer)
            SetupPlayer(Keys.W, Keys.S, Keys.A, Keys.D, Keys.Space, CoordinateForPlayer[0]);

        if (MainMenu.GameModeState == GameMode.TwoPlayer)
            SetupPlayer(Keys.Up, Keys.Down, Keys.Left, Keys.Right, Keys.L, CoordinateForPlayer[1]);

        State = MainMenu.GameModeState == GameMode.Constructor ? StateOfGame.Constructor : StateOfGame.Game;
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

        var playerTank = new PlayerModel(0.1f, playerPosition, PlayerImage, _collisionDetected.HasCollision,
            BulletObjects, true, 2);

        PlayerControllers.Add(new PlayerController(playerTank, controlButton));
        _playerViews.Add(new PlayerView(PlayerImage));
        PlayersTanks.Add(playerTank);
    }

    public void ReLoadTanks(int enemyInWave, GameTime gameTime)
    {
        _elapsedTime -= gameTime.ElapsedGameTime;
        var random = new Random();

        if (EnemyTanks.Count < enemyInWave && _elapsedTime < TimeSpan.Zero && EnemyInLevel > 0)
        {
            var enemyTank = new Enemy(0.08f, CoordinateForEnemy[random.Next(CoordinateForEnemy.Count)],
                EnemyImage, CellSize, _collisionDetected.HasCollision, BulletObjects, true, 1);

            EnemyTanks.Add(enemyTank);

            if (EnemyTanks.Count == enemyInWave)
                _elapsedTime = TimeSpan.FromMilliseconds(10000);

            _elapsedTime = TimeSpan.FromMilliseconds(3000);
            EnemyInLevel -= 1;
        }
    }

    protected override void Update(GameTime gameTime)
    {
        _updateGame.Updating(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        SpriteBatch.Begin();
        _drawGame.Drawing(MainMenu);
        SpriteBatch.End();
        base.Draw(gameTime);
    }
}