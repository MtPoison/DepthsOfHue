using UnityEngine;
using UnityEngine.Rendering;

public class ImgBackGroundEnigme : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private GameObject Go;

    private void Start()
    {
        mainCamera = Camera.main;
        SetSizeImage();
    }

    private void SetSizeImage()
    {
        Vector2 screenSize = GetScreenSizeInUnits();
        Vector3 position = new Vector3(screenSize.x, screenSize.y, 0);

        Vector3 scale = gameObject.transform.localScale;
        scale.x = screenSize.x / sprite.bounds.size.x;
        scale.y = screenSize.y / sprite.bounds.size.y;
        gameObject.transform.localScale = scale;
    }

    private Vector2 GetScreenSizeInUnits()
    {
        float height = mainCamera.orthographicSize * 2f;
        float width = height * mainCamera.aspect;
        return new Vector2(width, height);
    }

    public void UpdatePosition(Vector3 position)
    {
        Go.transform.position = position;
    }
}
