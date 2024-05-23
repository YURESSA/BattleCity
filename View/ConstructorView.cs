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

        spriteBatch.Draw(_battleCity.EnemyImage, new Vector2(960, 704), Color.White);
        spriteBatch.Draw(_battleCity.PlayerImage, new Vector2(960, 832), Color.White);

        foreach (var scene in _battleCity.Constructor.ConstructorScene) 
            scene.SceneView.Draw(_battleCity.SpriteBatch, scene.SceneModel);
    }

    private void DrawObject(SpriteBatch spriteBatch, TypeOfObject type, Vector2 position)
    {
        var texture = _battleCity.SceneDictionary[type];
        spriteBatch.Draw(texture, position, Color.White);
    }

    private void DrawEnemyAndPlayerPositions(SpriteBatch spriteBatch)
    {
        foreach (var pos in _battleCity.Constructor.CoordinateForEnemy)
            spriteBatch.Draw(_battleCity.EnemyImage, pos, Color.White);

        foreach (var pos in _battleCity.Constructor.CoordinateForPlayer)
            spriteBatch.Draw(_battleCity.PlayerImage, pos, Color.White);
    }
}