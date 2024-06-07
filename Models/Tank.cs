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
    public int Hp;
    public float Angle = MathHelper.TwoPi;
    public Vector2 Direction;
    protected TimeSpan ElapsedTime = TimeSpan.Zero;
    private float bulletSpeed;

    private readonly TimeSpan _shieldDuration = TimeSpan.FromSeconds(2);
    private TimeSpan _creationTime;

    protected Tank(float speed, Vector2 position, Texture2D sprite, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp, float bulletSpeed) :
        base(position + new Vector2(2, 2), speed, null, sprite.Width, sprite.Height, isAlive)
    {
        Hp = hp;
        this.bulletSpeed = bulletSpeed;
        _startPosition = position;
        _bulletObjects = bulletObjects;
        HasCollision = hasCollision;
        _creationTime = TimeSpan.FromMilliseconds(Environment.TickCount);
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
        var shot = new Shot(Position + Origin, bulletSpeed, Angle, HasCollision, this, true);
        _bulletObjects.Add(shot);
        MusicController.PlayShot();
        ElapsedTime = TimeSpan.FromMilliseconds(1000);
    }

    public override void Kill()
    {
        if (IsShieldActive()) return;

        if (Hp > 1)
        {
            Hp -= 1;
            Position = _startPosition;
            _creationTime = TimeSpan.FromMilliseconds(Environment.TickCount); // Update shield timer
        }
        else
        {
            IsAlive = false;
        }
    }

    public virtual Vector2 GetCoordinate()
    {
        var halfCellSize = Width / 2;

        var centerX = Position.X + halfCellSize;
        var centerY = Position.Y + halfCellSize;

        if (Math.Abs(centerX % 32) < 6 && Math.Abs(centerY % 32) < 6 && Position.X % BattleCity.BlockSize < 8 &&
            Position.Y % BattleCity.BlockSize < 8) 
            return new Vector2((int)centerX / BattleCity.BlockSize, (int)centerY / BattleCity.BlockSize);
        return Vector2.Zero;
    }

    private bool IsShieldActive()
    {
        if (this is EnemyModel)
            return false;
        var currentTime = TimeSpan.FromMilliseconds(Environment.TickCount);
        return currentTime - _creationTime < _shieldDuration;
    }
}