using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class RectObjects : GameObject
{
    public Vector2 Origin;
    public Texture2D Sprite;

    public RectObjects(Vector2 position, int size, Texture2D sprite)
    {
        Position = position;
        Size = size;
        Sprite = sprite;
        Width = sprite.Width;
        Height = sprite.Height;
        Origin = new Vector2(Sprite.Width / 2f, Sprite.Height / 2f);
    }

    public Vector2 Position { get; set; }
    public int Size { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public bool Intersect(MovedObject that)
    {
        var a = new Rectangle((int)Position.X, (int)Position.Y, Height, Width);
        var b = new Rectangle((int)that.Position.X, (int)that.Position.Y, that.Width, that.Height);
        return a.Intersects(b);
    }
}