using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class PlayerModel : Tank
{
    public PlayerModel(float speed, Vector2 position, Texture2D sprite, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp) :
        base(speed, position, sprite, cellSize, hasCollision, bulletObjects, isAlive, hp)
    {
    }

    private ControlButton ControlButton = new()
    {
        Up = Keys.W,
        Down = Keys.S,
        Left = Keys.A,
        Right = Keys.D,
        Shoot = Keys.Space
    };

    public void Update(GameTime gameTime)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;

        Direction = new Vector2();

        HandleInput();
        HandleShooting();

        if (Direction.Length() > 0f)
        {
            Position += Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) Position -= Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }

    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();
        Direction = Vector2.Zero;

        if (keyboardState.IsKeyDown(ControlButton.Left))
            MoveLeft();
        if (keyboardState.IsKeyDown(ControlButton.Right))
            MoveRight();
        if (keyboardState.IsKeyDown(ControlButton.Up))
            MoveUp();
        if (keyboardState.IsKeyDown(ControlButton.Down))
            MoveDown();
    }

    private void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero &&
            Keyboard.GetState().IsKeyDown(Keys.Space))
            Shoot();
    }
}