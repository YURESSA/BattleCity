using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Menu
{
    private readonly Texture2D _texture;
    private readonly Texture2D _cursor;

    public Menu(Texture2D sprite, Texture2D cursor)
    {
        _texture = sprite;
        _cursor = cursor;
    }

    public void Draw(SpriteBatch spriteBatch, MenuModel menuModel)
    {
        spriteBatch.Draw(_texture,
            new Vector2(0, 0), Color.White);
        spriteBatch.Draw(_cursor, menuModel.Position, Color.White);
    }
}