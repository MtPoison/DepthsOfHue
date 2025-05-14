using UnityEngine;

public class RopeLine : MonoBehaviour
{
    [SerializeField] private Rope2DCreator rope;
    [SerializeField] private LineRenderer line;

    private void Awake()
    {
        line.enabled = true;
        line.positionCount = rope.segments.Length;
    }

    private void Update()
    {
        if (rope.segments.Length <= 0) return;
        
        line.positionCount = rope.segments.Length;
        for (int i = 0; i < rope.segments.Length; i++)
        {
            line.SetPosition(i, rope.segments[i].position);
        }
    }
}
