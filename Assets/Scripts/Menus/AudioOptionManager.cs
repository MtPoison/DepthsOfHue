using UnityEngine;
using UnityEngine.UI;

public class AudioOptionManager : MonoBehaviour
{  
    [SerializeField] private Slider musicSlider;
    [SerializeField] private RectTransform handleMusicSlider;
    [SerializeField] private Slider soundEffectsSlider;
    [SerializeField] private RectTransform handleSoundEffectsSlider;
    private bool isLoad;
    
    public Slider MusicSlider
    {
        get => musicSlider;
        set => musicSlider = value;
    }

    public Slider SoundEffectsSlider
    {
        get => soundEffectsSlider;
        set => soundEffectsSlider = value;
    }

    public bool IsLoad
    {
        get => isLoad;
        set => isLoad = value;
    }

    public static float musicVolume {  get; private set; }
    public static float soundEffectsVolume { get; private set; }

    public float startMusicVolume = 5f;
    public float startSoundEffectsVolume = 5f;

    public delegate void SendStartLoadAudioToSave();
    public static event SendStartLoadAudioToSave OnSendStartLoadAudioToSave;

    private void Start()
    {
        OnSendStartLoadAudioToSave?.Invoke();
        OnMusicSliderValueChange();
        OnSoundEffectsSliderValueChange();
        
        if (!isLoad)
        {
            musicSlider.value = startMusicVolume;
            AudioManager.Instance.UpdateMixerVolume();
            
            soundEffectsSlider.value = startSoundEffectsVolume;
            AudioManager.Instance.UpdateMixerVolume();
        }
    }

    private void OnEnable()
    {
        if (!handleMusicSlider) return;
        //SetupFixPivot(handleMusicSlider, musicSlider);
        if (!handleSoundEffectsSlider) return;
        //SetupFixPivot(handleSoundEffectsSlider, soundEffectsSlider);
        
        Vector3 posM = handleMusicSlider.anchoredPosition;
        posM.x = 0f;
        handleMusicSlider.anchoredPosition = posM;
        
        Vector3 pos = handleSoundEffectsSlider.anchoredPosition;
        pos.x = 0f;
        handleSoundEffectsSlider.anchoredPosition = pos;
    }

    public void OnMusicSliderValueChange()
    {
        musicVolume = Mathf.Log10(musicSlider.value) * 20;
        AudioManager.Instance.UpdateMixerVolume();
        
        if (!handleMusicSlider) return;
        //SetupFixPivot(handleMusicSlider, musicSlider);
    }

    public void OnSoundEffectsSliderValueChange()
    {
        soundEffectsVolume = Mathf.Log10(soundEffectsSlider.value) * 20;
        AudioManager.Instance.UpdateMixerVolume();
        
        if (!handleSoundEffectsSlider) return;
        //SetupFixPivot(handleSoundEffectsSlider, soundEffectsSlider);
    }

    private void SetupFixPivot(RectTransform _rectTransform, Slider _slider)
    {
        if (!_rectTransform) return;
        FixPivot fixPivot = _rectTransform.gameObject.GetComponent<FixPivot>();
        switch (_slider.value)
        {
            case >= 0.5f when !fixPivot.IsSup:
                AdjustPivot(1f, _rectTransform);
                fixPivot.IsSup = true;
                fixPivot.IsDown = false;
                break;
            case < 0.5f when !fixPivot.IsDown:
                AdjustPivot(0.5f, _rectTransform);
                fixPivot.IsDown = true;
                fixPivot.IsSup = false;
                break;
        }
    }

    private void AdjustPivot(float _pivot, RectTransform _rectTransform)
    {
        Vector2 pivot = _rectTransform.pivot;
        pivot.x = _pivot;
        _rectTransform.pivot = pivot;
        Vector3 pos = _rectTransform.localPosition;
        pos.x = 0f;
        _rectTransform.localPosition = pos;
        Vector3 anchored = _rectTransform.anchoredPosition;
        anchored.x = 0f;
        _rectTransform.anchoredPosition = anchored;
    }
}
