using System.Collections.Generic;
using BattleCity;
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

public class ScenicObject : GameObject
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

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, Size, Size);
        spriteBatch.Draw(ScenicImage, Position, sourceRect, Color.White);
    }
}