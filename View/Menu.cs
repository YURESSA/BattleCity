using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Menu
{
    private readonly Texture2D _texture;
    public readonly SceneObjectsModel SceneModel;
    
    public Menu(Texture2D sprite)
    {
        _texture = sprite;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture,
            new Vector2(0, 0), Color.White);
    }
}