using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnigmaTimerManager : MonoBehaviour
{
    public static EnigmaTimerManager Instance;

    [SerializeField] private GameObject timerUIRoot;
    [SerializeField] private TextMeshProUGUI timerText; 

    /// <summary>
    /// Instance creation.
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        HideTimer();
    }

    public void ShowTimer()
    {
        timerUIRoot.SetActive(true);
    }

    public void HideTimer()
    {
        timerUIRoot.SetActive(false);
    }

    /// <summary>
    /// Update the display of the timer text. Parameter expects a time value in seconds.
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimerDisplay(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"Timer : {minutes:00}:{seconds:00}";
    }
}
