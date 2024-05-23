using Microsoft.Xna.Framework;

namespace BattleCity;

public class MovedObject : RectObjects
{
    protected float Speed { get; set; }
    public Tank Parent { get; set; }

    protected MovedObject(Vector2 position, float speed, Tank parent, int spriteWidth, int spriteHeight, bool isAlive) :
        base(position,
            spriteWidth, spriteHeight, isAlive)
    {
        Speed = speed;
        Parent = parent;
    }


    public virtual void Kill()
    {
    }
}