using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Player
{
    private readonly Texture2D _texture;
    public readonly PlayerModel PlayerModel;

    public Player(float speed, Vector2 position, Texture2D tankImage, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp)
    {
        PlayerModel = new PlayerModel(speed, position, tankImage, cellSize, hasCollision, bulletObjects, isAlive, hp);
        _texture = tankImage;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, PlayerModel.Width, PlayerModel.Height);
        spriteBatch.Draw(_texture, PlayerModel.Position + PlayerModel.Origin, sourceRect, Color.White,
            PlayerModel.Angle, PlayerModel.Origin, 1f, SpriteEffects.None, 0f);
    }

    public void Update(GameTime gameTime)
    {
        PlayerModel.Update(gameTime);
    }
}