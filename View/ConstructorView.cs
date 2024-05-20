using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class ConstructorView
{
    private readonly BattleCity _battleCity;
    private Texture2D _rectangleBlock;

    public ConstructorView(BattleCity battleCity)
    {
        _battleCity = battleCity;
        InitializeTextures();
    }

    private void InitializeTextures()
    {
        _rectangleBlock = new Texture2D(_battleCity.GraphicsDevice, 1, 1);
        _rectangleBlock.SetData(new[] { Color.White });
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        DrawBackground(spriteBatch);
        DrawSceneObjects(spriteBatch);
        DrawEnemyAndPlayerPositions(spriteBatch);
    }

    private void DrawBackground(SpriteBatch spriteBatch)
    {
        var position = new Point(960, 0);
        var size = new Point(1500, 960);
        _battleCity.GraphicsDevice.Clear(Color.Black);
        var rectangle = new Rectangle(position, size);
        spriteBatch.Draw(_rectangleBlock, rectangle, new Color(128, 128, 128));
    }

    private void DrawSceneObjects(SpriteBatch spriteBatch)
    {
        DrawObject(spriteBatch, TypeOfObject.Bricks, new Vector2(960, 64));
        DrawObject(spriteBatch, TypeOfObject.Concrete, new Vector2(960, 192));
        DrawObject(spriteBatch, TypeOfObject.Leaves, new Vector2(960, 320));
        DrawObject(spriteBatch, TypeOfObject.Water, new Vector2(960, 448));

        var position = new Point(960, 576);
        var size = new Point(64, 64);
        _battleCity.GraphicsDevice.Clear(Color.Black);
        var rectangle = new Rectangle(position, size);
        spriteBatch.Draw(_rectangleBlock, rectangle, Color.Black);

        spriteBatch.Draw(_battleCity._enemyImage, new Vector2(960, 704), Color.White);
        spriteBatch.Draw(_battleCity._playerImage, new Vector2(960, 832), Color.White);

        foreach (var scene in _battleCity.construtor.ConstructorScene) scene.Draw(spriteBatch);
    }

    private void DrawObject(SpriteBatch spriteBatch, TypeOfObject type, Vector2 position)
    {
        var texture = _battleCity._sceneDictionary[type];
        spriteBatch.Draw(texture, position, Color.White);
    }

    private void DrawEnemyAndPlayerPositions(SpriteBatch spriteBatch)
    {
        foreach (var pos in _battleCity.construtor.CoordinateForEnemy)
            spriteBatch.Draw(_battleCity._enemyImage, pos, Color.White);

        foreach (var pos in _battleCity.construtor.CoordinateForPlayer)
            spriteBatch.Draw(_battleCity._playerImage, pos, Color.White);
    }
}