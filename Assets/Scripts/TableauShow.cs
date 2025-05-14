using UnityEngine;

public class TableauShow : MonoBehaviour
{
    [SerializeField] private GameObject tableau;
    
    private void OnEnable()
    {
        LangueSwitch.OnShowTableau += SetShowtableau;
    }
    
    private void OnDisable()
    {
        LangueSwitch.OnShowTableau -= SetShowtableau;
    }

    private void SetShowtableau(bool _isShow)
    {
        tableau.SetActive(_isShow);
    }
}
