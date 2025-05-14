using TMPro;
using UnityEngine;

public class CharacterWobble : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMesh;

    private Mesh mesh;

    private Vector3[] vertices;

    private void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (int i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo c = textMesh.textInfo.characterInfo[i];

            int index = c.vertexIndex;

            Vector3 offset = Wobble(Time.time + i) * 5f;
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    private Vector2 Wobble(float time) {
        return new Vector2(Mathf.Sin(time*3.5f), Mathf.Cos(time*2.7f));
    }
}