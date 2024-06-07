using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;


namespace BattleCity;

public class BattleCity : Game
{
    public readonly HashSet<Shot> BulletObjects = new();
    private readonly GraphicsDeviceManager _graphics;
    public TimeSpan ElapsedTime;
    public int EnemyInLevel;
    public HashSet<EnemyController> EnemyTanks;
    public readonly HashSet<BangModel> BangModels = new();
    public Defeat GameDefeat;
    public MenuModel MainMenu;
    public Menu Menu;
    public HashSet<PlayerModel> PlayersTanks;
    public Dictionary<TypeOfObject, Texture2D> SceneDictionary;
    public Dictionary<string, Texture2D> TanksImage;
    private Dictionary<int, Texture2D> _frameDictionary;
    public SceneController[,] SceneObjects;
    public SpriteBatch SpriteBatch;
    public StateOfGame State = StateOfGame.MainMenu;
    public readonly ConstructorModel Constructor;
    public int NumberOfLevel;
    public static int  BlockSize = 64;
    public List<PlayerController> PlayerControllers;
    public List<PlayerView> PlayerViews;
    public readonly CollisionDetected CollisionDetected;
    private readonly UpdateGame _updateGame;
    private readonly DrawGame _drawGame;
    public List<Vector2> CoordinateForEnemy;
    public List<Vector2> CoordinateForPlayer;
    public MenuController MainMenuController;
    private MusicController _musicController;
    public Vector2 CoordinateOfStaff;
    public SettingsController SettingsController;
    public LevelController LevelController { get; }
    public GameData GameData { get; private set; }
    public const string LevelsPath = "Levels/levels.json";
    
    public BattleCity()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        CollisionDetected = new CollisionDetected(this);
        _updateGame = new UpdateGame(this);
        _drawGame = new DrawGame(this);
        Constructor = new ConstructorModel(this);
        LevelController = new LevelController(this);
    }
    
    private void LoadGameData()
    {
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = string.Concat(appDirectory.AsSpan(0,
            appDirectory.IndexOf("\\bin", StringComparison.Ordinal)), $"\\Settings/tanks.json");
        var json = File.ReadAllText(filePath);
        GameData = JsonConvert.DeserializeObject<GameData>(json);
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
        LoadGameData();
        LoadGameTextures();
        LoadSceneTextures();
        LoadFrameTextures();
        LoadMusic();
        InitializeGameObjects();

        Constructor.LoadStartMap();
    }

    private void LoadGameTextures()
    {
        MainMenu = new MenuModel(new Vector2(270, 495), this);
        MainMenuController = new MenuController(MainMenu);
        GameDefeat = new Defeat(Content.Load<Texture2D>("gameOver"));
        Shot.Texture = Content.Load<Texture2D>("bullet");
        DrawGame.Vision = Content.Load<Texture2D>("tank");
        DrawGame.LevelIcon = Content.Load<Texture2D>("flag");
        var textBlock = Content.Load<SpriteFont>("text");
        SettingsController = new SettingsController(textBlock, this);
        DrawGame.TextBlock = textBlock;
        ConstructorView.TextBlock = textBlock;
        DrawGame.FirstPlayerHp = Content.Load<Texture2D>("hp1Tank");
        DrawGame.SecondPlayerHp = Content.Load<Texture2D>("hp2Tank");
        TanksImage = new Dictionary<string, Texture2D>()
        {
            { "playerLevel1", Content.Load<Texture2D>("player") },
            { "enemyLevel1", Content.Load<Texture2D>("tank1") },
            { "enemyLevel2", Content.Load<Texture2D>("tank2") },
            { "enemyLevel3", Content.Load<Texture2D>("tank3") }
        };

        Menu = new Menu(Content.Load<Texture2D>("MainMenu"), Content.Load<Texture2D>("cursor"));
    }

    private void LoadMusic()
    {
        _musicController = new MusicController(this);
        _musicController.LoadContent();
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
        _frameDictionary = new Dictionary<int, Texture2D>
        {
            { 0, Content.Load<Texture2D>("bang1") },
            { 1, Content.Load<Texture2D>("bang2") },
            { 2, Content.Load<Texture2D>("bang3") }
        };
        BangView.TextureOfFrame = _frameDictionary;
    }

    private void InitializeGameObjects()
    {
        PlayersTanks = new HashSet<PlayerModel>();
        EnemyTanks = new HashSet<EnemyController>();
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