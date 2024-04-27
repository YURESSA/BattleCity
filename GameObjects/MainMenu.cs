using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class MainMenu : GameObject
{
    private static Texture2D MainMenuBackground { get; set; }
    private static int CellSize;

    public MainMenu(Texture2D sprite, int cellSize)
    {
        MainMenuBackground = sprite;
        CellSize = cellSize;
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(MainMenuBackground, new Vector2((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - 1433) / 2, CellSize), Color.White);
    }

    public static void Update(GameTime gameTime)
    {
    }
}