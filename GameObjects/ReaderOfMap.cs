using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

static class ReaderOfMap
{
    private static readonly Dictionary<char, TypeOfObject> Codes = new()
    {
        { '0', TypeOfObject.None },
        { '1', TypeOfObject.Bricks },
        { '2', TypeOfObject.Concrete },
        { '3', TypeOfObject.Leaves },
        { '4', TypeOfObject.Water },
        { '9', TypeOfObject.Staff }
    };
    static int CellSize = 64;
    public static List<ScenicObject> Reader(Dictionary<TypeOfObject, Texture2D> image, int cellSize)
    {
        const int borderSize = 64;
        CellSize = cellSize;
        using var sr = new StreamReader("C:\\Users\\goshr\\OneDrive\\Документы\\Универ\\Программирование\\Game\\BattleCity\\BattleCity\\input.txt");
        const int size = 13;
        var map = new List<ScenicObject>();
     
        for (var i = 0; i < size; i++)
        {
            var mapLine = sr.ReadLine();
            for (var j = 0; j < size; j++)
            {
                var x = j * CellSize + borderSize;
                var y = i * CellSize + borderSize;
                var type = Codes[mapLine[j]];
                var scene = new ScenicObject(new Vector2(x, y), type, image[type], CellSize);
                map.Add(scene);
            }  
        }
        return map;
    }
}