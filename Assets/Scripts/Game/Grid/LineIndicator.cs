using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineIndicator : MonoBehaviour
{
    public int[,] lineData;
    [HideInInspector] public int[] columnSquareIndexes;
    
    private Grid m_Grid;

    void Awake()
    {
        m_Grid = GetComponent<Grid>();
        lineData = new int[m_Grid.rows, m_Grid.columns];
        var index = 0;
        for (var row = 0; row < m_Grid.rows; row++)
        {
            for (var column = 0; column < m_Grid.columns; column++)
            {
                lineData[row, column] = index++;
            }
        }

        index = 0;
        columnSquareIndexes = new int[m_Grid.columns];
        for (var column = 0; column < m_Grid.columns; column++)
        {
            columnSquareIndexes[column] = index++;
        }
    }

    public int CheckVerticalLine(int[] verticalLine)
    {
        for (var column = 0; column < m_Grid.columns; column++)
        {
            var line = new int[m_Grid.rows];
            for (var row = 0; row < m_Grid.rows; row++)
            {
                line[row] = lineData[row, column];
            }

            var completed = true;
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] != verticalLine[i])
                {
                    completed = false;
                }
            }

            if (completed)
            {
                return column;
            }
        }  

        return -1;
    }

    public int CheckHorizontalLine(int[] horizontalLine)
    {
        for (var row = 0; row < m_Grid.rows; row++)
        {
            var line = new int[m_Grid.columns];
            for (var column = 0; column < m_Grid.columns; column++)
            {
                line[column] = lineData[row, column];
            }

            var completed = true;
            for (var i = 0; i < line.Length; i++)
            {
                if (line[i] != horizontalLine[i])
                {
                    completed = false;
                }
            }

            if (completed)
            {
                return row;
            }
        }

        return -1;
    }

    public (int, int) GetSquarePosition(int squareIndex)
    {
        var pos_row = -1;
        var pos_col = -1;

        for (var row = 0; row < 8; row++)
        {
            for (var column = 0; column < 8; column++)
            {
                if (lineData[row, column] == squareIndex)
                {
                    pos_row = row;
                    pos_col = column;
                }
            }
        }

        return (pos_row, pos_col);
    }

    public List<int[]> GetTotalVerticalLines()
    {
        var verticalLines = new List<int[]>();
        for (var column = 0; column < m_Grid.columns; column++)
        {
            var line = new int[m_Grid.rows];
            for (var row = 0; row < m_Grid.rows; row++)
            {
                line[row] = lineData[row, column];
            }
            verticalLines.Add(line);
        }        

        return verticalLines;
    }

    public List<int[]> GetTotalHorizontalLines()
    {
        var horizontalLines = new List<int[]>();
        for (var row = 0; row < m_Grid.rows; row++)
        {
            var line = new int[m_Grid.columns];
            for (var column = 0; column < m_Grid.columns; column++)
            {
                line[column] = lineData[row, column];
            }
            horizontalLines.Add(line);
        }
        return horizontalLines;
    }

    public int[] GetVerticalLine(int squareIndex)
    {
        var line = new int[m_Grid.rows];
        var column = GetSquarePosition(squareIndex).Item2;

        for (var row = 0; row < m_Grid.rows; row++)
        {
            line[row] = lineData[row, column];
        }

        return line;
    }

    public int[] GetHorizontalLine(int squareIndex)
    {
        var line = new int[m_Grid.columns];
        var row = GetSquarePosition(squareIndex).Item1;

        for (var column = 0; column < m_Grid.columns; column++)
        {
            line[column] = lineData[row, column];
        }

        return line;
    }
}
