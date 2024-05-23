using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BattleCity;

public class BattleCity : Game
{
    public const int CellSize = 64;
    public readonly HashSet<Shot> BulletObjects = new();
    private readonly GraphicsDeviceManager _graphics;
    public TimeSpan _elapsedTime;
    public Texture2D EnemyImage;
    public int EnemyInLevel;
    public HashSet<EnemyController> EnemyTanks;
    public readonly HashSet<BangModel> BangModels = new();
    public Dictionary<int, string> FileNameDictionary;
    public Defeat GameDefeat;
    public MenuModel MainMenu;
    public Menu Menu;
    public Texture2D PlayerImage;
    public HashSet<PlayerModel> PlayersTanks;
    public Dictionary<TypeOfObject, Texture2D> SceneDictionary;
    private Dictionary<int, Texture2D> _frameDictionary;
    public SceneController[,] SceneObjects;
    public SpriteBatch SpriteBatch;
    public StateOfGame State = StateOfGame.MainMenu;
    public readonly ConstructorModel Constructor;
    public int NumberOfLevel;
    public List<PlayerController> PlayerControllers;
    public List<PlayerView> _playerViews;
    public readonly CollisionDetected _collisionDetected;
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
        LevelController = new LevelController(this);
    }

    public LevelController LevelController { get; }

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
        Shot.Texture = Content.Load<Texture2D>("bullet");
        DrawGame.Vision = Content.Load<Texture2D>("tank");
        DrawGame.LevelIcon = Content.Load<Texture2D>("flag");
        var textBlock = Content.Load<SpriteFont>("text");
        DrawGame.TextBlock = textBlock;
        ConstructorView.TextBlock = textBlock;
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