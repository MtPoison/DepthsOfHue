using UnityEngine;

[System.Serializable]
public class Sound : MonoBehaviour
{
    public enum AudioTypes
    {
        soundEffect,
        music
    }
    public AudioTypes audioType;

    [HideInInspector] public AudioSource source;
    public string clipName;
    public AudioClip audioClip;
    public bool isLoop;
    public bool playOnAwake;
    [Range(0f, 10f)] public float volume = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
