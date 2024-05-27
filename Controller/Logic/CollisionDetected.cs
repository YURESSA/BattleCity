using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace BattleCity;

public class CollisionDetected
{
    private readonly BattleCity _battleCity;

    public CollisionDetected(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }


    private void AddBang(Vector2 position, bool isAlive)
    {
        var bangModel = BangController.GetBang(position, isAlive);
        _battleCity.BangModels.Add(bangModel);
    }

    public bool HasCollision(MovedObject obj)
    {
        if (CheckSceneObjectCollisions(obj)) return true;
        if (CheckBulletCollisions(obj)) return true;
        if (TanksCollision(obj)) return true;

        return false;
    }

    private bool TanksCollision(MovedObject obj)
    {
        if (obj is not Tank tank) return false;
        var hasCollision = false;

        foreach (var enemyTank in _battleCity.EnemyTanks
                     .Where(enemyTank => tank != enemyTank.EnemyModel && tank.Intersect(enemyTank.EnemyModel)))
        {
            ResolveTankCollision(tank, enemyTank.EnemyModel);
            hasCollision = true;
        }

        foreach (var playersTank in _battleCity.PlayersTanks
                     .Where(playersTank => tank != playersTank && tank.Intersect(playersTank)))
        {
            ResolveTankCollision(tank, playersTank);
            hasCollision = true;
        }

        return hasCollision;
    }

    private static void ResolveTankCollision(Tank tank1, Tank tank2)
    {
        var deltaX = tank1.Position.X - tank2.Position.X;
        var deltaY = tank1.Position.Y - tank2.Position.Y;

        var overlapX = tank1.Width / 2 + tank2.Width / 2 - Math.Abs(deltaX);
        var overlapY = tank1.Height / 2 + tank2.Height / 2 - Math.Abs(deltaY);
        if (overlapX < overlapY)
        {
            if (deltaX < 0)
                tank1.Position -= new Vector2(overlapX / 2, 0);
            else
                tank1.Position += new Vector2(overlapX / 2, 0);
        }
        else
        {
            if (deltaY < 0)
                tank1.Position -= new Vector2(0, overlapY / 2);
            else
                tank1.Position += new Vector2(0, overlapY / 2);
        }
    }

    private bool CheckSceneObjectCollisions(MovedObject obj)
    {
        foreach (var scene in _battleCity.SceneObjects)
            if (scene.SceneModel.Intersect(obj) && scene.SceneModel.Type != TypeOfObject.None
                                                && scene.SceneModel.Type != TypeOfObject.Leaves)
            {
                HandleSceneCollision(scene.SceneModel, obj);
                return true;
            }

        return false;
    }

    private void HandleSceneCollision(SceneModel sceneModel, MovedObject obj)
    {
        if ((sceneModel.Type == TypeOfObject.Bricks || sceneModel.Type == TypeOfObject.Staff) && obj is ShotModel)
        {
            if (sceneModel.Type == TypeOfObject.Staff)
            {
                _battleCity.State = StateOfGame.DefeatLevel;
                MusicController.PauseLevelMusic();
                MusicController.PauseStartMusic();
                MusicController.PlayEndMusic();
            }

            sceneModel.IsAlive = false;
            AddBang(obj.Position, true);
            MusicController.PlayDestroy();
            obj.Kill();
        }
        else if (sceneModel.Type != TypeOfObject.Bricks && sceneModel.Type != TypeOfObject.Water && obj is ShotModel)
        {
            AddBang(obj.Position, true);
            MusicController.PlayDestroy();
            obj.Kill();
        }
    }

    private bool CheckBulletCollisions(MovedObject obj)
    {
        foreach (var bullet in _battleCity.BulletObjects)
        {
            if (CheckBulletCollisionWithEnemy(obj, bullet)) return true;
            if (CheckBulletCollisionWithPlayer(obj, bullet)) return true;
            if (CheckBulletCollisionWithShot(obj, bullet)) return true;
        }

        return false;
    }

    private bool CheckBulletCollisionWithEnemy(MovedObject obj, Shot bullet)
    {
        if (obj is not EnemyModel || bullet.ShotModel.Parent is not PlayerModel ||
            !obj.Intersect(bullet.ShotModel)) return false;
        bullet.ShotModel.Kill();
        obj.Kill();
        AddBang(obj.Position + obj.Origin, true);
        MusicController.PlayDestroy();
        return true;
    }

    private bool CheckBulletCollisionWithPlayer(MovedObject obj, Shot bullet)
    {
        if (obj is not PlayerModel || bullet.ShotModel.Parent is not EnemyModel ||
            !obj.Intersect(bullet.ShotModel)) return false;
        bullet.ShotModel.Kill();
        obj.Kill();
        AddBang(obj.Position + obj.Origin, true);
        MusicController.PlayDestroy();
        return true;
    }

    private bool CheckBulletCollisionWithShot(MovedObject obj, Shot bullet)
    {
        if (obj is not ShotModel || !obj.Intersect(bullet.ShotModel) || obj == bullet.ShotModel) return false;
        bullet.ShotModel.Kill();
        obj.Kill();
        AddBang(obj.Position + obj.Origin, true);
        MusicController.PlayDestroy();
        return true;
    }
}