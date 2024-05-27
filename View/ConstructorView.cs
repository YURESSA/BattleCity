using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class ConstructorView
{
    private readonly BattleCity _battleCity;
    private Texture2D _rectangleBlock;
    public static SpriteFont TextBlock;

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
        DrawCurrentPosition(spriteBatch);
        DrawElementsToConstruct(spriteBatch);
        DrawEnemyAndPlayerPositions(spriteBatch);
        DrawException(spriteBatch);
    }

    private void DrawException(SpriteBatch spriteBatch)
    {
        if (_battleCity.Constructor.CoordinateForPlayer.Count <= 0
            || _battleCity.Constructor.CoordinateForEnemy.Count <= 0)
            spriteBatch.DrawString(TextBlock, "Не назначены точки появлений", new Vector2(64, 890),
                Color.Black);
    }

    private void DrawCurrentPosition(SpriteBatch spriteBatch)
    {
        var position = new Point(956, _battleCity.Constructor.CurrentId * 64 - 4);
        var size = new Point(72, 72);
        var rectangle = new Rectangle(position, size);
        spriteBatch.Draw(_rectangleBlock, rectangle, Color.Lime);

        size = new Point(64, 64);
        position = new Point(960, _battleCity.Constructor.CurrentId * 64);
        rectangle = new Rectangle(position, size);
        spriteBatch.Draw(_rectangleBlock, rectangle, new Color(128, 128, 128));
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
        foreach (var scene in _battleCity.Constructor.ConstructorScene)
            scene.SceneView.Draw(spriteBatch, scene.SceneModel);
    }

    private void DrawElementsToConstruct(SpriteBatch spriteBatch)
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

        spriteBatch.Draw(_battleCity.TanksImage["playerLevel1"], new Vector2(966, 710), Color.White);
        spriteBatch.Draw(_battleCity.TanksImage["enemyLevel1"], new Vector2(966, 838), Color.White);
    }

    private void DrawObject(SpriteBatch spriteBatch, TypeOfObject type, Vector2 position)
    {
        var texture = _battleCity.SceneDictionary[type];
        spriteBatch.Draw(texture, position, Color.White);
    }

    private void DrawEnemyAndPlayerPositions(SpriteBatch spriteBatch)
    {
        foreach (var pos in _battleCity.Constructor.CoordinateForEnemy)
            spriteBatch.Draw(_battleCity.TanksImage["enemyLevel1"], pos, Color.White);

        foreach (var pos in _battleCity.Constructor.CoordinateForPlayer)
            spriteBatch.Draw(_battleCity.TanksImage["playerLevel1"], pos, Color.White);
    }
}