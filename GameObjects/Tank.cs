using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Tank : MovedObject
{
    protected float Angle = MathHelper.TwoPi;

    public int counterOfShot;
    public HashSet<Shot> bulletObjects;
    protected Vector2 Direction;
    protected TimeSpan elapsedTime = TimeSpan.Zero;
    protected Func<MovedObject, bool> HasCollision;

    protected Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects) :
        base(position + new Vector2(2, 2), cellSize, speed, sprite, null)
    {
        this.bulletObjects = bulletObjects;
        HasCollision = hasCollision;
        counterOfShot = 0;
    }

    protected void MoveLeft()
    {
        Angle = -MathHelper.PiOver2;
        Direction = new Vector2(-Speed, 0);
    }

    protected void MoveRight()
    {
        Angle = +MathHelper.PiOver2;
        Direction = new Vector2(Speed, 0);
    }

    protected void MoveFront()
    {
        Angle = MathHelper.TwoPi;
        Direction = new Vector2(0, -Speed);
    }

    protected void MoveBack()
    {
        Angle = MathHelper.Pi;
        Direction = new Vector2(0, Speed);
    }

    protected void Shoot()
    {
        var shot = new Shot(Position + Origin, 0.5f, 14, Angle, HasCollision, this);
        bulletObjects.Add(shot);
        elapsedTime = TimeSpan.FromMilliseconds(1000);
    }
    
    public virtual Vector2 GetCoordinate()
    {
        return Angle switch
        {
            MathHelper.Pi => new Vector2((int)Position.X / Size, (int)Position.Y / Size),
            MathHelper.TwoPi => new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size),
            -MathHelper.PiOver2 => new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size),
            _ => new Vector2((int)Position.X / Size, (int)(Position.Y) / Size)
        };
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Width, Height);
        spriteBatch.Draw(Sprite, Position + Origin, sourceRect, Color.White, Angle, Origin, 1f, SpriteEffects.None, 0f);
    }
}