using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class MovedObject : RectObjects
{
    public MovedObject(Vector2 position, int size, float speed, Texture2D sprite, Tank parent) : base(position, size, sprite)
    {
        Speed = speed;
        Parent = parent;
    }

    public float Speed { get; set; }
    public Tank Parent { get; set; }
}