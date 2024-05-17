using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class PlayerModel : Tank
{
    public PlayerModel(float speed, Vector2 position, Texture2D sprite, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp) :
        base(speed, position, sprite, cellSize, hasCollision, bulletObjects, isAlive, hp)
    {
    }

    public void Update(GameTime gameTime)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;
        HasCollision(this);
        if (Direction.Length() > 0f)
        {
            Position += Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) Position -= Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }
    }

    public override Vector2 GetCoordinate()
    {
        var cellSize = 64;
        var center = Position + Origin;
        var x = (int)(center.X / cellSize);
        var y = (int)(center.Y / cellSize);

        return new Vector2(x, y);
    }

    public void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero)
            Shoot();
    }
}