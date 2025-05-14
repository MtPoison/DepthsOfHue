using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup soundEffectsMixerGroup;
    [SerializeField] private AudioMixerGroup underwaterEffectsMixerGroup;
    [SerializeField] private Sound[] sounds;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.audioClip;
            s.source.volume = s.volume;

            switch (s.audioType)
            {
                case Sound.AudioTypes.music:
                    s.source.outputAudioMixerGroup = musicMixerGroup;
                    break;
                case Sound.AudioTypes.soundEffect:
                    s.source.outputAudioMixerGroup = soundEffectsMixerGroup;
                    break;
            }

            if(s.playOnAwake)
            {
                s.source.Play();
            }
        }
    }

    public void Play(string clipname)
    {
        Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + " does EXIST NOT.");
            return;
        }
        s.source.Play();
    }

    public void Stop(string clipname)
    {
        Sound s = Array.Find(sounds, dummySound => dummySound.clipName == clipname);
        if (s == null)
        {
            Debug.LogError("Sound: " + clipname + " does EXIST NOT.");
            return;
        }
        s.source.Stop();
    }

    public void UpdateMixerVolume()
    {
        float musicDB = Mathf.Log10(AudioOptionManager.musicVolume) * 20;
        musicMixerGroup.audioMixer.SetFloat("Music Volume", AudioOptionManager.musicVolume);

        float soundEffectsDB = Mathf.Log10(AudioOptionManager.soundEffectsVolume) * 20;
        soundEffectsMixerGroup.audioMixer.SetFloat("Sound Effects Volume", AudioOptionManager.soundEffectsVolume);
        
        underwaterEffectsMixerGroup.audioMixer.SetFloat("Underwater Volume", AudioOptionManager.soundEffectsVolume/2);
    }
}
