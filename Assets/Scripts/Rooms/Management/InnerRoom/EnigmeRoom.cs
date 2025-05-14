using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class EnigmeRoom : Room
{
    [SerializeField] protected List<Enigme> enigmes;

    public Enigme currentEnigme;

    private int enigmesResolved = 0;
      
    [SerializeField] private GameObject successBanner;
    [SerializeField] private CanvasGroup bannerCanvasGroup;
    [SerializeField] private RectTransform bannerTransform;
    

    

    protected virtual void Start()
    {
        if (InputItems.Instance != null)
        {
            InputItems.Instance.OnClickOnGameObject += HandleObjectClick;
        }
    }

    [ContextMenu("Initialize")]
    /// <summary>
    /// This override is used intialize the room.
    /// </summary>
    public override void Initialize()
    {
        foreach (var enigme in enigmes)// subscribe to each enigme OnSucces event.
        {
            enigme.OnSuccess -= OnEnigmeResolved; // if already subscribed
            enigme.OnSuccess += OnEnigmeResolved;
        }
        InitilizeCurrentEnigma();
    }

    /// <summary>
    /// This function is used to initialize and launch the first enigme not resolved yet in the enigmas list.
    /// </summary>
    public virtual void InitilizeCurrentEnigma()
    {
        foreach (var enigme in enigmes)
        {
            if (enigme.GetIsResolved())
            {
                continue;
            }
            else
            {
                currentEnigme = enigme;
                enigme.Initialize();
                break;
            }
        }
    }

    /// <summary>
    /// This function is called whenever an enigme is resolved.
    /// </summary>
    protected void OnEnigmeResolved()
    {
        SuccessSequence();
        Debug.Log("+++");
        enigmesResolved++;
              
    }

    protected virtual void OnPostEnigme()
    {
        if (IsRoomComplete())
        {
            EndRoomSequence();
        }
        else
        {          
            InitilizeCurrentEnigma();
        }
    }

    /// <summary>
    /// Detects if the room has enigmes left. False = not all enigmes done.
    /// </summary>
    /// <returns></returns>
    public bool IsRoomComplete()
    {
        Debug.Log ("count : " + enigmes.Count);
        Debug.Log("completed : " + enigmesResolved);
        return enigmesResolved >= enigmes.Count;
    }


    /// <summary>
    /// End of room sequence. (logique..)
    /// </summary>
    public virtual void EndRoomSequence()
    {
        Debug.Log("fini");
        roomData.CurrentState = RoomStateEnum.Completed;
        roomData.roomState = roomData.CurrentState;


    }


    /// <summary>
    /// Ending Sequence with visual effects
    /// </summary>
    protected virtual void SuccessSequence()
    {

        successBanner.SetActive(true);

        float screenWidth = ((RectTransform)bannerTransform.parent).rect.width;
        float screenHeight = ((RectTransform)bannerTransform.parent).rect.height;

        float inOutDuration = 2f;
        // Positions X
        float xStart = screenWidth / 10;
        float xMid = screenWidth / 2;
        float xEnd = screenWidth / 4 * 9;

        // Y position fixe de base
        float y = screenHeight / 4;

        // Reset position + alpha
        bannerTransform.anchoredPosition = new Vector2(xStart, y);
        bannerCanvasGroup.alpha = 0;

        // Move to center + fade in
        Sequence seq = DOTween.Sequence();
        seq.Append(bannerCanvasGroup.DOFade(1f, 1f));
        seq.Join(bannerTransform.DOAnchorPosX(xMid, inOutDuration).SetEase(Ease.OutCubic));

        int bounceCount = 0;
        int maxBounces = 2;

        Tween floatTween = bannerTransform
            .DOAnchorPosY(y + 40f, 1f)
            .SetLoops(maxBounces * 2, LoopType.Yoyo) // 2 loops = 1 rebond complet
            .SetEase(Ease.InOutSine)
            .OnStepComplete(() =>
            {
                bounceCount++;
                if (bounceCount % 2 == 0) // rebond complet termin�
                {

                    if (bounceCount >= maxBounces * 2)
                    {

                        // Lancement de la suite (fade out + d�placement)
                        ContinueBannerExit(xEnd);
                    }
                }
            });


    }
    void ContinueBannerExit(float xValue)
    {

        Sequence exitSeq = DOTween.Sequence();
        exitSeq.Append(bannerCanvasGroup.DOFade(0f, 1f));
        exitSeq.Join(bannerTransform.DOAnchorPosX(xValue, 2f).SetEase(Ease.InCubic));

        // Callback when the animation is coming to an end
        exitSeq.AppendCallback(() =>
        {
            successBanner.SetActive(false);
            OnPostEnigme(); 
        });
    }

    /// <summary>
    /// Manage object's click behavior depending on the enigme
    /// </summary>
    /// <param name="robject"></param>
    protected virtual void HandleObjectClick(GameObject robject)
    {

        if (currentEnigme != null)
        {
            Debug.Log("OLE");
            currentEnigme.CheckItem(robject);
        }
    }

    public void GetHintCurrentEnigme()
    {
        if (currentEnigme.hintUsed < currentEnigme.hintLeft)
        {
            DialogueManager.Instance.StartNewDialogue(currentEnigme.hintUsed, currentEnigme.enigmeHintKey);
            currentEnigme.hintUsed++;
            if (currentEnigme.hintUsed == currentEnigme.hintLeft)
            {
                currentEnigme.hintUsed = 0;
            }
        }
    }

}
