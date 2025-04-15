using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform;
using ExCSS;
using PortAudioSharp;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SampleFormat = SoundFlow.Enums.SampleFormat;

namespace OneVs100.ViewModels;

//Naudojate uždarytą ('sealed') arba dalinę ('partial') klasę (0.5 t.)
public sealed class AudioPlayer : IDisposable
{
    private static readonly Lazy<AudioPlayer> LazyInstance = new(() => new AudioPlayer());
    public static AudioPlayer Instance => LazyInstance.Value;
    private readonly MiniAudioEngine engine;
    private readonly List<SoundPlayer> activeSoundPlayers = new List<SoundPlayer>(); // Do not use for counting how many audio tracks are playing
    private AudioPlayer()
    {
        engine = new MiniAudioEngine(44100, Capability.Playback);
    }

    public int PlaySound(SoundEffect soundEffect)
    {
        return PlaySound(soundEffect, out SoundPlayer player);
    }
    
    public int PlaySound(SoundEffect soundEffect, out SoundPlayer soundPlayer)
    {
        soundPlayer = new SoundPlayer(new StreamDataProvider(AssetLoader.Open(soundEffect.Track)));
        if (soundEffect.LoopStart.HasValue)
        {
            soundPlayer.IsLooping = true;
            soundPlayer.SetLoopPoints(soundEffect.LoopStart.Value, soundEffect.LoopEnd);
        }
        
        soundPlayer.Volume = 0.7f;
        Mixer.Master.AddComponent(soundPlayer);
        
        soundPlayer.Play();
        activeSoundPlayers.Add(soundPlayer);
        if (soundEffect.AwaitTime.HasValue)
            return soundEffect.AwaitTime.Value;
        else return 0;
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
    public static readonly SoundEffect AnswerSelect = new SoundEffect("avares://OneVs100/Assets/Audio/answer_select.wav", awaitTime: 5280);
    public static readonly SoundEffect AnswerShow = new SoundEffect("avares://OneVs100/Assets/Audio/answer_show.wav");
    public static readonly SoundEffect AnswerWrong = new SoundEffect("avares://OneVs100/Assets/Audio/answer_wrong.wav", awaitTime: 950);
    public static readonly SoundEffect BackgroundMoneyOrMob = new SoundEffect("avares://OneVs100/Assets/Audio/background_money_or_mob.wav", loopStart: 0.0f, loopEnd: 15.25f);
    public static readonly SoundEffect BackgroundQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/background_question.wav", loopStart: 0.0f, loopEnd: 12.0f);
    public static readonly SoundEffect BumperNextQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/bumper_next_question.wav");
    public static readonly SoundEffect MainIntro = new SoundEffect("avares://OneVs100/Assets/Audio/main_intro.wav");
    public static readonly SoundEffect MobNoWrong = new SoundEffect("avares://OneVs100/Assets/Audio/mob_no_wrong.wav");
    public static readonly SoundEffect TakeMob = new SoundEffect("avares://OneVs100/Assets/Audio/take_mob.wav");
    public static readonly SoundEffect TakeMoney = new SoundEffect("avares://OneVs100/Assets/Audio/take_money.wav");
    public static readonly SoundEffect TransitionMobWrongBoard = new SoundEffect("avares://OneVs100/Assets/Audio/transition_mob_wrong_board.wav", awaitTime: 585);
    public static readonly SoundEffect TransitionNextQuestion = new SoundEffect("avares://OneVs100/Assets/Audio/transition_next_question.wav");
    public static readonly SoundEffect Victory = new SoundEffect("avares://OneVs100/Assets/Audio/victory.wav");
}

public class SoundEffect (string soundUri, float? loopStart = null, float? loopEnd = null, int? awaitTime = null)
{
    public readonly Uri Track = new Uri(soundUri);
    public readonly float? LoopStart = loopStart;
    public readonly float? LoopEnd = loopEnd;
    public readonly int? AwaitTime = awaitTime;
}