using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BattleCity;

public class SceneController
{
    public SceneModel SceneModel { get; }
    public SceneView SceneView { get; init; }

    public SceneController(SceneModel sceneModel)
    {
        SceneModel = sceneModel;
    }

    public void Update(GameTime gameTime)
    {
        SceneModel.Update(gameTime);
    }

    public SceneController Clone(Dictionary<TypeOfObject, Texture2D> textures, Texture2D noneTexture)
    {
        return new SceneController(new SceneModel(SceneModel.Position, SceneModel.Type, SceneModel.Width,
            SceneModel.Height, SceneModel.IsAlive))
        {
            SceneView = new SceneView(textures, noneTexture)
        };
    }

    public static SceneController GetScene(Vector2 position, TypeOfObject type, Texture2D texture,
        Texture2D noneTexture, bool isAlive, Dictionary<TypeOfObject, Texture2D> textures)
    {
        var sceneModel = new SceneModel(position, type, texture.Width, texture.Height, isAlive);
        var sceneView = new SceneView(textures, noneTexture);
        var sceneController = new SceneController(sceneModel) { SceneView = sceneView };
        return sceneController;
    }
}