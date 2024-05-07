using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

internal enum StateOfGame
{
    MainMenu,
    Game,
    Final,
    Pause
}

public class BattleCity : Game
{
    private const int CellSize = 64;
    private readonly GraphicsDeviceManager _graphics;
    private HashSet<Shot> _bulletForDeleted;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;
    public HashSet<Shot> BulletObjects = new();
    private HashSet<Enemy> EnemyTanks;
    public HashSet<Enemy> EnemyTanksForDeleted = new();

    private Menu mainMenu;

    private HashSet<Player> PlayersTanks;
    public HashSet<Player> PlayersTanksForDeleted = new();
    public HashSet<Scene> ScenicsForDeleted = new();
    public Scene[,] ScenicsObjects;

    public BattleCity()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = 960;
        _graphics.PreferredBackBufferWidth = 1433;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        mainMenu = new Menu(Content.Load<Texture2D>("MainMenu"));
        PlayersTanks = new HashSet<Player>();
        EnemyTanks = new HashSet<Enemy>();
        Shot._texture = Content.Load<Texture2D>("bullet");
        var tankImage = Content.Load<Texture2D>("tank1");

        var playersTank = new Player(0.1f, new Vector2(326, 832), tankImage, CellSize, HasCollision, BulletObjects);
        PlayersTanks.Add(playersTank);

        var enemyTank = new Enemy(0.1f, new Vector2(64, 64), tankImage, CellSize, HasCollision, BulletObjects);
        EnemyTanks.Add(enemyTank);


        var images = new Dictionary<TypeOfObject, Texture2D>
        {
            { TypeOfObject.None, Content.Load<Texture2D>("none") },
            { TypeOfObject.Bricks, Content.Load<Texture2D>("bricks") },
            { TypeOfObject.Concrete, Content.Load<Texture2D>("concrete") },
            { TypeOfObject.Leaves, Content.Load<Texture2D>("leaves") },
            { TypeOfObject.Water, Content.Load<Texture2D>("water") },
            { TypeOfObject.Staff, Content.Load<Texture2D>("staff") },
            { TypeOfObject.Wall, Content.Load<Texture2D>("wall") }
        };
        ScenicsObjects = ReaderOfMap.Reader(images, CellSize, "input.txt");
    }

    protected override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case StateOfGame.MainMenu:
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    _state = StateOfGame.Game;
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                break;
            case StateOfGame.Game:
                var userCoordinates = Player.UpdateAndGetPlayersCoordinate(gameTime, PlayersTanks);

                Enemy.Update(gameTime, EnemyTanks, ScenicsObjects, userCoordinates);

                _bulletForDeleted = new HashSet<Shot>();
                foreach (var bullet in BulletObjects)
                {
                    bullet.Update(gameTime);
                    if (bullet.ShotModel.ShotHasCollisions)
                        _bulletForDeleted.Add(bullet);
                }

                Scene.Update(gameTime, ScenicsObjects, ScenicsForDeleted, Content.Load<Texture2D>("none"));
                foreach (var tank in EnemyTanksForDeleted) EnemyTanks.Remove(tank);
                foreach (var tank in PlayersTanksForDeleted) PlayersTanks.Remove(tank);

                BulletObjects.RemoveWhere(element => element.ShotModel.ShotHasCollisions);

                if (Keyboard.GetState().IsKeyDown(Keys.P))
                    _state = StateOfGame.MainMenu;
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        switch (_state)
        {
            case StateOfGame.MainMenu:
                GraphicsDevice.Clear(Color.Black);
                mainMenu.Draw(_spriteBatch);
                break;
            case StateOfGame.Game:
                GraphicsDevice.Clear(Color.Gray);
                foreach (var scenic in ScenicsObjects)
                    scenic.Draw(_spriteBatch);
                foreach (var tank in PlayersTanks)
                    tank.Draw(_spriteBatch);
                foreach (var tank in EnemyTanks)
                    tank.Draw(_spriteBatch);
                foreach (var bulletObject in BulletObjects)
                    bulletObject.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool HasCollision(MovedObject obj)
    {
        foreach (var scene in ScenicsObjects)
            if (scene.SceneModel.Intersect(obj) && scene.SceneModel.Type != TypeOfObject.None)
            {
                if (scene.SceneModel.Type == TypeOfObject.Bricks && obj is ShotModel) ScenicsForDeleted.Add(scene);
                return true;
            }


        foreach (var bullet in BulletObjects)
            if (obj.Intersect(bullet.ShotModel) && obj is ShotModel && obj != bullet.ShotModel)
            {
                BulletObjects.Remove(bullet);
                return true;
            }

        return false;
    }
}