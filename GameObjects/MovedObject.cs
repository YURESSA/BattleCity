using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class MovedObject : RectObjects
{
    public MovedObject(Vector2 position, int size, float speed, Texture2D sprite) : base(position, size, sprite)
    {
        Speed = speed;
    }

    public float Speed { get; set; }
}