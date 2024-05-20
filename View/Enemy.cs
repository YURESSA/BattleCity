using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Enemy
{
    public readonly EnemyModel EnemyModel;
    private readonly Texture2D _texture;

    public Enemy(float speed, Vector2 position, Texture2D tankImage, int cellSize, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp)
    {
        EnemyModel = new EnemyModel(speed, position, tankImage, hasCollision, bulletObjects, isAlive, hp);
        _texture = tankImage;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, EnemyModel.Width, EnemyModel.Height);

        var drawPosition = EnemyModel.Position + EnemyModel.Origin;

        spriteBatch.Draw(_texture, drawPosition, sourceRect, Color.White,
            EnemyModel.Angle, EnemyModel.Origin, 1f, SpriteEffects.None, 0f);
    }


    public void Update(GameTime gameTime, Scene[,] map, List<Vector2> userCoordinates,
        Vector2 CoordinateOfStaff)
    {
        EnemyModel.Update(gameTime, map, userCoordinates, CoordinateOfStaff);
    }
}