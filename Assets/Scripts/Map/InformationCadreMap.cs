using UnityEngine;

public class InformationCadreMap : MonoBehaviour
{
    [SerializeField] private SpriteRenderer material;
    [SerializeField] private SpriteRenderer locked;

    public SpriteRenderer Material
    {
        get => material;
        set => material = value;
    }
    
    public SpriteRenderer Locked
    {
        get => locked;
        set => locked = value;
    }
}
