using System;
using System.IO;
using Avalonia.Platform;
using PortAudioSharp;
using SoundFlow.Backends.MiniAudio;
using SoundFlow.Components;
using SoundFlow.Enums;
using SoundFlow.Interfaces;
using SoundFlow.Providers;
using SampleFormat = SoundFlow.Enums.SampleFormat;

namespace OneVs100.ViewModels;

public sealed class AudioPlayer
{
    private static readonly Lazy<AudioPlayer> LazyInstance = new(() => new AudioPlayer());
    public static AudioPlayer Instance => LazyInstance.Value;
    private MiniAudioEngine engine;
    private SoundPlayer? player;
    private AudioPlayer()
    {
        engine = new MiniAudioEngine(44100, Capability.Playback);
    }
    
    public SoundPlayer PlaySound(Uri soundUri)
    {
        player = new SoundPlayer(new StreamDataProvider(AssetLoader.Open(soundUri)));
        Mixer.Master.AddComponent(player);
        player.Play();
        return player;
    }

    public void StopSound(SoundPlayer soundPlayer)
    {
        soundPlayer.Stop();
        Mixer.Master.RemoveComponent(soundPlayer);
    }

    public void Dispose()
    {
        /*ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);*/
    }
    
    ~AudioPlayer()
    {
        ReleaseUnmanagedResources();
    }
    
    private void ReleaseUnmanagedResources()
    {
        if (player != null)
            Mixer.Master.RemoveComponent(player);
    }
}

public static class SoundEffects
{
    public static readonly Uri Test = new Uri("avares://OneVs100/Assets/Audio/test.wav");
    public static readonly Uri MobMemberOut = new Uri("avares://OneVs100/Assets/Audio/mob_out.wav");
    public static readonly Uri PlayerIntro = new Uri("avares://OneVs100/Assets/Audio/player_intro.wav");
}