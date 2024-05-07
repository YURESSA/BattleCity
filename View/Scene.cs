using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class Scene
{
    private readonly Texture2D _texture;
    public readonly ScenicObject SceneModel;

    public Scene(Vector2 position, TypeOfObject type, Texture2D sprite, int size)
    {
        SceneModel = new ScenicObject(position, type, sprite, size);
        _texture = sprite;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_texture, SceneModel.Position, Color.White);
    }

    public static void Update(GameTime gameTime, Scene[,] map, HashSet<Scene> scenicForDeleted, Texture2D noneTexture)
    {
        foreach (var scenic in scenicForDeleted)
            map[(int)(scenic.SceneModel.Position.Y / scenic.SceneModel.Height),
                    (int)(scenic.SceneModel.Position.X / scenic.SceneModel.Width)] =
                new Scene(scenic.SceneModel.Position, TypeOfObject.None,
                    noneTexture, 64);
    }
}