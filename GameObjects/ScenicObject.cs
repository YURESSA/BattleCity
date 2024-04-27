using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BattleCity;

public enum TypeOfObject
{
    None,
    Bricks,
    Concrete,
    Leaves,
    Water,
    Staff
}

public class ScenicObject
{
    private Vector2 Position { get; set; }
    public TypeOfObject Type { get; set; }
    public Texture2D ScenicImage { get; set; }
    public int Size { get; set; }

    public ScenicObject(Vector2 position, TypeOfObject type, Texture2D scenicImage, int size)
    {
        Position = position;
        Type = type;
        ScenicImage = scenicImage;
        Size = size;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Size, Size);
        spriteBatch.Draw(ScenicImage, Position, sourceRect, Color.White);
    }

    public bool Intersect(Tank that)
    {
        if (that.Position.X < 64 + Size / 2  || that.Position.X > 896 - Size / 2)
            return true;
        if (that.Position.Y < 64 + Size / 2  || that.Position.Y > 896 - Size / 2)
            return true;
        var a = new Rectangle((int)Position.X, (int)Position.Y , Size - 3, Size - 3);
        var b = new Rectangle((int)that.Position.X - Size / 2, (int)that.Position.Y - Size / 2, 
            that.Size, that.Size);
        return a.Intersects(b);
    }
}