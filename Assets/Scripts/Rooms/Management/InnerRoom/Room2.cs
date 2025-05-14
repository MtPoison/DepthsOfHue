using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Room2 : EnigmeRoom
{
    [SerializeField] private GameObject mapCanvas;
    [SerializeField] private GameObject SudokuCanvas;
    [SerializeField] private GameObject SoundCanvas;

    [SerializeField] private GameObject sirene;
    [SerializeField] private FramesManager frameM;

    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    public override void Initialize()
    {
      
        frameM.FrameSwitch -= EnigmeFrameSwitched;
        frameM.FrameSwitch += EnigmeFrameSwitched;

        foreach (var enigme in enigmes)// subscribe to each enigme OnSucces event.
        {
            enigme.OnSuccess -= OnEnigmeResolved; // if already subscribed
            enigme.OnSuccess += OnEnigmeResolved;
        }
    }

    public override void InitilizeCurrentEnigma()
    {
 
    }

    private void EnigmeFrameSwitched()
    {
        if ( currentEnigme!=null)
        {
            currentEnigme.EnigmeEndReset();
            currentEnigme = null;
        }   
    }
    public void InitializeSpecificEnigme(int enigme)
    {
        mapCanvas.SetActive(false);
        SudokuCanvas.SetActive(false);
        SoundCanvas.SetActive(false);



        currentEnigme = enigmes[enigme];

        enigmes[enigme].Initialize();
        DialogueManager.Instance.StartEnterPuzzleRoom(enigmes[enigme].enigmeDialogKey);
    }

    protected override void HandleObjectClick(GameObject robject)
    {
      
        if (robject == sirene)
        {
            InitializeSpecificEnigme(2);
        }

        base.HandleObjectClick(robject);
    }
}
