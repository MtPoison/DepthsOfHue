using System.Collections.Generic;
using UnityEngine;

public class LoopBackgroundMusic : MonoBehaviour
{
    public static LoopBackgroundMusic Instance;
    
    [Header("Property")]
    [SerializeField] private List<AudioClip> audioClips;
    [SerializeField] private AudioSource audioSource;
    
    private int currentClipIndex;
    private float currentTimer;
    private float actualAudioLength;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
    }

    private void Start()
    {
        currentClipIndex = 0;
        currentTimer = 0f;
        SwitchAudio();
    }

    private void SwitchAudio()
    {
        currentTimer = 0f;
        audioSource.clip = audioClips[currentClipIndex];
        audioSource.Play();
        currentClipIndex++;
        if (currentClipIndex >= 2) currentClipIndex = 0;
        actualAudioLength = audioClips[currentClipIndex].length;
    }

    private void Update()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer >= actualAudioLength)
        {
            SwitchAudio();
        }
    }
}
