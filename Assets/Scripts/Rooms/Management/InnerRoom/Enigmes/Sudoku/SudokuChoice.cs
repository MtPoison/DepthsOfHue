using UnityEngine;

public class SudokuChoice : MonoBehaviour
{
    [SerializeField] private int idButton;
    [SerializeField] private bool isColor = false;

    public void GetClickedInfo()
    {
        Sudoku.Instance.ChangePlay(idButton,isColor);
    }
}
