using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundEnigme : Enigme
{
    bool isAnimating = false;
    public List<Corail> corals;  
    public List<Corail> correctSequence; 
    private List<Corail> playerSequence;  
    public GameObject statue;

    public Color idleColor;
    public Color testColor;
    public Color sequenceColor;

    bool isSequencePlaying = false;

    public int amountOfNotes;

    public Light enigmeLight;


    private bool sequenced =false;

    public static SoundEnigme Instance;
    public GameObject canvaSoundEnigme;


    public AudioClip lose;
    [SerializeField] private AudioClip success;

    #region Event

    public delegate void SendSoundEffect(AudioClip _clip);

    public static event SendSoundEffect OnSendSoundEffect;

    #endregion

    public override void Initialize()
    {
        enigmeLight.color = testColor;
        canvaSoundEnigme.SetActive(true);
        corals.Clear();
        correctSequence.Clear();
   

        

        if (Instance == null)
        {
            Instance = this;

        }
        base.Initialize();

        

        for (int i = 0; i < objectsInEnigme.Count; i++)
        {
            if (objectsInEnigme[i].GetComponent<Corail>() != null)
            {
                corals.Add(objectsInEnigme[i].GetComponent<Corail>());
            }
        }
        
    }

    public void StartRound()
    {
        if (isResolved == false)
        {
            isSequencePlaying = true;

            foreach (Corail corail in corals)
            {

                DisableGlow(corail);
            }
            StopAllCoroutines();

            
            enigmeLight.color = sequenceColor;
            //panel.SetActive(true);

            sequenced = true;
            //StartCoroutine(FadeOverlay(1));
            GenerateMelody();
            playerSequence = new List<Corail>();
        }
    }

    //IEnumerator FadeOverlay(float targetAlpha)
    //{
    //    float duration = 0.5f;
    //    float startAlpha = panelImage.alpha;
    //    float t = 0f;

    //    while (t < duration)
    //    {
    //        t += Time.deltaTime;
    //        panelImage.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / duration);
    //        yield return null;
    //    }


    //    panelImage.alpha = targetAlpha;

    //}

    void GenerateMelody()
    {
        correctSequence = new List<Corail>();
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        for (int i = 0; i < amountOfNotes; i++)
        {
            int index = UnityEngine.Random.Range(0, corals.Count);
            Corail coral = corals[index];

            correctSequence.Add(coral);

            yield return StartCoroutine(PlaySoundWithGlow(coral)); 
            
        }

        isSequencePlaying = false;
    }


    public void OnCoralClicked(Corail coral)
    {
        if (sequenced == true)
        {
            if (isAnimating == false)
            {

                playerSequence.Add(coral);


                for (int i = 0; i < playerSequence.Count; i++)
                {
                    if (playerSequence[i] != correctSequence[i])
                    {

                        statue.GetComponent<AudioSource>().PlayOneShot(lose);
                        DialogueManager.Instance.StartNewDialogue(2, DialogueGroupKey.chantsSirene);
                        ResetPuzzle();

                        return;
                    }
                }
                PlayCoral(coral);

                if (playerSequence.Count == correctSequence.Count)
                {
                    DialogueManager.Instance.StartNewDialogue(1, DialogueGroupKey.chantsSirene);
                    SolvePuzzle();
                }
            }
        }
        else
        {
            PlayCoral(coral);
        }
    }
    void ResetPuzzle()
    {
        sequenced = false;
        playerSequence.Clear();
        StopAllCoroutines();
        foreach (var coral in corals)
        {
            DisableGlow(coral);
        }

        //StartCoroutine(FlashRedThenFadeOut()); 
        enigmeLight.color = testColor;
    }

    void SolvePuzzle()
    {
        
        StartCoroutine(RotateStatueTowardsCamera());
    }

    void PlayCoral(Corail coral)
    {
        StartCoroutine(PlaySoundWithGlow(coral));
    }

    IEnumerator PlaySoundWithGlow(Corail coral)
    {
        EnableGlow(coral);

        AudioClip sound = coral.GetAudioClip();
        AudioSource audioSource = statue.GetComponent<AudioSource>();
        float newPitch = UnityEngine.Random.Range(0.7f, 1.2f);

        audioSource.pitch = newPitch;
        audioSource.PlayOneShot(sound);

        yield return new WaitForSeconds(sound.length * newPitch + 0.5f);

        DisableGlow(coral);
 
    }
    void EnableGlow(Corail coral)
    {
        foreach (Material mat in coral.GetMaterials())
        {
            mat.EnableKeyword("_EMISSION");

            Color baseColor = mat.GetColor("_BaseColor"); // ou "_Color" selon ton shader
            mat.SetColor("_EmissionColor", baseColor * 2);

            StartCoroutine(PulseGlow(mat));
        }
    }

    void DisableGlow(Corail coral)
    {
        foreach (Material mat in coral.GetMaterials())
        {
            mat.DisableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", Color.black);
        }
    }
    IEnumerator PulseGlow(Material mat, float pulseDuration = 1.5f, float fadeOutDuration = 1f)
    {
        float time = 0f;
        float pulseSpeed = 1f; // vitesse du pulsé
        Color baseColor = mat.GetColor("_BaseColor"); // ou "_Color" si ton shader n'a pas _BaseColor

        // Phase de pulse
        while (time < pulseDuration)
        {
            time += Time.deltaTime;
            float intensity = (Mathf.Sin(time * pulseSpeed * Mathf.PI * 2f) + 1f) / 2f;
            mat.SetColor("_EmissionColor", baseColor * Mathf.Lerp(0.5f, 1.5f, intensity));
            yield return null;
        }

        // Phase de fade-out
        Color currentColor = mat.GetColor("_EmissionColor");
        float fadeTime = 0f;
        while (fadeTime < fadeOutDuration)
        {
            fadeTime += Time.deltaTime;
            float t = fadeTime / fadeOutDuration;
            mat.SetColor("_EmissionColor", Color.Lerp(currentColor, baseColor * 0f, t));
            yield return null;
        }

        mat.DisableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.black);
    }

    

    

    IEnumerator RotateStatueTowardsCamera()
    {
        isAnimating = true;
        float duration = 5f;
        float timer = 0f;

        Quaternion startRotation = statue.transform.localRotation;
        Quaternion endRotation = startRotation * Quaternion.Euler(0f, 90f, 0f); 

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;

            statue.transform.localRotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }
        isAnimating = false;
        statue.transform.localRotation = endRotation;
        
        canvaSoundEnigme.SetActive(false);
        enigmeLight.color = idleColor;
        Success();
        if (success) OnSendSoundEffect?.Invoke(success);
    }


    //IEnumerator FlashRedThenFadeOut()
    //{
    //    // Flash rouge
    //    Color originalColor = panelBackground.color;
    //    panelBackground.color = new Color(1f, 0f, 0f, originalColor.a);
    

    //    yield return new WaitForSeconds(0.7f); // durée du flash

        

    //    // Fade out ensuite
    //    yield return StartCoroutine(FadeOverlay(0));

    //    // Retour à la couleur d'origine (noir ou autre)
    //    panelBackground.color = originalColor;
    //}

    public override void CheckItem(GameObject item)
    {
        base.CheckItem(item);
        
        Corail itemCorail = item.GetComponent<Corail>();

        if (itemCorail != null && !isAnimating && !isSequencePlaying)
        {
            OnCoralClicked(itemCorail);
        }
    }

    public override void EnigmeEndReset()
    {
        base.EnigmeEndReset();
        enigmeLight.color = idleColor;
        canvaSoundEnigme.SetActive(false);

        foreach(Corail corail in corals) {

            DisableGlow(corail);
        }
        sequenced = false;
        playerSequence.Clear();
        correctSequence.Clear();
        StopAllCoroutines();
    }
}
