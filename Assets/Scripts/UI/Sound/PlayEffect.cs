using UnityEngine;

public class PlayEffect : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;

    private void OnEnable()
    {
        CellSudoku.OnSendSoundEffect += PlaySoundEffect;
        Sudoku.OnSendSoundEffect += PlaySoundEffect;
        MapPuzzle.OnSendSoundEffect += PlaySoundEffect;
        SoundEnigme.OnSendSoundEffect += PlaySoundEffect;
    }

    private void OnDisable()
    {
        CellSudoku.OnSendSoundEffect -= PlaySoundEffect;
        Sudoku.OnSendSoundEffect -= PlaySoundEffect;
        MapPuzzle.OnSendSoundEffect -= PlaySoundEffect;
        SoundEnigme.OnSendSoundEffect -= PlaySoundEffect;
    }

    public void PlaySoundEffect()
    {
        if (!audioClip) return;
        audioSource.clip = audioClip;
        audioSource.Play();
    }
    
    public void PlaySoundEffect(AudioClip _clip)
    {
        if (!_clip) return;
        audioSource.clip = _clip;
        audioSource.Play();
    }
}
