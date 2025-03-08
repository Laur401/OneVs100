using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Platform;
using PortAudioSharp;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SampleFormat = SoundFlow.Enums.SampleFormat;

namespace OneVs100.ViewModels;

public sealed class AudioPlayer : IDisposable
{
    private static readonly Lazy<AudioPlayer> LazyInstance = new(() => new AudioPlayer());
    public static AudioPlayer Instance => LazyInstance.Value;
    private readonly MiniAudioEngine engine;
    private readonly List<SoundPlayer> activeSoundPlayers = new List<SoundPlayer>(); // Contains non-stopped players,
                                                                                     // might want to remove via callback,
                                                                                     // but would that be a severe issue?
                                                                                     // They're killed in Dispose().
    private AudioPlayer()
    {
        engine = new MiniAudioEngine(44100, Capability.Playback);
    }

    public void PlaySound(SoundEffect soundEffect)
    {
        PlaySound(soundEffect, out SoundPlayer? soundPlayer);
    }
    
    public void PlaySound(SoundEffect soundEffect, out SoundPlayer? player)
    {
        player = new SoundPlayer(new StreamDataProvider(AssetLoader.Open(soundEffect.Track)));
        if (soundEffect.LoopStart.HasValue)
        {
            player.IsLooping = true;
            player.SetLoopPoints(soundEffect.LoopStart.Value, soundEffect.LoopEnd);
        }
        
        Mixer.Master.AddComponent(player);
        
        player.Play();
        activeSoundPlayers.Add(player);
    }

    public void StopSound(ref SoundPlayer? soundPlayer)
    {
        if (soundPlayer == null)
        {
            Console.Error.WriteLine("Sound player is null.");
            return;
        }
        soundPlayer.Stop();
        activeSoundPlayers.Remove(soundPlayer);
        
        Mixer.Master.RemoveComponent(soundPlayer);
        
        soundPlayer = null;
    }

    public void StopAllSounds()
    {
        foreach (var player in activeSoundPlayers.ToList())
        {
            Mixer.Master.RemoveComponent(player);
            activeSoundPlayers.Remove(player);
        }
    }

    public void Dispose()
    {
        StopAllSounds();
        engine.Dispose();
    }
}

public static class SoundEffects
{
    public static readonly SoundEffect Test = new SoundEffect("avares://OneVs100/Assets/Audio/test.wav");
    public static readonly SoundEffect MobMemberOut = new SoundEffect("avares://OneVs100/Assets/Audio/mob_out.wav");
    public static readonly SoundEffect PlayerIntro = new SoundEffect("avares://OneVs100/Assets/Audio/player_intro.wav");
    public static readonly SoundEffect AnswerCorrect = new SoundEffect("avares://OneVs100/Assets/Audio/answer_correct.wav");
    public static readonly SoundEffect AnswerSelect = new SoundEffect("avares://OneVs100/Assets/Audio/answer_select.wav");
    public static readonly SoundEffect AnswerShow = new SoundEffect("avares://OneVs100/Assets/Audio/answer_show.wav");
    public static readonly SoundEffect AnswerWrong = new SoundEffect("avares://OneVs100/Assets/Audio/answer_wrong.wav");
    public static readonly SoundEffect BackgroundMoneyOrMob = new SoundEffect("avares://OneVs100/Assets/Audio/background_money_or_mob.wav", loopStart: 0.0f, loopEnd: 15.25f);
    public static readonly SoundEffect BackgroundQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/background_question.wav", loopStart: 0.0f, loopEnd: 8.0f);
    public static readonly SoundEffect BumperNextQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/bumper_next_question.wav");
    public static readonly SoundEffect MainIntro = new SoundEffect("avares://OneVs100/Assets/Audio/main_intro.wav");
    public static readonly SoundEffect MobNoWrong = new SoundEffect("avares://OneVs100/Assets/Audio/mob_no_wrong.wav");
    public static readonly SoundEffect TakeMob = new SoundEffect("avares://OneVs100/Assets/Audio/take_mob.wav");
    public static readonly SoundEffect TakeMoney = new SoundEffect("avares://OneVs100/Assets/Audio/take_money.wav");
    public static readonly SoundEffect TransitionMobWrongBoard = new SoundEffect("avares://OneVs100/Assets/Audio/transition_mob_wrong_board.wav");
    public static readonly SoundEffect TransitionNextQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/transition_next_question.wav");
    //QuestionBack - loop start 0 end 8.0
    //MoneyOrMobBack - loop start 0 end 15.25
}

public class SoundEffect (string soundUri, float? loopStart = null, float? loopEnd = null)
{
    public readonly Uri Track = new Uri(soundUri);
    public readonly float? LoopStart = loopStart;
    public readonly float? LoopEnd = loopEnd;
}