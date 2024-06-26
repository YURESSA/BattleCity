using Microsoft.Xna.Framework;

namespace BattleCity;

public class RectObjects
{
    public bool IsAlive;
    public Vector2 Origin;

    protected RectObjects(Vector2 position, int spriteWidth, int spriteHeight, bool isAlive)
    {
        Position = position;
        Width = spriteWidth;
        Height = spriteHeight;
        Origin = new Vector2(Width / 2f, Height / 2f);
        IsAlive = isAlive;
    }

    public Vector2 Position { get; set; }
    public int Width { get; }
    public int Height { get; }

    public bool Intersect(MovedObject that)
    {
        var a = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        var b = new Rectangle((int)that.Position.X, (int)that.Position.Y, that.Width, that.Height);
        return a.Intersects(b);
    }
}