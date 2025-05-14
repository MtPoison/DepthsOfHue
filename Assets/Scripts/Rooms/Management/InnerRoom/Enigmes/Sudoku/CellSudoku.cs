using UnityEngine;

public class CellSudoku : MonoBehaviour
{
    public int x;

    public int y;
    
    public bool isEditable = true;

    [SerializeField] private GameObject notEditableImage;
    [SerializeField] private AudioClip clip;

    #region Event

    public delegate void SendSoundEffect(AudioClip _clip);
    public static event SendSoundEffect OnSendSoundEffect;

    #endregion

    public void UpdateNotEditable()
    {
        if (!isEditable)
        {
            notEditableImage.SetActive(true);
        }
           
    }

    public void Test()
    {
        Debug.Log(x +" , " + y);
    }

    public void SetPosition(int _x, int _y)
    {
        x = _x;
        y = _y;
    }

    public void GetCellPosition(out int _x, out int _y)
    {
        _x = x;
        _y = y;
    }

    public void PlayChoice()
    {
        if (isEditable)
        {
            Sudoku.Instance.PlayHand(x, y, this);
            if (clip) OnSendSoundEffect?.Invoke(clip);
        }
        else
        {
            Debug.Log("This cell is not editable.");
        }
    }
}
