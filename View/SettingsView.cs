using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BattleCity;

public class MenuView
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteFont _font;

    public MenuView(SpriteBatch spriteBatch, SpriteFont font)
    {
        _spriteBatch = spriteBatch;
        _font = font;
    }

    public void Draw(List<MenuOption> menuOptions, int selectedOptionIndex)
    {
        for (var i = 0; i < menuOptions.Count; i++)
        {
            var color = i == selectedOptionIndex ? Color.Lime : Color.White;
            _spriteBatch.DrawString(_font, menuOptions[i].ToString(), new Vector2(100, 100 + i * 100), color);
        }
    }
}