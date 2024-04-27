using System;
using System.Collections.Generic;
using BattleCity;
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

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;

    private HashSet<Tank> tanksObjects;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        MainMenu.MainMenuBackground = Content.Load<Texture2D>("MainMenu");
        tanksObjects = new HashSet<Tank>();
        var tankImage = Content.Load<Texture2D>("tank1");
        var playersTank = new Tank(0.1f, new Vector2(400, 300), tankImage);
        tanksObjects.Add(playersTank);
    }

    protected override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case StateOfGame.MainMenu:
                MainMenu.Update(gameTime);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    _state = StateOfGame.Game;
                break;
            case StateOfGame.Game:
                foreach (var tanks in tanksObjects)
                    tanks.Update(gameTime);
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    _state = StateOfGame.MainMenu;
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        switch (_state)
        {
            case StateOfGame.MainMenu:
                MainMenu.Draw(_spriteBatch);
                break;
            case StateOfGame.Game:
                foreach (var tank in tanksObjects)
                    tank.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}