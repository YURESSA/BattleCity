using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BattleCity;

public class PlayerController
{
    private readonly ControlButton _controlButton;
    private readonly PlayerModel _playerModel;

    public PlayerController(PlayerModel playerModel, ControlButton controlButton)
    {
        _playerModel = playerModel;
        _controlButton = controlButton;
    }

    public void Update(GameTime gameTime)
    {
        HandleInput();
        _playerModel.Update(gameTime);
    }

    private void HandleInput()
    {
        var keyboardState = Keyboard.GetState();
        _playerModel.Direction = Vector2.Zero;

        if (keyboardState.IsKeyDown(_controlButton.Left))
            _playerModel.MoveLeft();
        if (keyboardState.IsKeyDown(_controlButton.Right))
            _playerModel.MoveRight();
        if (keyboardState.IsKeyDown(_controlButton.Up))
            _playerModel.MoveUp();
        if (keyboardState.IsKeyDown(_controlButton.Down))
            _playerModel.MoveDown();
        if (keyboardState.IsKeyDown(_controlButton.Shoot))
            _playerModel.HandleShooting();
    }
}