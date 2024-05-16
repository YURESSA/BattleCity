using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class PlayerView
{
    private readonly Texture2D _texture;

    public PlayerView(Texture2D texture)
    {
        _texture = texture;
    }

    public void Draw(SpriteBatch spriteBatch, PlayerModel playerModel)
    {
        var sourceRect = new Rectangle(0, 0, playerModel.Width, playerModel.Height);
        spriteBatch.Draw(_texture, playerModel.Position + playerModel.Origin, sourceRect, Color.White,
            playerModel.Angle, playerModel.Origin, 1f, SpriteEffects.None, 0f);
    }
}