using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class PlayerModel : Tank
{
    private const int CellSize = 64;

    public PlayerModel(float speed, Vector2 position, Texture2D sprite,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp, float bulletSpeed) :
        base(speed, position, sprite, hasCollision, bulletObjects, isAlive, hp, bulletSpeed)
    {
    }

    public void Update(GameTime gameTime)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;
        HasCollision(this);
        if (!(Direction.Length() > 0f)) return;
        Position += Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (HasCollision(this)) Position -= Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    public override Vector2 GetCoordinate()
    {
        var center = Position + Origin;
        var x = (int)(center.X / CellSize);
        var y = (int)(center.Y / CellSize);

        return new Vector2(x, y);
    }

    public void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero)
            Shoot();
    }
}