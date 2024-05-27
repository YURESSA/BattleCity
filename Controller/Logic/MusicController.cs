using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace BattleCity;

public class MusicController
{
    private readonly BattleCity _battleCity;
    private static Dictionary<string, SoundEffect> _soundEffects;
    private static SoundEffectInstance _backgroundMusicInstance;
    private static SoundEffectInstance _startMusicInstance;
    private static SoundEffectInstance _endMusicInstance;

    public static float VolumeOfSounds;
    public static float VolumeOfBackground;

    public MusicController(BattleCity battleCity)
    {
        _battleCity = battleCity;
        _soundEffects = new Dictionary<string, SoundEffect>();
    }

    public void LoadContent()
    {
        _soundEffects["Shot"] = _battleCity.Content.Load<SoundEffect>("shot");
        _soundEffects["Destroy"] = _battleCity.Content.Load<SoundEffect>("destroy");
        _soundEffects["Start"] = _battleCity.Content.Load<SoundEffect>("level_start");
        _soundEffects["End"] = _battleCity.Content.Load<SoundEffect>("level_finish");
        _soundEffects["Move"] = _battleCity.Content.Load<SoundEffect>("move");
        _soundEffects["Engine"] = _battleCity.Content.Load<SoundEffect>("engine");
    }

    private static void PlaySound(string soundName, bool isLooped = false)
    {
        if (!_soundEffects.TryGetValue(soundName, out var soundEffect)) return;
        var instance = soundEffect.CreateInstance();
        instance.Volume = VolumeOfSounds;
        instance.IsLooped = isLooped;
        instance.Play();
    }

    public static void PlayShot()
    {
        PlaySound("Shot");
    }

    public static void PlayDestroy()
    {
        PlaySound("Destroy");
    }

    public static void StartLevelMusic()
    {
        _backgroundMusicInstance = _soundEffects["Engine"].CreateInstance();
        _backgroundMusicInstance.Volume = VolumeOfBackground;
        _backgroundMusicInstance.IsLooped = true;
        _backgroundMusicInstance.Play();
    }

    public static void PauseLevelMusic()
    {
        _backgroundMusicInstance.Pause();
    }

    public static void PlayStartMusic()
    {
        _startMusicInstance = _soundEffects["Start"].CreateInstance();
        _startMusicInstance.Volume = VolumeOfBackground;
        _startMusicInstance.IsLooped = false;
        _startMusicInstance.Play();
    }

    public static void PauseStartMusic()
    {
        _startMusicInstance.Pause();
    }

    public static void PlayEndMusic()
    {
        _endMusicInstance = _soundEffects["End"].CreateInstance();
        _endMusicInstance.Volume = VolumeOfBackground;
        _endMusicInstance.IsLooped = false;
        _endMusicInstance.Play();
    }

    public static void PauseEndMusic()
    {
        if (_endMusicInstance is not null)
            _endMusicInstance.Pause();
    }
}