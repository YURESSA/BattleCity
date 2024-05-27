using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class EnemyController
{
    public EnemyModel EnemyModel { get; }
    private EnemyView EnemyView { get; }

    private EnemyController(EnemyModel enemyModel, EnemyView enemyView)
    {
        EnemyModel = enemyModel;
        EnemyView = enemyView;
    }

    public void Update(GameTime gameTime, SceneController[,] map, List<Vector2> userCoordinates,
        Vector2 coordinateOfStaff)
    {
        EnemyModel.Update(gameTime, map, userCoordinates, coordinateOfStaff);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        EnemyView.Draw(spriteBatch, EnemyModel);
    }

    public static EnemyController GetEnemy(float speed, Vector2 position, Texture2D texture,
        Func<MovedObject, bool> hasCollision,
        HashSet<Shot> bulletObjects, bool isAlive, int hp, float bulletSpeed)
    {
        var enemyModel = new EnemyModel(speed, position, texture, hasCollision, bulletObjects, isAlive, hp, bulletSpeed);
        var enemyView = new EnemyView(texture);
        var enemyController = new EnemyController(enemyModel, enemyView);
        return enemyController;
    }
}