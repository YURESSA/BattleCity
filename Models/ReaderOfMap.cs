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
    private static  List<Vector2> _coordinateForPlayers;

    public static List<Vector2> GetEnemyCoordinate() => _coordinateForEnemy;
    public static List<Vector2> GetPlayerCoordinate() => _coordinateForPlayers;
    public static Scene[,] MapReader(Dictionary<TypeOfObject, Texture2D> sprite, int cellSize, string fileName)
    {
        _coordinateForEnemy = new();
        _coordinateForPlayers = new();
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var path = string.Concat(appDirectory.AsSpan(0, appDirectory.IndexOf("\\bin", StringComparison.Ordinal)), $"\\{fileName}");
        var file = new StreamReader(path).ReadToEnd();
        var lines = file.Split("\r\n");
        var height = lines.Length;
        var width = lines[0].Length;
        var map = new Scene[height, width];
        for (var i = 0; i < height; i++)
        {
            var mapLine = lines[i];
            ProcessMapLine(sprite, cellSize, width, mapLine, i, map);
        }

        return map;
    }

    private static void ProcessMapLine(Dictionary<TypeOfObject, Texture2D> sprite, int cellSize, int width, string mapLine, int i, Scene[,] map)
    {
        for (var j = 0; j < width; j++)
        {
            if (mapLine[j] != 'E' && mapLine[j] != 'P')
            {
                var type = Codes[mapLine[j]];
                var x = j * sprite[type].Width;
                var y = i * sprite[type].Height;
                var scene = new Scene(new Vector2(x, y), type, sprite[type],
                    sprite[TypeOfObject.None], cellSize, true);
                map[i, j] = scene;
            }
            else
            {
                var x = j * sprite[TypeOfObject.None].Width;
                var y = i * sprite[TypeOfObject.None].Height;
                var scene = new Scene(new Vector2(x, y), TypeOfObject.None, sprite[TypeOfObject.None],
                    sprite[TypeOfObject.None], cellSize, true);
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
}