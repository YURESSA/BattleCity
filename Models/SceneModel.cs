using Microsoft.Xna.Framework;

namespace BattleCity;

public class SceneModel : RectObjects
{
    public SceneModel(Vector2 position, TypeOfObject type, int spriteWidth, int spriteHeight, int size,
        bool isAlive) :
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