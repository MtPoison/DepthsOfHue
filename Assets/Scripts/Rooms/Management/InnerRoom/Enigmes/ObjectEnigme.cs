using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ObjectEnigme : MonoBehaviour
{

    //[SerializeField] private int DoubleID;

    //[ContextMenu("Debug")]
    //public void OnObjectClicked(GameObject )
    //{
    //    if (Enigme_FindObjects.Instance != null)
    //    {
    //        Enigme_FindObjects.Instance.CheckItem(this);
    //    }
    //    else if(Enigme_Doble.Instance != null)
    //    {
    //        enigmeD.ObjectClicked(gameObject);
    //    }
    //    else
    //    {
    //        Debug.Log("no instance");
    //    }


    //}
    //public int GetId() { return ID; }


    public void MoveFragment(Vector3 targetPosition, Vector3 finalScale)
    {
        Vector3 start = transform.position;
        float duration = 2f;

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
            transform.position = center + offset;

        }, 360f * spinCount, duration).SetEase(Ease.InOutSine);

        // Scale
        transform.DOScale(finalScale, duration).SetEase(Ease.OutBack);

        // Rotation sur lui-même
        transform.DORotate(new Vector3(360f, 360f, 360f), duration, RotateMode.FastBeyond360)
                 .SetEase(Ease.InOutSine);
    }
}
