using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public enum State
{
    Empty,
    Wall,
    Visited
}

public class Point
{
    public int X { get; init; }
    public int Y { get; init; }
    public Point PreviousPoint { get; init; }

    public string GetKey()
    {
        return $"{X}_{Y}";
    }
}

public class EnemyModel : Tank
{
    public EnemyModel(float speed, Vector2 position, Texture2D sprite, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp) : base(speed, position, sprite, cellSize,
        hasCollision, bulletObjects, isAlive, hp)
    {
    }

    public void Update(GameTime gameTime, Scene[,] map, Vector2 coordinate)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;
        var path = FindPath(map, coordinate);
        if (path.Count < 2) return;
        MoveAlongPath(path, coordinate);
        UpdatePosition(gameTime);
    }

    private void MoveAlongPath(List<Point> path, Vector2 coordinate)
    {
        var firstPoint = path[^1];
        var secondPoint = path[^2];
        var difference = new Point { X = firstPoint.X - secondPoint.X, Y = firstPoint.Y - secondPoint.Y };

        switch (difference.X)
        {
            case 0 when difference.Y == -1:
                MoveDown();
                break;
            case 0 when difference.Y == 1:
                MoveUp();
                break;
            case -1 when difference.Y == 0:
                MoveRight();
                break;
            case 1 when difference.Y == 0:
                MoveLeft();
                break;
        }
        if (coordinate.X == firstPoint.X || coordinate.Y == firstPoint.Y)
            HandleShooting();
    }

    private void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero)
            Shoot();
    }

    private void UpdatePosition(GameTime gameTime)
    {
        if (!(Direction.Length() > 0f)) return;
        Position += Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (HasCollision(this)) Position -= Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
    }

    private List<Point> FindPath(Scene[,] labyrinth, Vector2 resultCoordinate)
    {
        var path = new List<Point>();
        var map = CreateMap(labyrinth);
        var pointDictionary = new Dictionary<string, Point>();

        var startCoordinate = GetCoordinate();
        var startPoint = new Point { X = (int)startCoordinate.X, Y = (int)startCoordinate.Y };
        pointDictionary.Add($"{startPoint.X}_{startPoint.Y}", startPoint);
        var queue = new Queue<Point>();
        queue.Enqueue(startPoint);

        while (queue.Count != 0)
        {
            var point = queue.Dequeue();
            if (!IsPointValid(map, point)) continue;

            map[point.X, point.Y] = State.Visited;

            foreach (var neighbor in GetNeighbors(map, point))
            {
                var key = neighbor.GetKey();
                if (pointDictionary.ContainsKey(key)) continue;
                pointDictionary.Add(key, neighbor);
                queue.Enqueue(neighbor);
            }
        }

        if (!pointDictionary.ContainsKey($"{resultCoordinate.X}_{resultCoordinate.Y}")) return path;
        var p = pointDictionary[$"{resultCoordinate.X}_{resultCoordinate.Y}"];
        while (p != null)
        {
            path.Add(p);
            p = p.PreviousPoint;
        }

        return path;
    }

    private static State[,] CreateMap(Scene[,] labyrinth)
    {
        var map = new State[labyrinth.GetLength(0), labyrinth.GetLength(1)];

        for (var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            map[x, y] = labyrinth[y, x].SceneModel.Type == TypeOfObject.None ? State.Empty : State.Wall;

        return map;
    }

    private static bool IsPointValid(State[,] map, Point point)
    {
        return point.X >= 0 && point.X < map.GetLength(0) && point.Y >= 0 && point.Y < map.GetLength(1) &&
               map[point.X, point.Y] == State.Empty;
    }

    private static IEnumerable<Point> GetNeighbors(State[,] map, Point point)
    {
        var directions = new[]
        {
            new Point { X = 0, Y = -1 }, new Point { X = 0, Y = 1 }, new Point { X = -1, Y = 0 },
            new Point { X = 1, Y = 0 }
        };
        foreach (var dir in directions)
        {
            var nextX = point.X + dir.X;
            var nextY = point.Y + dir.Y;
            if (IsPointValid(map, new Point { X = nextX, Y = nextY }))
                yield return new Point { X = nextX, Y = nextY, PreviousPoint = point };
        }
    }
}