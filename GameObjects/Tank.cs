using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;


public class Tank: GameObject
{
    public static Texture2D TankImage { get; set; }
    
    public static void Update(GameTime gameTime) { }

    public static void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(TankImage, Vector2.Zero, Color.White);
    }  
}
