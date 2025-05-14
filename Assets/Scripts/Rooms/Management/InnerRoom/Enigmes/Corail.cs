using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corail : MonoBehaviour
{
    [SerializeField] private AudioClip baseMelodySound;

    public AudioClip GetAudioClip() { return baseMelodySound; }


    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowIntensity = 2f;

    private List<Material> coralMaterials = new List<Material>();
 

    public List<Material> GetMaterials() { return coralMaterials; }

    private Renderer[] coralRenderers;

    void Start()
    {

        // Récupère tous les Renderers dans les enfants
        coralRenderers = GetComponentsInChildren<Renderer>();


        if (coralRenderers.Length == 0)
        {
            return;
        }

        // Stocke les materials pour les modifier plus tard
        foreach (Renderer rend in coralRenderers)
        {
            coralMaterials.Add(rend.material); // .material instancie une copie du material si nécessaire
        }
    }
}
