﻿using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

internal enum StateOfGame
{
    MainMenu,
    Game,
    Level1,
    Level2,
    Level3,
    WinLevel,
    DefeatLevel,
    Final,
    Pause
}

public class BattleCity : Game
{
    private const int CellSize = 64;
    private readonly GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private StateOfGame _state = StateOfGame.MainMenu;
    private readonly HashSet<Shot> _bulletObjects = new();
    private HashSet<Enemy> _enemyTanks;
    private HashSet<Player> _playersTanks;
    private Menu _mainMenu;

    private readonly HashSet<Scene> _sceneObjectsForDeleted = new();
    private Scene[,] _sceneObjects;

    public BattleCity()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        _graphics.PreferredBackBufferHeight = 960;
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _mainMenu = new Menu(Content.Load<Texture2D>("MainMenu"));
        _playersTanks = new HashSet<Player>();
        _enemyTanks = new HashSet<Enemy>();
        Shot._texture = Content.Load<Texture2D>("bullet");
        var tankImage = Content.Load<Texture2D>("tank1");

        var playersTank = new Player(0.1f, new Vector2(326, 832), tankImage, CellSize, 
            HasCollision, _bulletObjects, true, 2);
        _playersTanks.Add(playersTank);

        var enemyTank = new Enemy(0.08f, new Vector2(64, 64), tankImage, CellSize, 
            HasCollision, _bulletObjects, true, 1);
        _enemyTanks.Add(enemyTank);
        var enemyTank2 = new Enemy(0.08f, new Vector2(832, 64), tankImage, CellSize, 
            HasCollision, _bulletObjects, true, 1);
        _enemyTanks.Add(enemyTank2);


        var images = new Dictionary<TypeOfObject, Texture2D>
        {
            { TypeOfObject.None, Content.Load<Texture2D>("none") },
            { TypeOfObject.Bricks, Content.Load<Texture2D>("bricks") },
            { TypeOfObject.Concrete, Content.Load<Texture2D>("concrete") },
            { TypeOfObject.Leaves, Content.Load<Texture2D>("leaves") },
            { TypeOfObject.Water, Content.Load<Texture2D>("water") },
            { TypeOfObject.Staff, Content.Load<Texture2D>("staff") },
            { TypeOfObject.Wall, Content.Load<Texture2D>("wall") }
        };
        _sceneObjects = ReaderOfMap.Reader(images, CellSize, "input.txt");
    }

    protected override void Update(GameTime gameTime)
    {
        switch (_state)
        {
            case StateOfGame.MainMenu:
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                    _state = StateOfGame.Game;
                
                if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();
                break;
            case StateOfGame.Game:
                UpdateObjects(gameTime);
                RemoveNotAliveObjects();
                
                if (Keyboard.GetState().IsKeyDown(Keys.P))
                    _state = StateOfGame.MainMenu;
                break;
        }

        base.Update(gameTime);
    }

    private void RemoveNotAliveObjects()
    {
        _bulletObjects.RemoveWhere(element => element.ShotModel.ShotHasCollisions);
        _bulletObjects.RemoveWhere(element => element.ShotModel.IsAlive == false);
        _playersTanks.RemoveWhere(element => element.PlayerModel.IsAlive == false);
        _enemyTanks.RemoveWhere(element => element._enemyModel.IsAlive == false);
        if (_playersTanks.Count == 0)
            _state = StateOfGame.DefeatLevel;
        else if (_enemyTanks.Count == 0)
        {
            _state = StateOfGame.WinLevel;
        }
    }

    private void UpdateObjects(GameTime gameTime)
    {
        var userCoordinate = Vector2.One;
        foreach (var tanks in _playersTanks)
        {
            tanks.Update(gameTime);
            userCoordinate = tanks.PlayerModel.GetCoordinate();
        }

        foreach (var enemyTank in _enemyTanks)
            enemyTank.Update(gameTime, _sceneObjects, userCoordinate);

        foreach (var bullet in _bulletObjects)
            bullet.Update(gameTime);

        foreach (var scene in _sceneObjects)
            scene.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _spriteBatch.Begin();
        switch (_state)
        {
            case StateOfGame.MainMenu:
                GraphicsDevice.Clear(Color.Black);
                _mainMenu.Draw(_spriteBatch);
                break;
            case StateOfGame.Game:
                GraphicsDevice.Clear(Color.Gray);
                foreach (var scenic in _sceneObjects)
                    scenic.Draw(_spriteBatch);
                foreach (var tank in _playersTanks)
                    tank.Draw(_spriteBatch);
                foreach (var tank in _enemyTanks)
                    tank.Draw(_spriteBatch);
                foreach (var bulletObject in _bulletObjects)
                    bulletObject.Draw(_spriteBatch);
                break;
        }

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private bool HasCollision(MovedObject obj)
    {
        foreach (var scene in _sceneObjects)
            if (scene.SceneModel.Intersect(obj) && scene.SceneModel.Type != TypeOfObject.None)
            {
                if ((scene.SceneModel.Type == TypeOfObject.Bricks ||
                     scene.SceneModel.Type == TypeOfObject.Staff) && obj is ShotModel)
                {
                    if (scene.SceneModel.Type == TypeOfObject.Staff)
                        _state = StateOfGame.DefeatLevel;
                    scene.SceneModel.IsAlive = false;
                }
                   
                return true;
            }


        foreach (var shot in _bulletObjects)
        {
            if (obj.Intersect(shot.ShotModel) && obj is ShotModel && obj != shot.ShotModel)
            {
                shot.ShotModel.IsAlive = false;
                return true;
            }
        }
            

        foreach (var playersTank in _playersTanks)
        {
            if (!obj.Intersect(playersTank.PlayerModel) ||
                obj == playersTank.PlayerModel || obj.Parent == playersTank.PlayerModel) continue;
            playersTank.PlayerModel.Kill();
            return true;
        }
        foreach (var enemyTank in _enemyTanks)
        {
            if (!obj.Intersect(enemyTank._enemyModel) ||
                obj == enemyTank._enemyModel || obj.Parent == enemyTank._enemyModel) continue;
            enemyTank._enemyModel.Kill();
            return true;
        }

        return false;
    }
}