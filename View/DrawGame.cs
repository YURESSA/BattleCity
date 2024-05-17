using Microsoft.Xna.Framework;

namespace BattleCity;

public class DrawGame
{
    private BattleCity _battleCity;

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
        }
    }

    private void DrawMainMenu(MenuModel menuModel)
    {
        _battleCity.GraphicsDevice.Clear(Color.Black);
        _battleCity.Menu.Draw(_battleCity.SpriteBatch, menuModel);
    }

    private void Drawn()
    {
        _battleCity.GraphicsDevice.Clear(Color.Black);

        foreach (var tank in _battleCity.PlayersTanks)
        {
            PlayerView.Draw(_battleCity.SpriteBatch, tank);
        }
           
        DrawSceneObjects(TypeOfObject.Water);
        foreach (var tank in _battleCity.EnemyTanks)
            tank.Draw(_battleCity.SpriteBatch);
       
        foreach (var bulletObject in _battleCity.BulletObjects)
            bulletObject.Draw(_battleCity.SpriteBatch);
        DrawSceneObjectsExcept(TypeOfObject.Water);
    }

    private void DrawSceneObjects(TypeOfObject type)
    {
        foreach (var scenic in _battleCity.SceneObjects)
            if (scenic.SceneModel.Type == type)
                scenic.Draw(_battleCity.SpriteBatch);
    }

    private void DrawSceneObjectsExcept(TypeOfObject type)
    {
        foreach (var scenic in _battleCity.SceneObjects)
            if (scenic.SceneModel.Type != type)
                scenic.Draw(_battleCity.SpriteBatch);
    }

    private void DrawDefeatLevel()
    {
        _battleCity.GameDefeat.Draw(_battleCity.SpriteBatch);
    }
}