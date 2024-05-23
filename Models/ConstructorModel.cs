using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class ConstructorModel
{
    public SceneView[,] ConstructorScene { get; private set; } = new SceneView[15, 15];
    public SceneView[,] Copy { get; set; }
    private TypeOfObject ChosenBlock { get; set; } = TypeOfObject.None;
    private readonly BattleCity _battleCity;
    public List<Vector2> CoordinateForEnemy { get; } = new();
    public List<Vector2> CoordinateForPlayer { get; } = new();

    public ConstructorModel(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }

    public void LoadStartMap()
    {
        if (Copy == null)
            InitializeConstructorScene();
        else
            ConstructorScene = Copy;
    }

    private void InitializeConstructorScene()
    {
        for (var i = 0; i < 15; i++)
        for (var j = 0; j < 15; j++)
        {
            var x = i * 64;
            var y = j * 64;
            var scene = i == 0 || i == 14 || j == 0 || j == 14
                ? new SceneView(new Vector2(x, y), TypeOfObject.Wall,
                    _battleCity.SceneDictionary[TypeOfObject.Wall],
                    _battleCity.SceneDictionary[TypeOfObject.None], 64, true)
                : new SceneView(new Vector2(x, y), TypeOfObject.None,
                    _battleCity.SceneDictionary[TypeOfObject.None],
                    _battleCity.SceneDictionary[TypeOfObject.None], 64, true);

            ConstructorScene[i, j] = scene;
        }

        var staffScene = new SceneView(new Vector2(448, 832), TypeOfObject.Staff,
            _battleCity.SceneDictionary[TypeOfObject.Staff],
            _battleCity.SceneDictionary[TypeOfObject.None], 64, true);
        ConstructorScene[7, 13] = staffScene;
    }

    public void UpdateChosenBlock(Vector2 position)
    {
        if (position.X == 15)
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

        if (IsValidPosition(position))
        {
            if (ChosenBlock is TypeOfObject.Enemy or TypeOfObject.Player)
                UpdateCoordinates(position);
            else if (ConstructorScene[(int)position.X, (int)position.Y].SceneModel.Type != TypeOfObject.Staff)
                UpdateScene(position);
        }
    }

    private void RemoveOldCoordinates(Vector2 position)
    {
        CoordinateForPlayer.RemoveAll(coordinate =>
            new Vector2((int)coordinate.X / 64, (int)coordinate.Y / 64) == position &&
            ChosenBlock != TypeOfObject.Player);

        CoordinateForEnemy.RemoveAll(coordinate =>
            new Vector2((int)coordinate.X / 64, (int)coordinate.Y / 64) == position &&
            ChosenBlock != TypeOfObject.Enemy);
    }

    private static bool IsValidPosition(Vector2 position)
    {
        return position.X < 14 && position.X > 0 && position.Y < 14 && position.Y > 0;
    }

    private void UpdateScene(Vector2 position)
    {
        var x = position.X * 64;
        var y = position.Y * 64;
        var scene = new SceneView(new Vector2(x, y), ChosenBlock,
            _battleCity.SceneDictionary[ChosenBlock],
            _battleCity.SceneDictionary[TypeOfObject.None], 64, true);
        ConstructorScene[(int)position.X, (int)position.Y] = scene;
    }

    private void UpdateCoordinates(Vector2 position)
    {
        var x = position.X * 64;
        var y = position.Y * 64;
        var scene = new SceneView(new Vector2(x, y), TypeOfObject.None,
            _battleCity.SceneDictionary[TypeOfObject.None],
            _battleCity.SceneDictionary[TypeOfObject.None], 64, true);
        ConstructorScene[(int)position.X, (int)position.Y] = scene;

        const int offset = 4;
        if (ChosenBlock == TypeOfObject.Enemy)
            CoordinateForEnemy.Add(new Vector2(x + offset, y + offset));
        else
            CoordinateForPlayer.Add(new Vector2(x + offset, y + offset));
    }

    public void Update()
    {
        _battleCity.SceneObjects = ConstructorScene;
        _battleCity.CoordinateForPlayer = CoordinateForPlayer;
        _battleCity.CoordinateForEnemy = CoordinateForEnemy;
        Copy = DeepCopyArray(ConstructorScene);
    }

    public SceneView[,] DeepCopyArray(SceneView[,] original)
    {
        var rows = original.GetLength(0);
        var cols = original.GetLength(1);
        var copy = new SceneView[rows, cols];

        for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
            copy[i, j] = (SceneView)original[i, j].Clone();

        return copy;
    }
}