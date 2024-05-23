using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace BattleCity
{
    public class SceneController
    {
        public SceneModel SceneModel { get; }

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
            return new SceneController(new SceneModel(SceneModel.Position, SceneModel.Type, SceneModel.Width, SceneModel.Height, SceneModel.IsAlive))
            {
                SceneView = new SceneView(textures, noneTexture)
            };
        }

        public SceneView SceneView { get; set; }
    }
}
