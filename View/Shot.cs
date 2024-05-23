using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Shot
{
    public readonly ShotModel ShotModel;

    public Shot(Vector2 position, float speed, float angle,
        Func<MovedObject, bool> hasCollision, Tank parent, bool isAlive)
    {
        ShotModel = new ShotModel(position, speed, angle, hasCollision, parent, Texture, isAlive);
    }

    public static Texture2D Texture { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, ShotModel.Width, ShotModel.Height);
        spriteBatch.Draw(Texture, ShotModel.Position + ShotModel.Origin, sourceRect, Color.White,
            ShotModel.Angle, ShotModel.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void Update(GameTime gameTime)
    {
        ShotModel.Update(gameTime);
    }
}