using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace BattleCity;

public class BangView
{
    private readonly BangModel _bangModel;
    public static Dictionary<int, Texture2D> TextureOfFrame;

    public BangView(BangModel bangModel)
    {
        _bangModel = bangModel;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        var texture = TextureOfFrame[_bangModel.CurrentFrame];
        spriteBatch.Draw(texture, _bangModel.Position - _bangModel.Origin, Color.White);
    }
}