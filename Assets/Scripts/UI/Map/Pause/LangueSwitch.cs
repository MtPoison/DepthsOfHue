using UnityEngine;
using UnityEngine.Localization.Settings;

public class LangueSwitch : MonoBehaviour
{
    private int id;
    
    public void SwitchLangue(bool _isLeft)
    {
        if (_isLeft)
        {
            if (id <= 0) return;
            id--;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == "fr-FR");
        }
        else
        {
            if (id >= 1) return;
            id++;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales.Find(locale => locale.Identifier.Code == "en");
        }
    }
    
    // FIX TABLEAU
    #region Event

    public delegate void ShowTableau(bool _isShow);
    public static event ShowTableau OnShowTableau;

    #endregion
    
    private bool isShow = false;

    public void ShowTableauFunc()
    {
        isShow = !isShow;
        OnShowTableau?.Invoke(isShow);
    }
}
