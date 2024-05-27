using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace BattleCity;

public class SettingsController
{
    private static List<MenuOption> _menuOptions;
    private int _selectedOptionIndex;
    private readonly MenuView _menuView;
    private readonly BattleCity _battleCity;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    private readonly string _settingsFilePath;

    public SettingsController(SpriteFont font, BattleCity battleCity)
    {
        _battleCity = battleCity;
        _menuOptions = new List<MenuOption>
        {
            new("Танков в уровне", 1, 16),
            new("Танков в волне", 1, 10),
            new("Громкость эффектов", 0, 100),
            new("Громкость музыки", 0, 100)
        };

        _selectedOptionIndex = 0;
        _menuView = new MenuView(_battleCity.SpriteBatch, font);
        _previousKeyboardState = Keyboard.GetState();
        _previousMouseState = Mouse.GetState();
        var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
        _settingsFilePath = string.Concat(appDirectory.AsSpan(0,
            appDirectory.IndexOf("\\bin", StringComparison.Ordinal)), $"\\Settings/settings.json");
        LoadSettings();
    }

    public void Update()
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        HandleKeyboard(keyboardState);
        HandleMouse(mouseState);
        if (_previousKeyboardState != keyboardState || _previousMouseState != mouseState)
            SaveSettings();
        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;
    }

    private void HandleKeyboard(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.W) && _previousKeyboardState.IsKeyUp(Keys.W))
            _selectedOptionIndex = (_selectedOptionIndex - 1 + _menuOptions.Count) % _menuOptions.Count;

        if (keyboardState.IsKeyDown(Keys.S) && _previousKeyboardState.IsKeyUp(Keys.S))
            _selectedOptionIndex = (_selectedOptionIndex + 1) % _menuOptions.Count;

        if (keyboardState.IsKeyDown(Keys.A))
            _menuOptions[_selectedOptionIndex].Decrease();

        if (keyboardState.IsKeyDown(Keys.D))
            _menuOptions[_selectedOptionIndex].Increase();

        if (keyboardState.IsKeyDown(Keys.Enter) && _previousKeyboardState.IsKeyUp(Keys.Enter))
            _battleCity.State = StateOfGame.MainMenu;
    }

    private void HandleMouse(MouseState mouseState)
    {
        if (mouseState.ScrollWheelValue > _previousMouseState.ScrollWheelValue)
            _menuOptions[_selectedOptionIndex].Increase();
        else if (mouseState.ScrollWheelValue < _previousMouseState.ScrollWheelValue)
            _menuOptions[_selectedOptionIndex].Decrease();
    }

    public void Draw()
    {
        _menuView.Draw(_menuOptions, _selectedOptionIndex);
    }

    private void SaveSettings()
    {
        var settings = new SettingsData
        {
            TankLevel = _menuOptions[0].Value,
            WaveLevel = _menuOptions[1].Value,
            SoundVolume = _menuOptions[2].Value,
            MusicVolume = _menuOptions[3].Value
        };

        var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
        File.WriteAllText(_settingsFilePath, json);
    }

    public static void ReloadSettings()
    {
        UpdateGame.EnemyInLevel = _menuOptions[0].Value;
        UpdateGame.EnemyInWave = _menuOptions[1].Value;
        MusicController.VolumeOfSounds = _menuOptions[2].Value / 100f;
        MusicController.VolumeOfBackground = _menuOptions[3].Value / 100f;
    }

    private void LoadSettings()
    {
        if (File.Exists(_settingsFilePath))
        {
            var json = File.ReadAllText(_settingsFilePath);
            var settings = JsonConvert.DeserializeObject<SettingsData>(json);

            if (settings == null) return;
            _menuOptions[0].Value = settings.TankLevel;
            UpdateGame.EnemyInLevel = _menuOptions[0].Value;
            _menuOptions[1].Value = settings.WaveLevel;
            UpdateGame.EnemyInWave = _menuOptions[1].Value;
            _menuOptions[2].Value = settings.SoundVolume;
            MusicController.VolumeOfSounds = _menuOptions[2].Value / 100f;
            _menuOptions[3].Value = settings.MusicVolume;
            MusicController.VolumeOfBackground = _menuOptions[3].Value / 100f;
        }
        else
        {
            SaveSettings();
        }
        
    }
}

public class SettingsData
{
    public int TankLevel { get; init; }
    public int WaveLevel { get; init; }
    public int SoundVolume { get; init; }
    public int MusicVolume { get; init; }
}