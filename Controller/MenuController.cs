using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class MenuController
{
    private readonly ControlButton _controlButton;
    private readonly MenuModel _menuModel;
    private KeyboardState _previousKeyboardState;


    public MenuController(MenuModel menuModel)
    {
        _menuModel = menuModel;
        _previousKeyboardState = Keyboard.GetState();
    }

    public void Update(GameTime gameTime)
    {
        HandleInput();
        _menuModel.Update(gameTime);
    }

    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();

        if (keyboardState.IsKeyDown(Keys.S) && _previousKeyboardState.IsKeyUp(Keys.S))
            _menuModel.CursorMoveDown();
        if (keyboardState.IsKeyDown(Keys.W) && _previousKeyboardState.IsKeyUp(Keys.W))
            _menuModel.CursorMoveUp();
        if (keyboardState.IsKeyDown(Keys.Enter))
            _menuModel.StartGame();
        _previousKeyboardState = keyboardState;
    }
}