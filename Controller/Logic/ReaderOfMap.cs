using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

internal static class ReaderOfMap
{
    private static readonly Dictionary<char, TypeOfObject> Codes = new()
    {
        { '0', TypeOfObject.None },
        { '1', TypeOfObject.Bricks },
        { '2', TypeOfObject.Concrete },
        { '3', TypeOfObject.Leaves },
        { '4', TypeOfObject.Water },
        { '5', TypeOfObject.Wall },
        { '9', TypeOfObject.Staff }
    };

    private static List<Vector2> _coordinateForEnemy;
    private static List<Vector2> _coordinateForPlayers;
    private static Vector2 _coordinateOfStaff;

    public static List<Vector2> GetEnemyCoordinate()
    {
        return _coordinateForEnemy;
    }

    public static Vector2 GetCoordinateOfStaff()
    {
        return _coordinateOfStaff;
    }

    public static List<Vector2> GetPlayerCoordinate()
    {
        return _coordinateForPlayers;
    }

    public static SceneController[,] MapReader(Dictionary<TypeOfObject, Texture2D> sprite, int cellSize,
        string fileName)
    {
        _coordinateForEnemy = new List<Vector2>();
        _coordinateForPlayers = new List<Vector2>();
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var path = string.Concat(appDirectory.AsSpan(0,
                appDirectory.IndexOf("\\bin", StringComparison.Ordinal)), $"\\{fileName}");
        var file = new StreamReader(path).ReadToEnd();
        var lines = file.Split("\r\n");
        var height = lines.Length;
        var width = lines[0].Length;
        var map = new SceneController[height, width];
        for (var i = 0; i < height; i++)
        {
            var mapLine = lines[i];
            ProcessMapLine(sprite, width, mapLine, i, map);
        }

        return map;
    }


    private static void ProcessMapLine(Dictionary<TypeOfObject, Texture2D> sprite, int width,
        string mapLine, int i, SceneController[,] map)
    {
        for (var j = 0; j < width; j++)
            if (mapLine[j] != 'E' && mapLine[j] != 'P')
            {
                var type = Codes[mapLine[j]];

                var x = j * sprite[type].Width;
                var y = i * sprite[type].Height;
                if (type == TypeOfObject.Staff)
                    _coordinateOfStaff = new Vector2(x, y);
                var scene = SceneController.GetScene(new Vector2(x, y), type, sprite[type],
                    sprite[TypeOfObject.None], true, sprite);
                map[i, j] = scene;
            }
            else
            {
                var x = j * sprite[TypeOfObject.None].Width;
                var y = i * sprite[TypeOfObject.None].Height;
                var scene = SceneController.GetScene(new Vector2(x, y), TypeOfObject.None, sprite[TypeOfObject.None],
                    sprite[TypeOfObject.None], true, sprite);
                map[i, j] = scene;
                var offset = 4;
                switch (mapLine[j])
                {
                    case 'E':
                        _coordinateForEnemy.Add(new Vector2(x + offset, y + offset));
                        break;
                    case 'P':
                        _coordinateForPlayers.Add(new Vector2(x + offset, y + offset));
                        break;
                }
            }
    }
}