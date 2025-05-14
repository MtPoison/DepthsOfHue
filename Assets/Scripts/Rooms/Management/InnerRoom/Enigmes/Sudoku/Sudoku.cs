using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public struct SudokuPlay
{
    [Range(1, 4)]
    public int countPiece;
    public ColorSudoku color;
}

public enum ColorSudoku
{
    None,  // 0
    Red,   // 1
    Purple,  // 2
    Blue,// 3
    Yellow // 4
}
public class Sudoku : Enigme
{
    [SerializeField] private GameObject SudokuCanvas;

    public static Sudoku Instance;
    
    private SudokuPlay[,] sudokuGrid;
    [SerializeField] private int gridSize = 4;
    [SerializeField] private GameObject cellPrefab;
    

    [SerializeField] private GameObject startPosition;

    private int currentCount = 0;
    private int colorId = 0;
    [SerializeField] private TMP_Text choiceCount;
    [SerializeField] private Image choiceColorImage;
    [SerializeField] private Image resultChoiceImage;

    [SerializeField]
    private List<Color> colorList = new List<Color>();

    [SerializeField] private Color unmodifiableColor;
    [SerializeField]
    private List<Sprite> countSpriteList = new List<Sprite>();

    [SerializeField] private int numberOfBlankCases = 8;
    [SerializeField] private GameObject canvaEnigme;
    [SerializeField] private AudioClip clipSuccess;

    private SudokuGenerator sudokuGenerator;
    private SudokuPlay[,] fullSolution;

    #region Event

    public delegate void SendSoundSuccess(AudioClip _clip);
    public static event SendSoundSuccess OnSendSoundEffect;

    #endregion

    private void Start()
    {
        startPosition.SetActive(false);
        hintLeft = numberOfBlankCases;
    }

    public override void Initialize()
    {
        SudokuCanvas.SetActive(true);
        canvaEnigme.SetActive(false);

        if (Instance == null)
        {
            Instance = this;
        }
        base.Initialize();
        
        sudokuGenerator = GetComponent<SudokuGenerator>();
        if (fullSolution != null) return;
        fullSolution = sudokuGenerator.CreateSolvedGrid();

        sudokuGrid = new SudokuPlay[gridSize, gridSize];
        System.Random rand = new System.Random();

        int totalCells = gridSize * gridSize;

        List<(int, int)> allPositions = new List<(int, int)>();
        for (int i = 0; i < gridSize; i++)
        for (int j = 0; j < gridSize; j++)
            allPositions.Add((i, j));
        
        for (int i = allPositions.Count - 1; i > 0; i--)
        {
            int j = rand.Next(i + 1);
            (allPositions[i], allPositions[j]) = (allPositions[j], allPositions[i]);
        }
        
        HashSet<(int, int)> blankPositions = new HashSet<(int, int)>(allPositions.GetRange(0, numberOfBlankCases));


        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (blankPositions.Contains((i, j)))
                {
                    sudokuGrid[i, j] = new SudokuPlay
                    {
                        countPiece = 0,
                        color = ColorSudoku.None
                    };
                }
                else
                {
                    sudokuGrid[i, j] = fullSolution[i, j];
                }
            }
        }


        InstantiateGrid();
        startPosition.SetActive(true);
    }

    
    private void InitializeGrid()
    {
        sudokuGrid = new SudokuPlay[gridSize,gridSize];
    }

    public bool IsGameWon()
    {
        if (sudokuGrid == null) return false;

        for (int i = 0; i < gridSize; i++)
        {
            var numbersInRow = new HashSet<int>();
            var numbersInCol = new HashSet<int>();
            var colorsInRow = new HashSet<ColorSudoku>();
            var colorsInCol = new HashSet<ColorSudoku>();

            for (int j = 0; j < gridSize; j++)
            {
                SudokuPlay rowCell = sudokuGrid[i, j];
                if (rowCell.countPiece < 1 || rowCell.countPiece > 4 ||
                    !numbersInRow.Add(rowCell.countPiece) ||
                    !colorsInRow.Add(rowCell.color))
                {
                    return false;
                }

                SudokuPlay colCell = sudokuGrid[j, i];
                if (colCell.countPiece < 1 || colCell.countPiece > 4 ||
                    !numbersInCol.Add(colCell.countPiece) ||
                    !colorsInCol.Add(colCell.color))
                {
                    return false;
                }

                if (rowCell.color == ColorSudoku.None || colCell.color == ColorSudoku.None)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void PlayHand(int row, int col, CellSudoku cell)
    {
        if (row >= 0 && row < gridSize && col >= 0 && col < gridSize)
        {
            SudokuPlay play = new SudokuPlay();
            play.countPiece = currentCount;
            play.color = (ColorSudoku)colorId;
            if (play.countPiece == 0 && play.color != ColorSudoku.None)
            {
                play.countPiece = sudokuGrid[row, col].countPiece;
            }
            else if (play.countPiece != 0 && play.color == ColorSudoku.None)
            {
                play.color = sudokuGrid[row, col].color;
            }
            sudokuGrid[row, col] = play;
            ChangeColorCell(cell,play.countPiece,(int)play.color);
            ResetChoice();
            CheckWin();
        }
    }

    private void ChangeColorCell(CellSudoku cell, int countPiece, int colorId)
    {
        GameObject child = cell.transform.GetChild(1).gameObject;
        child.GetComponent<Image>().color = colorList[colorId];
        //cell.GetComponent<Image>().color = colorList[colorId];
        if (countPiece != 0)
        {
            child.SetActive(true);
            child.GetComponent<Image>().sprite = countSpriteList[countPiece-1];
            cell.GetComponent<Image>().color = colorList[0];
            cell.GetComponentInChildren<TMP_Text>().text = "";
            //cell.GetComponentInChildren<TMP_Text>().text = countPiece.ToString();
        }
        else
        {
            child.SetActive(false);
            child.GetComponent<Image>().sprite = null;
            cell.GetComponent<Image>().color = colorList[colorId];
            //cell.GetComponentInChildren<TMP_Text>().text = "";
        }
    }

    public void InstantiateGrid()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                cell.transform.SetParent(startPosition.transform);
                var cellScript = cell.GetComponent<CellSudoku>();
                cellScript.x = i;
                cellScript.y = j;

                int colorIndex = (int)sudokuGrid[i, j].color;

                GameObject child = cell.transform.GetChild(1).gameObject;

                var cellImage = child.GetComponent<Image>();
                if (colorIndex >= 0 && colorIndex < colorList.Count)
                {
                    cellImage.color = colorList[colorIndex];
                }
                else
                {
                    cellImage.color = Color.white;
                }

                //var text = cell.GetComponentInChildren<TMP_Text>();

                if (sudokuGrid[i, j].countPiece != 0)
                {
                    //text.text = sudokuGrid[i, j].countPiece.ToString();
                    child.SetActive(true);
                    cellImage.sprite = countSpriteList[sudokuGrid[i, j].countPiece-1];
                    cellScript.isEditable = false;
                    cell.GetComponent<Image>().color = unmodifiableColor;
                    //cellScript.UpdateNotEditable();
                }
                else
                {
                    //text.text = "";
                    cellScript.isEditable = true; 
                }
            }
        }
    }


    public void ChangePlay(int buttonId, bool isColor)
    {
        GameObject child = resultChoiceImage.transform.GetChild(0).gameObject;
        if (isColor)
        {
            colorId = buttonId;
            choiceColorImage.color = colorList[colorId];
            
            if (currentCount > 0)
            {
                child.GetComponent<Image>().color = colorList[colorId];
            }
            else
            {
                resultChoiceImage.color = colorList[colorId];
            }
            
        }
        else
        {
            currentCount = buttonId;
            child.GetComponent<Image>().color = colorList[colorId];
            resultChoiceImage.color = colorList[0];
            if (currentCount == 0)
            {
                child.GetComponent<Image>().sprite = null;
                choiceCount.text = "";
                child.SetActive(false);
                resultChoiceImage.color = colorList[colorId];
            }
            else
            {
                child.GetComponent<Image>().sprite = countSpriteList[currentCount-1];
                choiceCount.text = currentCount.ToString();
                child.SetActive(true);
            }
        }
    }

    private void ResetChoice()
    {
        currentCount = 0;
        colorId = 0;
        ChangePlay(0,false);
        ChangePlay(0,true);
    }
    
    public void ProvideHint(GameObject button)
    {
        List<CellSudoku> hintableCells = new List<CellSudoku>();
    
        foreach (Transform child in startPosition.transform)
        {
            CellSudoku cell = child.GetComponent<CellSudoku>();
            if (cell.isEditable)
            {
                SudokuPlay currentPlay = sudokuGrid[cell.x, cell.y];
                SudokuPlay correctPlay = fullSolution[cell.x, cell.y];
                
                if (currentPlay.countPiece == 0 || 
                    currentPlay.countPiece != correctPlay.countPiece || 
                    currentPlay.color != correctPlay.color)
                {
                    hintableCells.Add(cell);
                }
            }
        }

        if (hintableCells.Count == 0)
        {
            Debug.Log("No cells need hints - all editable cells are correct!");
            button.GetComponent<Button>().interactable = false;
            return;
        }
        
        System.Random rand = new System.Random();
        CellSudoku hintCell = hintableCells[rand.Next(hintableCells.Count)];
        SudokuPlay correctPlayToFill = fullSolution[hintCell.x, hintCell.y];
        
        sudokuGrid[hintCell.x, hintCell.y] = correctPlayToFill;
        GameObject childCell = hintCell.transform.GetChild(1).gameObject;
        
        childCell.GetComponent<Image>().color = colorList[(int)correctPlayToFill.color];
        childCell.GetComponent<Image>().sprite = countSpriteList[correctPlayToFill.countPiece-1];
        childCell.SetActive(true);
        hintCell.isEditable = false;
        hintCell.GetComponent<Image>().color = unmodifiableColor;
        //hintCell.UpdateNotEditable();
        CheckWin();
    }

    private void CheckWin()
    {
        if (!IsGameWon())
        {
            if(IsBoardFull())
                DialogueManager.Instance.StartNewDialogue(2,DialogueGroupKey.sudokuLike);
            return;
        }
        Debug.Log("You won the game!");
        if (clipSuccess) OnSendSoundEffect?.Invoke(clipSuccess);
        canvaEnigme.SetActive(true);
        SudokuCanvas.SetActive(false);
        DialogueManager.Instance.StartNewDialogue(1,DialogueGroupKey.sudokuLike);
        Success();
    }

    private bool IsBoardFull()
    {
        foreach (var sudokuPlay in sudokuGrid)
        {
            if (sudokuPlay.countPiece == 0 || sudokuPlay.color == ColorSudoku.None)
            {
                return false;
            }
        }
        return true;
    }
    public void Quit()
    {
        SudokuCanvas.SetActive(false);
        canvaEnigme.SetActive(true);
    }
}
