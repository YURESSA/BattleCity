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
    Staff,
    Wall
}

public class ScenicObject : RectObjects
{
    public ScenicObject(Vector2 position, TypeOfObject type, Texture2D sprite, int size) : base(position, size, sprite)
    {
        Type = type;
    }

    public TypeOfObject Type { get; set; }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Sprite, Position, Color.White);
    }
}