using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Tank : MovedObject
{
    public float Angle = MathHelper.TwoPi;

    private readonly HashSet<Shot> _bulletObjects;
    protected Vector2 Direction;
    protected TimeSpan ElapsedTime = TimeSpan.Zero;
    protected readonly Func<MovedObject, bool> HasCollision;
    private int _hp;
    private readonly Vector2 _startPosition;

    protected Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp) :
        base(position + new Vector2(2, 2), speed, null, sprite.Width, sprite.Height, isAlive)
    {
        _hp = hp;
        _startPosition = position;
        _bulletObjects = bulletObjects;
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

    protected void MoveUp()
    {
        Angle = MathHelper.TwoPi;
        Direction = new Vector2(0, -Speed);
    }

    protected void MoveDown()
    {
        Angle = MathHelper.Pi;
        Direction = new Vector2(0, Speed);
    }

    protected void Shoot()
    {
        var shot = new Shot(Position + Origin, 0.5f, 14, Angle, HasCollision, this, true);
        _bulletObjects.Add(shot);
        ElapsedTime = TimeSpan.FromMilliseconds(1000);
    }

    public void Kill()
    {
        if (_hp > 1)
        {
            _hp -= 1;
            Position = _startPosition;
        }
        else
        {
            IsAlive = false;
        }
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