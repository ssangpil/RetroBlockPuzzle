using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[Serializable]
public class ShapeData : ScriptableObject
{
    [Serializable]
    public class Row
    {
        public bool[] column;
        private int _size = 0;

        public Row(int size)
        {
            CreateRow(size);
        }

        public void CreateRow(int size)
        {
            _size = size;
            column = new bool[_size];
            ClearRow();
        }

        public void ClearRow()
        {
            for (var i = 0; i < _size; i++)
            {
                column[i] = false;
            }
        }
    }

    public int id = 0;
    public int columns = 0;
    public int rows = 0;
    public Row[] board;

    public void Clear()
    {
        for (var i = 0; i < rows; i++)
        {
            board[i].ClearRow();
        }
    }

    public void CreateNewBoard()
    {
        board = new Row[rows];

        for (var i = 0; i < rows; i++)
        {
            board[i] = new Row(columns);
        }
    }

    public List<int> GetSquareIndexes()
    {
        var list = new List<int>();
        var squareIndex = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                if (board[row].column[column])
                {
                    list.Add(squareIndex);
                }

                squareIndex++;
            }
        }
        return list;
    }

    public override string ToString()
    {
        var list = new List<int>();
        var index = 0;
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                if (board[row].column[column])
                {
                    list.Add(index);
                }

                index++;
            }            
        }
        return $"[{string.Join(",", list)}]";
    }
}
