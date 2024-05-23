using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class SceneView
{
    private readonly Texture2D _noneTexture;
    private readonly Dictionary<TypeOfObject, Texture2D> _textures;

    public SceneView(Dictionary<TypeOfObject, Texture2D> textures, Texture2D noneTexture)
    {
        _textures = textures;
        _noneTexture = noneTexture;
    }

    public void Draw(SpriteBatch spriteBatch, SceneModel sceneModel)
    {
        var texture = sceneModel.IsAlive ? _textures[sceneModel.Type] : _noneTexture;
        spriteBatch.Draw(texture, sceneModel.Position, Color.White);
    }
}