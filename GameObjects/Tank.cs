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

public class Tank
{
    private TimeSpan elapsedTime;
    public float Angle = MathHelper.TwoPi;
    private float Speed { get; set; }
    public Vector2 Position { get; set; }
    private Texture2D TankImage { get; set; }
    public int Size { get; set; }
    private Vector2 origin;
    private Func<Tank, bool> HasCollision;
    public int counterOfShot = 0;
    public HashSet<Shot> bulletObjects;
    
    public Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<Tank, bool> hasCollision)
    {
        Speed = speed;
        Position = position;
        TankImage = sprite;
        origin = new Vector2(cellSize / 2f, cellSize / 2f);
        Size = cellSize - 4;
        HasCollision = hasCollision;
        bulletObjects = new HashSet<Shot>();
    }

    public void Update(GameTime gameTime)
    {
        elapsedTime += gameTime.ElapsedGameTime;

        //если нужное время на выстрел прошло то можно еще раз стрелять
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
            var shot = new Shot(Position, 0.5f, Size, Angle);
            bulletObjects.Add(shot);
            counterOfShot++;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Size, Size);
        spriteBatch.Draw(TankImage, Position, sourceRect, Color.White, Angle, origin, 1f, SpriteEffects.None, 0f);
    }
}