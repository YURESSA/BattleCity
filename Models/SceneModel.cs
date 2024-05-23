using Microsoft.Xna.Framework;

namespace BattleCity;

public class SceneModel : RectObjects
{
    public SceneModel(Vector2 position, TypeOfObject type, int spriteWidth, int spriteHeight, bool isAlive)
        : base(position, spriteWidth, spriteHeight, isAlive)
    {
        Type = type;
    }

    public TypeOfObject Type { get; private set; }

    public void Update(GameTime gameTime)
    {
        if (!IsAlive)
            Type = TypeOfObject.None;
    }
}