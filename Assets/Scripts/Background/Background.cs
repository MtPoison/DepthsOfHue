using UnityEngine;

public class Background : MonoBehaviour
{
    [Header("Property")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    private void Start()
    {
        FitToScreen();
    }

    private void FitToScreen()
    {
        if (Camera.main == null) return;
        
        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        Vector2 spriteSize = spriteRenderer.bounds.size;
        
        float scaleX = worldScreenWidth / spriteSize.x;
        float scaleY = worldScreenHeight / spriteSize.y;

        transform.localScale = new Vector3(scaleX + 1f, scaleY + 1f, 1);
        transform.position = Camera.main.transform.position + new Vector3(0, 0, 10);
    }
}