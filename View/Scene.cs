using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Scene
{
    private readonly Texture2D _noneTexture;
    public readonly SceneObjectsModel SceneModel;
    private Texture2D _texture;

    public Scene(Vector2 position, TypeOfObject type, Texture2D sprite, Texture2D noneTexture, int size, bool isAlive)
    {
        SceneModel = new SceneObjectsModel(position, type, sprite.Width, sprite.Height, size, isAlive);
        _texture = sprite;
        _noneTexture = noneTexture;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, SceneModel.Position, Color.White);
    }

    public void Update(GameTime gameTime)
    {
        if (SceneModel.IsAlive == false) _texture = _noneTexture;
        SceneModel.Update(gameTime);
    }

    public object Clone()
    {
        return new Scene(SceneModel.Position, SceneModel.Type, _texture, _noneTexture,
            SceneModel.Height, SceneModel.IsAlive);
    }
}