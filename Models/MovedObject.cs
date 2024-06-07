using Microsoft.Xna.Framework;

namespace BattleCity;

public class MovedObject : RectObjects
{
    protected float Speed { get; }

    protected MovedObject(Vector2 position, float speed, Tank parent, int spriteWidth, int spriteHeight, bool isAlive) :
        base(position,
            spriteWidth, spriteHeight, isAlive)
    {
        Speed = speed;
    }


    public virtual void Kill()
    {
    }
}