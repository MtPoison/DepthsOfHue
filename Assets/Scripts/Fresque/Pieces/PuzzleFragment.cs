using UnityEngine;
using DG.Tweening;

public class PuzzleFragment:MonoBehaviour
{
    public int row;
    public int col;
    public string id;
    public Sprite icon;
    public ParticleSystem burst;


    public void MoveFragment( Vector3 targetPosition, Vector3 finalScale)
    {
        Vector3 start = Vector3.zero;

        transform.localScale = Vector3.zero;
        float duration = 1.5f;

        float radius = 2.5f; // plus grand = spirale plus large
        int spinCount = 2; // nombre de tours
        float angle = 0f;

        DOTween.To(() => angle, x => {
            angle = x;
            float t = angle / (360f * spinCount);

            // Interpolation linéaire de la position de base à finale
            Vector3 center = Vector3.Lerp(start, targetPosition, t);

            // Création d’un offset circulaire
            float radians = Mathf.Deg2Rad * angle;
            Vector3 offset = new Vector3(
                Mathf.Cos(radians),
                Mathf.Sin(radians),
                0f
            ) * radius * (1 - t);

            // Applique la position finale
            transform.localPosition = center + offset;

        }, 360f * spinCount, duration).SetEase(Ease.InOutSine);

        // Scale
        transform.DOScale(finalScale, duration).SetEase(Ease.OutBack);

        // Rotation sur lui-même
        transform.DORotate(new Vector3(360f, 360f, 360f), duration, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutSine);


    }
    public void Pop()
    {
        if (burst != null)
        {

            ParticleSystem instance = Instantiate(burst, transform.position, Quaternion.identity);

  
            instance.Play();


            Destroy(instance.gameObject, instance.main.duration + instance.main.startLifetime.constantMax);
        }
        else
        {
            Debug.LogWarning("Aucun ParticleSystem assigné au fragment !");
        }
    }

    public void FloatAndRotateFragment(float amplitude = 0.5f, float floatDuration = 2f, float rotationSpeed = 30f)
    {

        transform.DOLocalMoveY(transform.localPosition.y + amplitude, floatDuration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);

        transform.DORotate(new Vector3(0, 0, 360f), 360f / rotationSpeed, RotateMode.FastBeyond360)
                 .SetEase(Ease.Linear)
                 .SetLoops(-1, LoopType.Restart);
    }
}
