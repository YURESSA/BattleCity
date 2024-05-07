using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class MenuModel
{
    public MenuModel(Texture2D sprite, int cellSize)
    {
        MainMenuBackground = sprite;
    }

    private static Texture2D MainMenuBackground { get; set; }

    public static void Update(GameTime gameTime)
    {
    }
}