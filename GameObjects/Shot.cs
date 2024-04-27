using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class Shot
{
    public static Texture2D SpriteOfBullet { get; set; }
    private Vector2 origin;
    private Vector2 Position;
    private int CellSize;
    private float Speed;
    public float Angle;
    
    public Shot(Vector2 position, float speed, int cellSize, float angle)
    {
        Position = position + new Vector2(0, 1);
        CellSize = cellSize;
        Angle = angle;
        Speed = speed;
        origin = new Vector2(32 / 2f, 40 / 2f);
    }
    public void Draw(SpriteBatch spriteBatch)
    {
        var sourceRect = new Rectangle(0, 0, CellSize, CellSize);
        spriteBatch.Draw(SpriteOfBullet, Position, sourceRect, Color.White,  Angle, origin, 1f, SpriteEffects.None, 0f);
    }
    
    public void Update(GameTime gameTime)
    {
        Vector2 direction = new Vector2();      

        //если танк направлен ВЛЕВО
        if (Angle == (float)MathHelper.PiOver2)
            direction += new Vector2(Speed, 0);             
        


        //если танк направлен ВВЕРХ
        if (Angle == (float)MathHelper.Pi)
            direction += new Vector2(0, Speed);
        
            

        //если танк направлен ВПРАВО
        if (Angle == -(float)MathHelper.PiOver2)
            direction += new Vector2(-Speed, 0);
        


        //если танк направлен ВНИЗ
        if (Angle == (float)MathHelper.TwoPi)
            direction += new Vector2(0, -Speed);
        
            
            
        if (direction.Length() > 0f)
            Position += direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
    }    
}