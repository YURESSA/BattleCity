using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D11;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Texture2D = Microsoft.Xna.Framework.Graphics.Texture2D;

namespace BattleCity;

public class Tank:MovedObject
{
    private TimeSpan elapsedTime;
    public float Angle = MathHelper.TwoPi;
    private Func<MovedObject, bool> HasCollision;
    private int counterOfShot = 0;
    public HashSet<Shot> bulletObjects;
    
    public Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision) : base(position, cellSize, speed, sprite)
    {
        HasCollision = hasCollision;
        bulletObjects = new HashSet<Shot>();
    }

    public void Update(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime;

        if (elapsedTime >= TimeSpan.FromMilliseconds(1000))
        {
            elapsedTime -= TimeSpan.FromMilliseconds(1000);
            counterOfShot = 0;
        }
        var keyboardState = Keyboard.GetState();
        var direction = new Vector2();
        if (keyboardState.IsKeyDown(Keys.A))
        {
            Angle = -MathHelper.PiOver2;
            direction = new Vector2(-Speed, 0);
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            Angle = +MathHelper.PiOver2;
            direction = new Vector2(Speed, 0);
        }

        if (keyboardState.IsKeyDown(Keys.W))
        {
            Angle = MathHelper.TwoPi;
            direction = new Vector2(0, -Speed);
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            Angle = MathHelper.Pi;
            direction = new Vector2(0, Speed);
        }

        if (direction.Length() > 0f)
        {
            Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) Position -= direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
        if (keyboardState.IsKeyDown(Keys.Space) && counterOfShot == 0)
        {
            var shot = new Shot(Position + Origin, 0.5f, 14, Angle, HasCollision);
            bulletObjects.Add(shot);
            counterOfShot++;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Width, Height);
        spriteBatch.Draw(Sprite, Position + Origin, sourceRect, Color.White, Angle, Origin, 1f, SpriteEffects.None, 0f);
    }
}