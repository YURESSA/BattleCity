using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace BattleCity;

public class Tank : GameObject
{
    public static float Angle = (float)MathHelper.TwoPi;
    private static float Speed { get; set; }
    private static Vector2 Position { get; set; }
    public static Texture2D TankImage { get; set; }
    private Vector2 origin;

    public Tank(float speed, Vector2 position, Texture2D sprite)
    {
        Speed = speed;
        Position = position;
        TankImage = sprite;
        origin = new Vector2(32 / 2f, 32 / 2f);
    }

    public void Update(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var direction = new Vector2();
        if (keyboardState.IsKeyDown(Keys.A))
        {
            Angle = -MathHelper.PiOver2;
            direction = new Vector2(-Speed, 0);
        }

        if (keyboardState.IsKeyDown(Keys.D))
        {
            Angle = +MathHelper.PiOver2;
            direction = new Vector2(Speed, 0);
        }

        if (keyboardState.IsKeyDown(Keys.W))
        {
            Angle = MathHelper.TwoPi;
            direction = new Vector2(0, -Speed);
        }

        if (keyboardState.IsKeyDown(Keys.S))
        {
            Angle = MathHelper.Pi;
            direction = new Vector2(0, Speed);
        }

        if (direction.Length() > 0f) Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, 32, 32);
        spriteBatch.Draw(TankImage, Position, sourceRect, Color.White, Angle, origin, 1f, SpriteEffects.None, 0f);
    }
}