using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Scene
{
    private Texture2D _texture;
    public readonly SceneObjectsModel SceneModel;
    private readonly Texture2D _noneTexture;

    public Scene(Vector2 position, TypeOfObject type, Texture2D sprite, Texture2D noneTexture, int size, bool isAlive)
    {
        SceneModel = new SceneObjectsModel(position, type, sprite.Width, sprite.Height, size, isAlive);
        _texture = sprite;
        this._noneTexture = noneTexture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, SceneModel.Position, Color.White);
    }

    public void Update(GameTime gameTime)
    {
        if (SceneModel.IsAlive == false)
        {
            _texture = _noneTexture;
        }
        SceneModel.Update(gameTime);
    }
}