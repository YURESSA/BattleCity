using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Tank : MovedObject
{
    public float Angle = MathHelper.TwoPi;
    public HashSet<Shot> bulletObjects;
    public int counterOfShot;
    public Vector2 Direction;
    public TimeSpan elapsedTime = TimeSpan.Zero;
    public Func<MovedObject, bool> HasCollision;

    public Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision) :
        base(position + new Vector2(2, 2), cellSize, speed, sprite)
    {
        HasCollision = hasCollision;
        bulletObjects = new HashSet<Shot>();
        counterOfShot = 0;
    }

    public void MoveLeft()
    {
        Angle = -MathHelper.PiOver2;
        Direction = new Vector2(-Speed, 0);
    }

    public void MoveRight()
    {
        Angle = +MathHelper.PiOver2;
        Direction = new Vector2(Speed, 0);
    }

    public void MoveFront()
    {
        Angle = MathHelper.TwoPi;
        Direction = new Vector2(0, -Speed);
    }

    public void MoveBack()
    {
        Angle = MathHelper.Pi;
        Direction = new Vector2(0, Speed);
    }

    public void Shoot()
    {
        var shot = new Shot(Position + Origin, 0.5f, 14, Angle, HasCollision);
        bulletObjects.Add(shot);
        elapsedTime = TimeSpan.FromMilliseconds(800);
    }

    public virtual Vector2 GetCoordinate()
    {
        return Angle switch
        {
            MathHelper.Pi => new Vector2((int)Position.X / Size, (int)Position.Y / Size),
            MathHelper.TwoPi => new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size),
            -MathHelper.PiOver2 => new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size),
            _ => new Vector2((int)Position.X / Size, (int)Position.Y / Size)
        };
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Width, Height);
        spriteBatch.Draw(Sprite, Position + Origin, sourceRect, Color.White, Angle, Origin, 1f, SpriteEffects.None, 0f);
    }
}