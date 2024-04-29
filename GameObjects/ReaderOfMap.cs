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

    private static int CellSize = 64;

    public static HashSet<ScenicObject> Reader(Dictionary<TypeOfObject, Texture2D> sprite, int cellSize)
    {
        CellSize = cellSize;
        using var sr =
            new StreamReader(
                "C:\\Users\\goshr\\OneDrive\\Документы\\Универ\\Программирование\\Game\\BattleCity\\BattleCity\\input.txt");
        const int size = 15;
        var map = new HashSet<ScenicObject>();

        for (var i = 0; i < size; i++)
        {
            var mapLine = sr.ReadLine();
            for (var j = 0; j < size; j++)
            {
                var type = Codes[mapLine[j]];
                var x = j * sprite[type].Width;
                var y = i * sprite[type].Height;
                var scene = new ScenicObject(new Vector2(x, y), type, sprite[type], CellSize);
                map.Add(scene);
            }
        }

        return map;
    }
}