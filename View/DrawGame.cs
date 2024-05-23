using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BattleCity;

public class DrawGame
{
    private readonly BattleCity _battleCity;
    private Texture2D _rectangleBlock;
    public static SpriteFont TextBlock;
    public static Texture2D Vision { get; set; }
    public static Texture2D LevelIcon { get; set; }
    public static Texture2D FirstPlayerHp { get; set; }
    public static Texture2D SecondPlayerHp { get; set; }

    public DrawGame(BattleCity battleCity)
    {
        _battleCity = battleCity;
    }

    public void Drawing(MenuModel menuModel)
    {
        switch (_battleCity.State)
        {
            case StateOfGame.MainMenu:
                DrawMainMenu(menuModel);
                break;

            case StateOfGame.Pause:
                break;

            case StateOfGame.Game:
                Drawn();
                break;

            case StateOfGame.DefeatLevel:
                DrawDefeatLevel();
                break;
            case StateOfGame.Constructor:
                DrawConstructor(_battleCity.SpriteBatch);
                break;
        }
    }

    private void DrawConstructor(SpriteBatch spriteBatch)
    {
        var constructor = new ConstructorView(_battleCity);
        constructor.Draw(spriteBatch);
    }

    private void DrawMainMenu(MenuModel menuModel)
    {
        _battleCity.GraphicsDevice.Clear(Color.Black);
        _battleCity.Menu.Draw(_battleCity.SpriteBatch, menuModel);
    }

    private void Drawn()
    {
        _battleCity.GraphicsDevice.Clear(Color.Black);
        DrawRightBorder(_battleCity.SpriteBatch);
        foreach (var tank in _battleCity.PlayersTanks) PlayerView.Draw(_battleCity.SpriteBatch, tank);
        foreach (var view in _battleCity.BangModels.Select(bang => new BangView(bang)))
        {
            view.Draw(_battleCity.SpriteBatch);
        }

        DrawSceneObjects(TypeOfObject.Water);
        foreach (var tank in _battleCity.EnemyTanks)
            tank.Draw(_battleCity.SpriteBatch);

        foreach (var bulletObject in _battleCity.BulletObjects)
            bulletObject.Draw(_battleCity.SpriteBatch);
        DrawSceneObjectsExcept(TypeOfObject.Water);
    }

    private void InitializeTextures()
    {
        _rectangleBlock = new Texture2D(_battleCity.GraphicsDevice, 1, 1);
        _rectangleBlock.SetData(new[] { Color.White });
    }

    private void DrawRightBorder(SpriteBatch spriteBatch)
    {
        InitializeTextures();
        var position = new Point(960, 0);
        var size = new Point(1500, 960);
        var rectangle = new Rectangle(position, size);
        spriteBatch.Draw(_rectangleBlock, rectangle, new Color(128, 128, 128));
        DrawEnemiesInLevel(spriteBatch);
        DrawNumberOfLevel(spriteBatch);
        DrawHp(spriteBatch);
    }

    private void DrawHp(SpriteBatch spriteBatch)
    {
        var playersTanks = _battleCity.PlayersTanks.ToList();
        var isTwoPlayerMode = _battleCity.MainMenu.GameModeState == GameMode.TwoPlayer;

        if (isTwoPlayerMode)
            DrawPlayerHp(spriteBatch, SecondPlayerHp, new Vector2(960, 600),
                new Vector2(997, 617), playersTanks, 1);
        if (_battleCity.MainMenu.GameModeState == GameMode.OnePlayer || isTwoPlayerMode)
            DrawPlayerHp(spriteBatch, FirstPlayerHp, new Vector2(960, 500),
                new Vector2(997, 517), playersTanks, 0);
    }

    private void DrawPlayerHp(SpriteBatch spriteBatch, Texture2D hpTexture, Vector2 texturePosition,
        Vector2 textPosition, List<PlayerModel> playersTanks, int playerIndex)
    {
        spriteBatch.Draw(hpTexture, texturePosition, Color.White);

        var hp = 0;
        if (playerIndex < playersTanks.Count) hp = playersTanks[playerIndex].Hp;

        spriteBatch.DrawString(TextBlock, $"{hp}", textPosition, Color.Black);
    }


    private void DrawNumberOfLevel(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(LevelIcon,
            new Vector2(960, 750), Color.White);
        var position = new Vector2(987, 777);
        spriteBatch.DrawString(TextBlock, $"{_battleCity.NumberOfLevel}", position, Color.Black);
    }


    private void DrawEnemiesInLevel(SpriteBatch spriteBatch)
    {
        var enemiesToDraw = _battleCity.EnemyInLevel + _battleCity.EnemyTanks.Count;
        for (var j = 0; enemiesToDraw > 0; j++)
        for (var i = 0; i < 2 && enemiesToDraw > 0; i++)
        {
            spriteBatch.Draw(Vision,
                new Vector2(960 + i * Vision.Width, 64 + j * Vision.Height), Color.White);
            enemiesToDraw--;
        }
    }

    private void DrawSceneObjects(TypeOfObject type)
    {
        foreach (var scenic in _battleCity.SceneObjects)
            if (scenic.SceneModel.Type == type)
                scenic.SceneView.Draw(_battleCity.SpriteBatch, scenic.SceneModel);
    }

    private void DrawSceneObjectsExcept(TypeOfObject type)
    {
        foreach (var scenic in _battleCity.SceneObjects)
            if (scenic.SceneModel.Type != type)
                scenic.SceneView.Draw(_battleCity.SpriteBatch, scenic.SceneModel);
    }

    private void DrawDefeatLevel()
    {
        _battleCity.GameDefeat.Draw(_battleCity.SpriteBatch);
    }
}