using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public enum TypeOfObject
{
    Bricks,
    Concrete,
    Leaves,
    Water,
    Staff
}

public class ScenicObject : GameObject
{
    private static Vector2 Position { get; set; }
    public static TypeOfObject Type { get; set; }

    public ScenicObject(Vector2 position, TypeOfObject type)
    {
        Position = position;
        Type = type;
    }

    public void Update(GameTime gameTime)
    {
    }

    public void Draw(SpriteBatch spriteBatch)
    {
    }
}