using System;
using System.Collections.Generic;
using UnityEngine;

public class SudokuGenerator : MonoBehaviour
{
    private SudokuPlay[,] grid = new SudokuPlay[4, 4];
    private System.Random random = new System.Random();

    public SudokuPlay[,] CreateSolvedGrid()
    {
        if (!SolveSudoku())
        {
            Debug.LogError("No solution exists!");
            return null;
        }
        
        RandomizeSolution();

        PrintGrid();
        return grid;
    }
    
    private bool SolveSudoku()
    {
        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
                grid[i, j] = new SudokuPlay { countPiece = 0, color = ColorSudoku.None };

        return SolveSudokuRecursive();
    }

    private bool SolveSudokuRecursive()
    {
        int row = -1, col = -1;
        List<(int num, ColorSudoku color)> options = null;

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                if (grid[i, j].countPiece != 0) continue;

                var cellOptions = GetPossibleOptions(i, j);
                if (cellOptions.Count == 0) return false;

                if (options == null || cellOptions.Count < options.Count)
                {
                    row = i;
                    col = j;
                    options = cellOptions;
                }
            }

        if (row == -1) return true; 
        
        foreach (var (num, color) in options)
        {
            grid[row, col].countPiece = num;
            grid[row, col].color = color;

            if (SolveSudokuRecursive())
                return true;
            
            grid[row, col].countPiece = 0;
            grid[row, col].color = ColorSudoku.None;
        }

        return false;
    }

    private List<(int num, ColorSudoku color)> GetPossibleOptions(int row, int col)
    {
        var options = new List<(int, ColorSudoku)>();
        
        for (int num = 1; num <= 4; num++)
        {
            if (!IsNumberValid(row, col, num)) continue;
            
            foreach (ColorSudoku color in Enum.GetValues(typeof(ColorSudoku)))
            {
                if (color == ColorSudoku.None) continue;
                if (IsColorValid(row, col, color))
                    options.Add((num, color));
            }
        }
        
        ShuffleOptions(options);

        return options;
    }

    private void ShuffleOptions(List<(int num, ColorSudoku color)> options)
    {
        int n = options.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (options[i], options[j]) = (options[j], options[i]);
        }
    }

    private bool IsNumberValid(int row, int col, int number)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != col && grid[row, i].countPiece == number) return false;
            if (i != row && grid[i, col].countPiece == number) return false;
        }
        return true;
    }

    private bool IsColorValid(int row, int col, ColorSudoku color)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i != col && grid[row, i].color == color) return false;
            if (i != row && grid[i, col].color == color) return false;
        }
        return true;
    }
    
    private void RandomizeSolution()
    {
        int[] numberMap = GenerateRandomPermutation(4);
        ColorSudoku[] colorMap = GenerateRandomColorPermutation();

        for (int i = 0; i < 4; i++)
            for (int j = 0; j < 4; j++)
            {
                grid[i, j].countPiece = numberMap[grid[i, j].countPiece - 1] + 1;
                grid[i, j].color = colorMap[(int)grid[i, j].color - 1];
            }
        
        ShuffleRowsAndColumns();
    }

    private int[] GenerateRandomPermutation(int n)
    {
        int[] arr = new int[n];
        for (int i = 0; i < n; i++) arr[i] = i;
        for (int i = n - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    private ColorSudoku[] GenerateRandomColorPermutation()
    {
        var colors = new List<ColorSudoku> { ColorSudoku.Red, ColorSudoku.Blue, ColorSudoku.Purple, ColorSudoku.Yellow };
        for (int i = colors.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (colors[i], colors[j]) = (colors[j], colors[i]);
        }
        return colors.ToArray();
    }

    private void ShuffleRowsAndColumns()
    {
        for (int i = 3; i > 0; i--)
        {
            int j = random.Next(i + 1);
            for (int col = 0; col < 4; col++)
            {
                (grid[i, col], grid[j, col]) = (grid[j, col], grid[i, col]);
            }
        }
        
        for (int i = 3; i > 0; i--)
        {
            int j = random.Next(i + 1);
            for (int row = 0; row < 4; row++)
            {
                (grid[row, i], grid[row, j]) = (grid[row, j], grid[row, i]);
            }
        }
    }
    
    private void PrintGrid()
    {
        string output = "Randomized Valid Solution:\n";
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
                output += $"{grid[i, j].countPiece}/{grid[i, j].color}".PadRight(15);
            output += "\n";
        }
        Debug.Log(output);
    }

}