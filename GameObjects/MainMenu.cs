using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class MainMenu : GameObject
{
    public static Texture2D MainMenuBackground { get; set; }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(MainMenuBackground, Vector2.Zero, Color.White);
    }
    public static void Update(GameTime gameTime) { }      
}