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

public class SceneObjectsModel : RectObjects
{
    public SceneObjectsModel(Vector2 position, TypeOfObject type, int spriteWidth, int spriteHeight, int size, bool isAlive) :
        base(position, spriteWidth, spriteHeight, isAlive)
    {
        Type = type;
    }

    public TypeOfObject Type { get; set; }

    public void Update(GameTime gameTime)
    {
        if (IsAlive == false)
            Type = TypeOfObject.None;
    }
}