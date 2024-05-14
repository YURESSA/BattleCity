using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Enemy
{
    public readonly EnemyModel _enemyModel;
    private readonly Texture2D _texture;

    public Enemy(float speed, Vector2 position, Texture2D tankImage, int cellSize, Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp)
    {
        _enemyModel = new EnemyModel(speed, position, tankImage, cellSize, hasCollision, bulletObjects, isAlive, hp);
        _texture = tankImage;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, _enemyModel.Width, _enemyModel.Height);

        var drawPosition = _enemyModel.Position + _enemyModel.Origin;

        spriteBatch.Draw(_texture, drawPosition, sourceRect, Color.White,
            _enemyModel.Angle, _enemyModel.Origin, 1f, SpriteEffects.None, 0f);
    }


    public void Update(GameTime gameTime, Scene[,] map, Vector2 userCoordinates)
    {
        _enemyModel.Update(gameTime, map, userCoordinates);
    }
}