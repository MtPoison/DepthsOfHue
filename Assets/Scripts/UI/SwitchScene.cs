using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void SwitchSceneFunc(string _scene)
    {
        SceneManager.LoadSceneAsync(_scene);
    }
}
