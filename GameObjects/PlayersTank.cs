using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class PlayersTank : Tank
{
    private float Angle = MathHelper.TwoPi;

    public PlayersTank(float speed, Vector2 position, Texture2D sprite, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects) :
        base(speed, position, sprite, cellSize, hasCollision, bulletObjects)
    {
    }
    

    public void Update(GameTime gameTime)
    {
        elapsedTime -= gameTime.ElapsedGameTime;

        Direction = new Vector2();

        HandleInput();
        HandleShooting(gameTime);

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

        if (keyboardState.IsKeyDown(Keys.A))
            MoveLeft();
        if (keyboardState.IsKeyDown(Keys.D))
            MoveRight();
        if (keyboardState.IsKeyDown(Keys.W))
            MoveFront();
        if (keyboardState.IsKeyDown(Keys.S))
            MoveBack();
    }

    private void HandleShooting(GameTime gameTime)
    {
        if (elapsedTime <= TimeSpan.Zero &&
            Keyboard.GetState().IsKeyDown(Keys.Space))
            Shoot();
    }
}