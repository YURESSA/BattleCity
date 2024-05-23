using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class EnemyView
{
    private readonly Texture2D _texture;

    public EnemyView(Texture2D texture)
    {
        _texture = texture;
    }

    public void Draw(SpriteBatch spriteBatch, EnemyModel enemyModel)
    {
        var sourceRect = new Rectangle(0, 0, enemyModel.Width, enemyModel.Height);
        var drawPosition = enemyModel.Position + enemyModel.Origin;

        spriteBatch.Draw(_texture, drawPosition, sourceRect, Color.White,
            enemyModel.Angle, enemyModel.Origin, 1f, SpriteEffects.None, 0f);
    }
}