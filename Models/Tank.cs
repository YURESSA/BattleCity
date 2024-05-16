using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Tank : MovedObject
{
    private readonly HashSet<Shot> _bulletObjects;
    private readonly Vector2 _startPosition;
    protected readonly Func<MovedObject, bool> HasCollision;
    private int _hp;
    public float Angle = MathHelper.TwoPi;
    public Vector2 Direction;
    protected TimeSpan ElapsedTime = TimeSpan.Zero;

    protected Tank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp) :
        base(position + new Vector2(2, 2), speed, null, sprite.Width, sprite.Height, isAlive)
    {
        _hp = hp;
        _startPosition = position;
        _bulletObjects = bulletObjects;
        HasCollision = hasCollision;
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

    public void MoveUp()
    {
        Angle = MathHelper.TwoPi;
        Direction = new Vector2(0, -Speed);
    }

    public void MoveDown()
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

    public override void Kill()
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
        var cellSize = 64;
        var halfCellSize = Width / 2;

        var centerX = Position.X + halfCellSize;
        var centerY = Position.Y + halfCellSize;

        if (Math.Abs(centerX % 32) < 6 && Math.Abs(centerY % 32) < 6 && Position.X % cellSize < 8 &&
            Position.Y % cellSize < 8) return new Vector2((int)centerX / cellSize, (int)centerY / cellSize);
        return Vector2.Zero;
    }
}