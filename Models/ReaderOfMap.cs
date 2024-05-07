using System;
using System.Collections.Generic;
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


    public static Scene[,] Reader(Dictionary<TypeOfObject, Texture2D> sprite, int cellSize, string fileName)
    {
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var path = string.Concat(appDirectory.AsSpan(0, appDirectory.IndexOf("\\bin")), $"\\{fileName}");

        var file = new StreamReader(path).ReadToEnd();
        var lines = file.Split("\r\n");
        var height = lines.Length;
        var width = lines[0].Length;
        var map = new Scene[height, width];
        for (var i = 0; i < height; i++)
        {
            var mapLine = lines[i];
            for (var j = 0; j < width; j++)
            {
                var type = Codes[mapLine[j]];
                var x = j * sprite[type].Width;
                var y = i * sprite[type].Height;
                var scene = new Scene(new Vector2(x, y), type, sprite[type], sprite[TypeOfObject.None], cellSize, true);
                map[i, j] = scene;
            }
        }

        return map;
    }
}