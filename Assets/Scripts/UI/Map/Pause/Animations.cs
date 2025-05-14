using UnityEngine;

public class Animations : MonoBehaviour
{
    private static readonly int Open = Animator.StringToHash("Open");
    private static readonly int Close = Animator.StringToHash("Close");
    [SerializeField] private Animator animator;
    [SerializeField] private AnimScale animScale;

    private void OnEnable()
    {
        animator.ResetTrigger(Close);
        animator.SetTrigger(Open);
    }
    
    public void OnHiddeAnimation()
    {
        animator.ResetTrigger(Open);
        animator.SetTrigger(Close);
    }
    
    public void OnHiddePauseCanvasFunc()
    {
        animScale.SetHiddePauseCanvas();
    }
}
