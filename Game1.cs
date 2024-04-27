
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
    private HashSet<Tank> tanksObjects;
    public List<ScenicObject> scenicsObjects;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        mainMenu = new MainMenu(Content.Load<Texture2D>("MainMenu"), CellSize);
        tanksObjects = new HashSet<Tank>();
        var tankImage = Content.Load<Texture2D>("tank1");
        var playersTank = new Tank(0.1f, new Vector2(96, 96), tankImage, CellSize, HasCollision);


        var images = new Dictionary<TypeOfObject, Texture2D>()
        {
            { TypeOfObject.None, Content.Load<Texture2D>("none") },
            { TypeOfObject.Bricks, Content.Load<Texture2D>("bricks") },
            { TypeOfObject.Concrete, Content.Load<Texture2D>("concrete") },
            { TypeOfObject.Leaves, Content.Load<Texture2D>("leaves") },
            { TypeOfObject.Water, Content.Load<Texture2D>("water") },
            { TypeOfObject.Staff, Content.Load<Texture2D>("staff") }
        };
        scenicsObjects = ReaderOfMap.Reader(images, CellSize);
        tanksObjects.Add(playersTank);
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
                foreach (var tanks in tanksObjects)
                    tanks.Update(gameTime);
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
                foreach (var scenic in scenicsObjects)
                    scenic.Draw(_spriteBatch);
                foreach (var tank in tanksObjects)
                    tank.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public bool HasCollision(Tank obj)
    {
        foreach (var scene in scenicsObjects)
            if (scene.Intersect(obj) && scene.Type != TypeOfObject.None)
                return true;
        return false;
    }
}