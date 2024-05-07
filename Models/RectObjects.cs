using Microsoft.Xna.Framework;

namespace BattleCity;

public class RectObjects
{
    public Vector2 Origin;

    public RectObjects(Vector2 position, int spriteWidth, int spriteHeight)
    {
        Position = position;
        Width = spriteWidth;
        Height = spriteHeight;
        Origin = new Vector2(Width / 2f, Height / 2f);
    }

    public Vector2 Position { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public bool Intersect(MovedObject that)
    {
        var a = new Rectangle((int)Position.X, (int)Position.Y, Height, Width);
        var b = new Rectangle((int)that.Position.X, (int)that.Position.Y, that.Width, that.Height);
        return a.Intersects(b);
    }
}