using Microsoft.Xna.Framework;

namespace BattleCity;

public class MovedObject : RectObjects
{
    public MovedObject(Vector2 position, float speed, Tank parent, int spriteWidth, int spriteHeight, bool isAlive) :
        base(position,
            spriteWidth, spriteHeight, isAlive)
    {
        Speed = speed;
        Parent = parent;
    }

    public float Speed { get; set; }
    public Tank Parent { get; set; }

    public virtual void Kill()
    {
    }
}