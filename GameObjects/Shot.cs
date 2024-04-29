using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Shot : MovedObject
{
    public float Angle;
    public Func<MovedObject, bool> HasCollision;

    public Shot(Vector2 position, float speed, int size, float angle, Func<MovedObject, bool> hasCollision) : base(
        position, size, speed, SpriteOfBullet)
    {
        Position = position - Origin;
        Angle = angle;
        ShotHasCollisions = false;
        HasCollision = hasCollision;
    }

    public static Texture2D SpriteOfBullet { get; set; }
    public bool ShotHasCollisions { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Width, Height);
        spriteBatch.Draw(Sprite, Position + Origin, sourceRect, Color.White,
            Angle, Origin, 1f, SpriteEffects.None, 0f);
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


        if (direction.Length() > 0f)
        {
            Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) ShotHasCollisions = true;
        }
    }
}