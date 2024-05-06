using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;
enum State
{
    Empty,
    Wall,
    Visited
};

public class Point
{
    public int X { get; set; }
    public int Y { get; set; }
    public Point PreviousPoint { get; set; }

    public string GetKey()
    {
        return $"{X}_{Y}";
    }
}
public class EnemyTank:Tank
{
    public EnemyTank(float speed, Vector2 position, Texture2D sprite, int cellSize, Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects) : base(speed, position, sprite, cellSize, hasCollision, bulletObjects)
    {

    }

    public void Update(GameTime gameTime ,ScenicObject[,] map, Vector2 coordinate)
    {
        elapsedTime -= gameTime.ElapsedGameTime;
        var path = FindPath(map, coordinate);
        if (path.Count < 2) return;
        var firstPoint = path[path.Count - 1];
        var secondPoint = path[path.Count-2];
        var difference = new Point() { X = firstPoint.X - secondPoint.X, Y = firstPoint.Y - secondPoint.Y };
        
        if (difference.X == 0 && difference.Y == -1)
            MoveBack();
        if(difference.X == 0 && difference.Y == 1)
            MoveFront();
        if(difference.X == -1 && difference.Y == 0)
            MoveRight();
        if(difference.X == 1 && difference.Y == 0)
            MoveLeft();
        if (coordinate.X == firstPoint.X || coordinate.Y == firstPoint.Y)
            HandleShooting(gameTime);
        if (Direction.Length() > 0f)
        {
            Position += Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (HasCollision(this)) Position -= Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

    }
    private void HandleShooting(GameTime gameTime)
    {
        if (elapsedTime <= TimeSpan.Zero)
            Shoot();
    }

    public List<Point> FindPath(ScenicObject[,] labyrinth, Vector2 resultCoordinate)
    {
        var path = new List<Point>();
        var map = new State[labyrinth.GetLength(0), labyrinth.GetLength(1)];
        var pointDictionary = new Dictionary<string, Point>(); // Словарь для хранения точек по координатам

        for (var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            if (labyrinth[y, x].Type == TypeOfObject.None)
                map[x, y] = State.Empty;
            else
                map[x, y] = State.Wall;
        var startCoordinate = this.GetCoordinate();
        var startPoint = new Point(){X = (int)startCoordinate.X, Y = (int)startCoordinate.Y};
        pointDictionary.Add($"{startPoint.X}_{startPoint.Y}", startPoint);
        var queue = new Queue<Point>();
        queue.Enqueue(startPoint);

        while (queue.Count != 0)
        {
            var point = queue.Dequeue();
            if (point.X < 0 || point.X >= map.GetLength(0) || point.Y < 0 || point.Y >= map.GetLength(1)) continue;
            if (map[point.X, point.Y] != State.Empty) continue;
            map[point.X, point.Y] = State.Visited;

            for (var dy = -1; dy <= 1; dy++)
            for (var dx = -1; dx <= 1; dx++)
            {
                if (dx != 0 && dy != 0) continue;
                var nextX = point.X + dx;
                var nextY = point.Y + dy;
                if (nextX < 0 || nextX >= map.GetLength(0) || nextY < 0 || nextY >= map.GetLength(1)) continue;
                if (map[nextX, nextY] == State.Empty)
                {
                    var nextPoint = new Point
                        { X = nextX, Y = nextY, PreviousPoint = point }; // Обновляем ссылку на предыдущую точку
                    queue.Enqueue(nextPoint);
                    var key = nextPoint.GetKey();
                    if (!pointDictionary.ContainsKey(key))
                        pointDictionary.Add(key, nextPoint);
                }
            }
        }
        if (pointDictionary.ContainsKey($"{resultCoordinate.X}_{resultCoordinate.Y}"))
        {
            var p = pointDictionary[$"{resultCoordinate.X}_{resultCoordinate.Y}"];
            while (p != null)
            {
                path.Add(p);
                p = p.PreviousPoint;
            }
            return path;
        }

        return path;
    }
    
    public override Vector2 GetCoordinate()
    {
        if (Angle == MathHelper.Pi)
            return new Vector2((int)Position.X / Size, (int)(Position.Y) / Size);
        if (Angle == MathHelper.TwoPi || Angle == -MathHelper.PiOver2)
            return new Vector2((int)(Position.X + 52) / Size, (int)(Position.Y + 52) / Size);
        return new Vector2((int)Position.X / Size, (int)(Position.Y) / Size);
    }
}