namespace BattleCity;

public class CollisionDetected
{
    private readonly BattleCity _battleCity;

    public CollisionDetected(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }

    public bool HasCollision(MovedObject obj)
    {
        if (CheckSceneObjectCollisions(obj)) return true;
        if (CheckBulletCollisions(obj)) return true;

        return false;
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

    private void HandleSceneCollision(SceneObjectsModel sceneModel, MovedObject obj)
    {
        if ((sceneModel.Type == TypeOfObject.Bricks || sceneModel.Type == TypeOfObject.Staff) && obj is ShotModel)
        {
            if (sceneModel.Type == TypeOfObject.Staff) _battleCity.State = StateOfGame.DefeatLevel;

            sceneModel.IsAlive = false;
            obj.Kill();
        }
        else if (sceneModel.Type != TypeOfObject.Bricks && sceneModel.Type != TypeOfObject.Water && obj is ShotModel)
        {
            obj.Kill();
        }
    }

    private bool CheckBulletCollisions(MovedObject obj)
    {
        foreach (var bullet in _battleCity.BulletObjects)
        {
            if (CheckBulletCollisionWithEnemy(obj, bullet)) break;
            if (CheckBulletCollisionWithPlayer(obj, bullet)) break;
            if (CheckBulletCollisionWithShot(obj, bullet)) break;
        }

        return false;
    }

    private bool CheckBulletCollisionWithEnemy(MovedObject obj, Shot bullet)
    {
        if (obj is EnemyModel && bullet.ShotModel.Parent is PlayerModel && obj.Intersect(bullet.ShotModel))
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }

    private bool CheckBulletCollisionWithPlayer(MovedObject obj, Shot bullet)
    {
        if (obj is PlayerModel && bullet.ShotModel.Parent is EnemyModel && obj.Intersect(bullet.ShotModel))
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }

    private bool CheckBulletCollisionWithShot(MovedObject obj, Shot bullet)
    {
        if (obj is ShotModel && obj.Intersect(bullet.ShotModel) && obj != bullet.ShotModel)
        {
            bullet.ShotModel.Kill();
            obj.Kill();
            return true;
        }

        return false;
    }
}