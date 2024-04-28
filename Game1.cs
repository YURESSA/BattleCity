using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace BattleCity;

internal enum StateOfGame
{
    MainMenu,
    Game,
    Final,
    Pause
}

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;
    private const int CellSize = 64;

    private MainMenu mainMenu;
    private HashSet<Tank> TanksObjects;

    public HashSet<ScenicObject> ScenicsObjects;
    public HashSet<ScenicObject> ScenicsForDeleted = new();
    public HashSet<Shot> BulletObjects;
    private HashSet<Shot> _bulletForDeleted;

    public Game1()
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
        mainMenu = new MainMenu(Content.Load<Texture2D>("MainMenu"), CellSize);
        TanksObjects = new HashSet<Tank>();
        var tankImage = Content.Load<Texture2D>("tank1");
        var playersTank = new Tank(0.1f, new Vector2(326, 832), tankImage, CellSize, HasCollision);
        Shot.SpriteOfBullet = Content.Load<Texture2D>("bullet");
        var images = new Dictionary<TypeOfObject, Texture2D>()
        {
            { TypeOfObject.None, Content.Load<Texture2D>("none") },
            { TypeOfObject.Bricks, Content.Load<Texture2D>("bricks") },
            { TypeOfObject.Concrete, Content.Load<Texture2D>("concrete") },
            { TypeOfObject.Leaves, Content.Load<Texture2D>("leaves") },
            { TypeOfObject.Water, Content.Load<Texture2D>("water") },
            { TypeOfObject.Staff, Content.Load<Texture2D>("staff") }
        };
        ScenicsObjects = ReaderOfMap.Reader(images, CellSize);
        TanksObjects.Add(playersTank);
        BulletObjects = playersTank.bulletObjects;
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
                foreach (var tanks in TanksObjects)
                    tanks.Update(gameTime);

                _bulletForDeleted = new HashSet<Shot>();
                foreach (var bullet in BulletObjects)
                {
                    bullet.Update(gameTime);
                    if (bullet.ShotHasCollisions)
                        _bulletForDeleted.Add(bullet);
                }

                foreach (var scenic in ScenicsForDeleted)
                {
                    ScenicsObjects.Remove(scenic);
                    ScenicsObjects.Add(new ScenicObject(scenic.Position, TypeOfObject.None,
                        Content.Load<Texture2D>("none"), CellSize));
                }

                BulletObjects.RemoveWhere(element => element.ShotHasCollisions);

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
                foreach (var tank in TanksObjects)
                    tank.Draw(_spriteBatch);
                foreach (var bulletObject in BulletObjects)
                    bulletObject.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public bool HasCollision(MovedObject obj)
    {
        foreach (var scene in ScenicsObjects)
            if (scene.Intersect(obj) && scene.Type != TypeOfObject.None)
            {
                if (scene.Type == TypeOfObject.Bricks && obj is Shot) ScenicsForDeleted.Add(scene);
                    return true;
            }

        return false;
    }
}