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

public class AudioPlayer : IDisposable
{
    private MiniAudioEngine engine;
    public AudioPlayer()
    {
        engine = new MiniAudioEngine(44100, Capability.Playback);
    }
    
    public void PlaySound(Uri soundUri)
    {
        Console.WriteLine(AppDomain.CurrentDomain.BaseDirectory);
        SoundPlayer player = new SoundPlayer(new StreamDataProvider(AssetLoader.Open(soundUri)));
        Mixer.Master.AddComponent(player);
        player.Play();
    }

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }
    
    ~AudioPlayer()
    {
        ReleaseUnmanagedResources();
    }
    
    private void ReleaseUnmanagedResources()
    {
        engine.Dispose();
    }
}

public static class SoundEffects
{
    public static readonly Uri Test = new Uri("avares://OneVs100/Assets/Audio/test.wav");
    public static readonly Uri MobMemberOut = new Uri("avares://OneVs100/Assets/Audio/mob_out.wav");
}