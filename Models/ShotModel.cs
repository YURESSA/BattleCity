using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class ShotModel : MovedObject
{
    public float Angle;
    public Func<MovedObject, bool> HasCollision;
    public Tank Parent;

    public ShotModel(Vector2 position, float speed, int size, float angle,
        Func<MovedObject, bool> hasCollision, Tank parent, Texture2D SpriteOfBullet, bool isAlive) : base(
        position, speed, parent, SpriteOfBullet.Width, SpriteOfBullet.Height, isAlive)
    {
        Position = position - Origin;
        Angle = angle;
        ShotHasCollisions = false;
        HasCollision = hasCollision;
        Parent = parent;
    }

    public static Texture2D SpriteOfBullet { get; set; }
    public bool ShotHasCollisions { get; set; }


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


        if (direction.Length() > 0f)
        {
            Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) ShotHasCollisions = true;
        }
    }

    public override void Kill()
    {
        IsAlive = false;
    }
}