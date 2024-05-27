using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class EnemyModel : Tank
{
    public EnemyModel(float speed, Vector2 position, Texture2D sprite,
        Func<MovedObject, bool> hasCollision, HashSet<Shot> bulletObjects, bool isAlive, int hp, float bulletSpeed)
        : base(speed, position, sprite, hasCollision, bulletObjects, isAlive, hp, bulletSpeed)
    {
    }

    public void Update(GameTime gameTime, SceneController[,] map, List<Vector2> coordinates, Vector2 coordinateOfStaff)
    {
        ElapsedTime -= gameTime.ElapsedGameTime;
        var paths = GetPathsToCoordinates(map, coordinates, coordinateOfStaff);

        var shortestPath = paths.MinBy(path => path.Count);
        if (shortestPath.Count >= 2)
        {
            ConvertToPath(shortestPath);
            TryShoot(new Vector2(shortestPath[^1].X, shortestPath[^1].Y), GetCoordinate());
        }

        UpdatePosition(gameTime);
    }

    private IEnumerable<List<Point>> GetPathsToCoordinates(SceneController[,] map, List<Vector2> coordinates,
        Vector2 coordinateOfStaff)
    {
        var paths = new List<List<Point>>();
        var labyrinth = CreateMap(map);

        foreach (var coordinate in coordinates)
            paths.Add(FindPath(labyrinth, GetCoordinate(), coordinate));

        var staffCoordinate = new Vector2(coordinateOfStaff.X / 64, coordinateOfStaff.Y / 64);
        labyrinth = CreateStaffMap(map, staffCoordinate);

        var pathToStaff = FindPath(labyrinth, GetCoordinate(), staffCoordinate);
        if (pathToStaff.Count > 0)
            paths.Add(pathToStaff);

        return paths;
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
        else
            HandleShooting();
    }

    private void ConvertToPath(List<Point> path)
    {
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

    private List<Point> FindPath(State[,] map, Vector2 startPosition, Vector2 targetPosition)
    {
        var start = new Point((int)startPosition.X, (int)startPosition.Y);
        var target = new Point((int)targetPosition.X, (int)targetPosition.Y);

        if (!IsPointValid(start, map) || !IsPointValid(target, map))
            return new List<Point>();

        var openSet = new PriorityQueue<Point, float>();
        var cameFrom = new Dictionary<Point, Point>();
        var gScore = new Dictionary<Point, float> { [start] = 0 };
        var fScore = new Dictionary<Point, float> { [start] = Heuristic(start, target) };

        openSet.Enqueue(start, fScore[start]);

        while (openSet.Count > 0)
        {
            var current = openSet.Dequeue();

            if (current == target)
                return ReconstructPath(cameFrom, current);

            foreach (var neighbor in GetNeighbors(current, map))
            {
                var tentativeGScore = gScore[current] + 1;

                if (gScore.ContainsKey(neighbor) && !(tentativeGScore < gScore[neighbor])) continue;
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeGScore;
                fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, target);

                if (openSet.UnorderedItems.All(item => item.Element != neighbor))
                    openSet.Enqueue(neighbor, fScore[neighbor]);
            }
        }

        return new List<Point>();
    }

    private static float Heuristic(Point a, Point b)
    {
        return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
    {
        var path = new List<Point> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Insert(0, current);
        }

        return path;
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

    private static bool IsPointValid(Point point, State[,] map)
    {
        return point.X >= 0 && point.X < map.GetLength(0) &&
               point.Y >= 0 && point.Y < map.GetLength(1) &&
               map[point.X, point.Y] == State.Empty;
    }


    private void HandleShooting()
    {
        if (ElapsedTime <= TimeSpan.Zero)
            Shoot();
    }

    private static State[,] CreateMap(SceneController[,] labyrinth)
    {
        var map = new State[labyrinth.GetLength(0), labyrinth.GetLength(1)];

        for (var x = 0; x < map.GetLength(0); x++)
        for (var y = 0; y < map.GetLength(1); y++)
            map[x, y] = labyrinth[y, x].SceneModel.Type == TypeOfObject.None ||
                        labyrinth[y, x].SceneModel.Type == TypeOfObject.Leaves
                ? State.Empty
                : State.Wall;
        return map;
    }

    private static State[,] CreateStaffMap(SceneController[,] labyrinth, Vector2 staffCoordinate)
    {
        var map = CreateMap(labyrinth);

        for (var i = -1; i < 2; i++)
        for (var j = -1; j < 2; j++)
        {
            var point = staffCoordinate + new Vector2(i, j);
            if (point.X >= 0 && point.X < labyrinth.GetLength(0) &&
                point.Y >= 0 && point.Y < labyrinth.GetLength(1))
                map[(int)point.X, (int)point.Y] = State.Empty;
        }

        return map;
    }
}