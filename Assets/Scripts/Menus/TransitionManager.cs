using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public Animator transition;
    public Animator transitionToMainMenu;
    [SerializeField]
    private float transitionTime = 1f;

    [SerializeField] private bool skipEndTransition = false;

    public static TransitionManager Instance;
    private static readonly int SkipEndTransition = Animator.StringToHash("SkipEndTransition");
    private static readonly int Start = Animator.StringToHash("Start");

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        if (skipEndTransition)
        {
            transition.SetBool(SkipEndTransition, true);
        }
    }

    public void StartGame()
    {
        StartCoroutine(LoadScene("Pathfinding_1.0"));
    }

    public void BackToMainMenu()
    {
        StartCoroutine(MainMenu("MainMenu"));
    }

    public void StartEnigme(string _name)
    {
        StartCoroutine(LoadScene(_name));
    }
    
    public void BackToExploration()
    {
        StartCoroutine(MainMenu("Pathfinding_1.0"));
    }

    IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger(Start);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator MainMenu(string sceneName)
    {
        transitionToMainMenu.SetTrigger(Start);
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(sceneName);
    }

}
