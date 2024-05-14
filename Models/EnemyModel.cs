using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public enum State
{
    Empty,
    Wall,
    Visited
}
public class EnemyModel : Tank
{
    public EnemyModel(float speed, Vector2 position, Texture2D sprite, int cellSize,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp)
        : base(speed, position, sprite, cellSize, hasCollision, bulletObjects, isAlive, hp)
    {
    }

    public void Update(GameTime gameTime, Scene[,] map, Vector2 coordinate)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;
        var path = FindPath(map, GetCoordinate(), coordinate);
        MoveAlongPath(path);
        TryShoot(coordinate, GetCoordinate());
        UpdatePosition(gameTime);
    }

    private void TryShoot(Vector2 enemy, Vector2 player)
    {
        if (Math.Abs(enemy.X - player.X) < 0.0001 || Math.Abs(enemy.Y - player.Y) < 0.0001)
            HandleShooting();
    }

    private void UpdatePosition(GameTime gameTime)
    {
        if (Direction == Vector2.Zero) return;
        var pathLength = Direction * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        if (!HasCollision(this))
            Position += pathLength;
    }

    private void MoveAlongPath(List<Point> path)
    {
        if (path.Count < 2) return;
        var difference = new Point(path[0].X - path[1].X, path[0].Y - path[1].Y);
        switch (difference)
        {
            case { X: 1, Y: 0 }:
                MoveLeft();
                break;
            case { X: -1, Y: 0 }:
                MoveRight();
                break;
            case { X: 0, Y: 1 }:
                MoveUp();
                break;
            case { X: 0, Y: -1 }:
                MoveDown();
                break;
        }
    }

    private List<Point> FindPath(Scene[,] labyrinth, Vector2 startPosition, Vector2 targetPosition)
    {
        var start = new Point((int)startPosition.X, (int)startPosition.Y);
        var target = new Point((int)targetPosition.X, (int)targetPosition.Y);

        var map = CreateMap(labyrinth);

        if (!IsPointValid(start, map) || !IsPointValid(target, map))
            return new List<Point>();

        var openSet = new Queue<Point>();
        var closedSet = new HashSet<Point>();
        var cameFrom = new Dictionary<Point, Point>();
        openSet.Enqueue(start);

        while (openSet.Any())
        {
            var current = openSet.Dequeue();

            if (current == target)
            {
                var path = new List<Point> { current };
                while (cameFrom.ContainsKey(current))
                {
                    current = cameFrom[current];
                    path.Insert(0, current);
                }
                return path;
            }

            closedSet.Add(current);

            foreach (var neighbor in GetNeighbors(current, map))
            {
                if (closedSet.Contains(neighbor)) continue;

                if (!cameFrom.ContainsKey(neighbor))
                {
                    openSet.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return new List<Point>();
    }

    private IEnumerable<Point> GetNeighbors(Point point, State[,] map)
    {
        var directions = new[] { new Point(0, -1), new Point(0, 1), new Point(-1, 0), new Point(1, 0) };
        foreach (var dir in directions)
        {
            var next = new Point(point.X + dir.X, point.Y + dir.Y);
            if (IsPointValid(next, map))
                yield return next;
        }
    }

    private bool IsPointValid(Point point, State[,] labyrinth)
    {
        return point.X >= 0 && point.X < labyrinth.GetLength(0) &&
               point.Y >= 0 && point.Y < labyrinth.GetLength(1) &&
               labyrinth[point.X, point.Y] == State.Empty;
    }

    private void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero)
            Shoot();
    }

    private static State[,] CreateMap(Scene[,] labyrinth)
    {
        var map = new State[labyrinth.GetLength(0), labyrinth.GetLength(1)];

        for (var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            map[x, y] = labyrinth[y, x].SceneModel.Type == TypeOfObject.None ? State.Empty : State.Wall;

        return map;
    }
}