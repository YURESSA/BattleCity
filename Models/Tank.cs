using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Tank : MovedObject
{
    public float Angle = MathHelper.TwoPi;

    public HashSet<Shot> bulletObjects;
    protected Vector2 Direction;
    protected TimeSpan elapsedTime = TimeSpan.Zero;
    protected Func<MovedObject, bool> HasCollision;

    protected Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects) :
        base(position + new Vector2(2, 2), speed, null, sprite.Width, sprite.Height)
    {
        this.bulletObjects = bulletObjects;
        HasCollision = hasCollision;
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
        var Size = 64;
        if (Angle == MathHelper.Pi)
            return new Vector2((int)Position.X / Size, (int)Position.Y / Size);
        if (Angle == MathHelper.TwoPi || Angle == -MathHelper.PiOver2)
            return new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size);
        return new Vector2((int)Position.X / Size, (int)Position.Y / Size);
    }
}