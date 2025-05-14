using UnityEngine;

public class GetShowMap : MonoBehaviour
{
    [SerializeField] private AnimScale showMap;

    public void ResetTriggerFunc()
    {
        showMap.ResetTrigger();
    }
    
    public void PlaySoundInAnimation()
    {
        showMap.PlaySoundBtn();
    }
}
