using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private bool launchTimer;
    private float timer;
    [SerializeField] private Inventaire inventaire;
    [SerializeField] private UI_Inventaire inv;
    [SerializeField] private ParticleSystem part;
    private AudioSource audioSource;
    void Start()
    {
        part.Stop();
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Appuie sur Espace pour jouer le son
        {
            PlayAudio();
        }

    }

    public void OnObjectClicked()
    {
        
        string gameObjectName = gameObject.name.ToLower();

        ItemData foundItem = inventaire.GetItems().Find(item => item.itemName.ToLower().Contains(gameObjectName));

        if (foundItem != null)
        {
            inventaire.Add( foundItem, gameObject);
        }
        else
        {
            // Si aucun élément correspondant n'est trouvé
            Debug.Log("Aucun élément trouvé avec un nom similaire à : " + gameObjectName);
        }
        
    }



    public void PlayAudio()
    {
        if (audioSource != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
