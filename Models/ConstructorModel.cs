using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class ConstructorModel
{
    public SceneController[,] ConstructorScene { get; private set; } = new SceneController[15, 15];
    public SceneController[,] Copy { get; set; }
    public int CurrentId { get; private set; }
    private TypeOfObject ChosenBlock { get; set; } = TypeOfObject.None;
    private readonly BattleCity _battleCity;
    public List<Vector2> CoordinateForEnemy { get; } = new();
    public List<Vector2> CoordinateForPlayer { get; } = new();

    private static readonly Dictionary<char, TypeOfObject> Codes = new()
    {
        { '0', TypeOfObject.None },
        { '1', TypeOfObject.Bricks },
        { '2', TypeOfObject.Concrete },
        { '3', TypeOfObject.Leaves },
        { '4', TypeOfObject.Water },
        { '5', TypeOfObject.Wall },
        { '9', TypeOfObject.Staff },
        { 'E', TypeOfObject.Enemy },
        { 'P', TypeOfObject.Player }
    };

    private static readonly Dictionary<TypeOfObject, char> ReverseCodes = new()
    {
        { TypeOfObject.None, '0' },
        { TypeOfObject.Bricks, '1' },
        { TypeOfObject.Concrete, '2' },
        { TypeOfObject.Leaves, '3' },
        { TypeOfObject.Water, '4' },
        { TypeOfObject.Wall, '5' },
        { TypeOfObject.Staff, '9' },
        { TypeOfObject.Enemy, 'E' },
        { TypeOfObject.Player, 'P' }
    };

    public ConstructorModel(BattleCity battleCity)
    {
        CurrentId = 9;
        _battleCity = battleCity;
    }

    public void LoadStartMap()
    {
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.Combine(appDirectory[..appDirectory.IndexOf("\\bin", StringComparison.Ordinal)],
            "Levels/custom.json");
        try
        {
            LoadMap(path);
        }
        catch (Exception)
        {
            InitializeConstructorScene();
        }
    }

    private void SaveMap(string fileName)
    {
        var mapData = new List<string>();
        for (var i = 0; i < 15; i++)
        {
            var row = string.Empty;
            for (var j = 0; j < 15; j++)
            {
                var type = ConstructorScene[i, j].SceneModel.Type;
                row += ReverseCodes[type];
            }

            mapData.Add(row);
        }

        var json = JsonSerializer.Serialize(new { Levels = new List<List<string>> { mapData } },
            new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
    }

    private void LoadMap(string fileName)
    {
        var json = File.ReadAllText(fileName);
        var mapData =
            JsonSerializer.Deserialize<MapData>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (mapData?.Levels == null || mapData.Levels.Count == 0)
            throw new Exception("Failed to read or deserialize the map data.");

        var level = mapData.Levels[0]; // Load the first level for simplicity

        for (var i = 0; i < 15; i++)
        {
            var row = level[i];
            for (var j = 0; j < 15; j++)
            {
                var type = Codes[row[j]];
                var x = j * BattleCity.BlockSize;
                var y = i * BattleCity.BlockSize;

                if (type == TypeOfObject.Enemy)
                {
                    CoordinateForEnemy.Add(new Vector2(x, y));
                    type = TypeOfObject.None; // Set the type to None after recording the coordinate
                }
                else if (type == TypeOfObject.Player)
                {
                    CoordinateForPlayer.Add(new Vector2(x, y));
                    type = TypeOfObject.None; // Set the type to None after recording the coordinate
                }

                var scene = GetScene(new Vector2(x, y), type, _battleCity.SceneDictionary[type],
                    _battleCity.SceneDictionary[TypeOfObject.None], true);
                ConstructorScene[i, j] = scene;
            }
        }
    }

    private SceneController GetScene(Vector2 position, TypeOfObject type, Texture2D texture, Texture2D noneTexture,
        bool isAlive)
    {
        var sceneModel = new SceneModel(position, type, texture.Width, texture.Height, isAlive);
        var sceneView = new SceneView(_battleCity.SceneDictionary, noneTexture);
        var sceneController = new SceneController(sceneModel) { SceneView = sceneView };
        return sceneController;
    }

    private void InitializeConstructorScene()
    {
        for (var i = 0; i < 15; i++)
        for (var j = 0; j < 15; j++)
        {
            var x = j * BattleCity.BlockSize;
            var y = i * BattleCity.BlockSize;
            var scene = i == 0 || i == 14 || j == 0 || j == 14
                ? GetScene(new Vector2(x, y), TypeOfObject.Wall,
                    _battleCity.SceneDictionary[TypeOfObject.Wall],
                    _battleCity.SceneDictionary[TypeOfObject.None], true)
                : GetScene(new Vector2(x, y), TypeOfObject.None,
                    _battleCity.SceneDictionary[TypeOfObject.None],
                    _battleCity.SceneDictionary[TypeOfObject.None], true);

            ConstructorScene[i, j] = scene;
        }

        var staffScene = GetScene(new Vector2(7 * BattleCity.BlockSize, 13 * BattleCity.BlockSize), TypeOfObject.Staff,
            _battleCity.SceneDictionary[TypeOfObject.Staff],
            _battleCity.SceneDictionary[TypeOfObject.None], true);
        ConstructorScene[13, 7] = staffScene;
    }

    public void UpdateChosenBlock(Vector2 position)
    {
        if (position.X != 15 || position.Y % 2 == 0 || !(1 <= position.Y) || !(position.Y <= 13)) return;
        CurrentId = (int)position.Y;
        ChosenBlock = position.Y switch
        {
            1 => TypeOfObject.Bricks,
            3 => TypeOfObject.Concrete,
            5 => TypeOfObject.Leaves,
            7 => TypeOfObject.Water,
            9 => TypeOfObject.None,
            11 => TypeOfObject.Enemy,
            13 => TypeOfObject.Player,
            _ => ChosenBlock
        };
    }

    public void UpdateMap(Vector2 position)
    {
        RemoveOldCoordinates(position);
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var path = Path.Combine(appDirectory[..appDirectory.IndexOf("\\bin", StringComparison.Ordinal)],
            "Levels/custom.json");
        SaveMap(path);
        if (IsValidPosition(position))
        {
            if (ChosenBlock is TypeOfObject.Enemy or TypeOfObject.Player)
                UpdateCoordinates(position);
            else if (ConstructorScene[(int)position.Y, (int)position.X].SceneModel.Type != TypeOfObject.Staff)
                UpdateScene(position);
        }
    }

    private void RemoveOldCoordinates(Vector2 position)
    {
        CoordinateForPlayer.RemoveAll(coordinate =>
            new Vector2((int)coordinate.X / BattleCity.BlockSize, 
                (int)coordinate.Y / BattleCity.BlockSize) == position &&
            ChosenBlock != TypeOfObject.Player);

        CoordinateForEnemy.RemoveAll(coordinate =>
            new Vector2((int)coordinate.X / BattleCity.BlockSize, (int)coordinate.Y / BattleCity.BlockSize) == position &&
            ChosenBlock != TypeOfObject.Enemy);
    }

    private static bool IsValidPosition(Vector2 position)
    {
        return position.X is < 14 and > 0 && position.Y is < 14 and > 0;
    }

    private void UpdateScene(Vector2 position)
    {
        var x = position.X * BattleCity.BlockSize;
        var y = position.Y * BattleCity.BlockSize;
        var scene = GetScene(new Vector2(x, y), ChosenBlock,
            _battleCity.SceneDictionary[ChosenBlock],
            _battleCity.SceneDictionary[TypeOfObject.None], true);
        ConstructorScene[(int)position.Y, (int)position.X] = scene;
    }

    private void UpdateCoordinates(Vector2 position)
    {
        var x = position.X * BattleCity.BlockSize;
        var y = position.Y * BattleCity.BlockSize;
        var scene = GetScene(new Vector2(x, y), TypeOfObject.None,
            _battleCity.SceneDictionary[TypeOfObject.None],
            _battleCity.SceneDictionary[TypeOfObject.None], true);

        ConstructorScene[(int)position.X, (int)position.Y] = scene;

        const int offset = 4;
        if (ChosenBlock == TypeOfObject.Enemy)
            CoordinateForEnemy.Add(new Vector2(x + offset, y + offset));
        else
            CoordinateForPlayer.Add(new Vector2(x + offset, y + offset));
    }

    public SceneController[,] DeepCopyArray(SceneController[,] original)
    {
        var rows = original.GetLength(0);
        var cols = original.GetLength(1);
        var copy = new SceneController[rows, cols];

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
            copy[i, j] = original[i, j]
                .Clone(_battleCity.SceneDictionary, _battleCity.SceneDictionary[TypeOfObject.None]);

        return copy;
    }

    private class MapData
    {
        public List<List<string>> Levels { get; set; }
    }
}