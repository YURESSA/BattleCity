using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class ShotModel : MovedObject
{
    public readonly float Angle;
    private readonly Func<MovedObject, bool> _hasCollision;
    public new readonly Tank Parent;
    public bool ShotHasCollisions { get; set; }

    public ShotModel(Vector2 position, float speed, float angle,
        Func<MovedObject, bool> hasCollision, Tank parent, Texture2D spriteOfBullet, bool isAlive) : base(
        position, speed, parent, spriteOfBullet.Width, spriteOfBullet.Height, isAlive)
    {
        Position = position - Origin;
        Angle = angle;
        ShotHasCollisions = false;
        _hasCollision = hasCollision;
        Parent = parent;
    }


    public void Update(GameTime gameTime)
    {
        var direction = new Vector2();

        switch (Angle)
        {
            case MathHelper.PiOver2:
                direction += new Vector2(Speed, 0);
                break;
            case MathHelper.Pi:
                direction += new Vector2(0, Speed);
                break;
            case -MathHelper.PiOver2:
                direction += new Vector2(-Speed, 0);
                break;
            case MathHelper.TwoPi:
                direction += new Vector2(0, -Speed);
                break;
        }


        if (!(direction.Length() > 0f)) return;
        Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (_hasCollision(this))
            ShotHasCollisions = true;
    }

    public override void Kill()
    {
        IsAlive = false;
    }
}