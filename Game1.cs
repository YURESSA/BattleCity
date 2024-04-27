using System;
using BattleCity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

enum StateOfGame
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
    private StateOfGame state = StateOfGame.MainMenu;
    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
        _graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        MainMenu.MainMenuBackground = Content.Load<Texture2D>("MainMenu");
        Tank.TankImage = Content.Load<Texture2D>("tank1");
        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        switch (state)
        {
            case StateOfGame.MainMenu:
                MainMenu.Update(gameTime);
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    state = StateOfGame.Game;
                break;
           case StateOfGame.Game:
               Tank.Update(gameTime);
               if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                   state = StateOfGame.MainMenu;
               break;
        }
        
       

        // TODO: Add your update logic here
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        switch (state)
        {
            case StateOfGame.MainMenu:
                MainMenu.Draw(_spriteBatch);
                break;
            case StateOfGame.Game:
                Tank.Draw(_spriteBatch);
                break;
        }
        
        _spriteBatch.End();
        // TODO: Add your drawing code here

        base.Draw(gameTime);
    }
}