
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BoardSquare
{
    public int SquareIndex;
    public bool Selected;
    public bool SquareOccupied;
}

public class BoardBlock
{
    public int ID;
    public ShapeData ShapeData;
    public List<int[]> CanBePlacedList;
}

public class BoardBlockGroup
{
    public int ID;
    public int Prob;
    public List<BoardBlock> Blocks;
}

public class Board
{
    public List<BoardSquare> Squares => m_Squares;    

    private int m_Columns;
    private int m_Rows;
    private List<BoardSquare> m_Squares = new List<BoardSquare>();
    private List<ShapeData> m_UsableShapeData = new List<ShapeData>();
    private int[,] m_LineData;
    private int[] m_ColumnSquareIndexes;
    
    public Board(Grid grid)
    {
        m_Columns = grid.columns;
        m_Rows = grid.rows;
        foreach (var square in grid.gridSquares)
        {
            var comp = square.GetComponent<GridSquare>();
            m_Squares.Add(new BoardSquare
            {
                Selected = comp.Selected,
                SquareIndex = comp.SquareIndex,                
                SquareOccupied = comp.SquareOccupied,
            });
        }

        m_LineData = new int[m_Rows, m_Columns];
        var index = 0;
        for (var row = 0; row < m_Rows; row++)
        {
            for (var column = 0; column < m_Columns; column++)
            {
                m_LineData[row, column] = index++;
            }
        }

        index = 0;
        m_ColumnSquareIndexes = new int[m_Columns];
        for (var column = 0; column < m_Columns; column++)
        {
            m_ColumnSquareIndexes[column] = index++;
        }

        m_UsableShapeData.AddRange(GameManager.Instance.shapeDataList);
    }

    public Board(Board board)
    {
        m_Rows = board.m_Rows;
        m_Columns = board.m_Columns;
        foreach (var square in board.m_Squares)
        {
            m_Squares.Add(new BoardSquare
            {
                SquareIndex = square.SquareIndex,
                SquareOccupied = square.SquareOccupied
            });
        }

        m_LineData = new int[m_Rows, m_Columns];
        for (var row = 0; row < m_Rows; row++)
        {
            for (var column = 0; column < m_Columns; column++)
            {
                m_LineData[row, column] = board.m_LineData[row, column];
            }
        }

        m_ColumnSquareIndexes = new int[m_Columns];
        for (var column = 0; column < m_Columns; column++)
        {
            m_ColumnSquareIndexes[column] = board.m_ColumnSquareIndexes[column];
        }

        m_UsableShapeData.AddRange(board.m_UsableShapeData);
    }

    public static void AllocateShapeData(Grid grid, List<Shape> shapeList)
    {
        var board = new Board(grid);

        var randomProb = Random.Range(0, 10000);
        var isBonus = randomProb < Config.RBBlockSpawnRate;
        var shapeIndex = Random.Range(0, shapeList.Count);

        for (var i = 0; i < shapeList.Count; i++)
        {
            var shape = shapeList[i];
            var boardShape = board.RandomBoardShape();
            if (null == boardShape)
            {
                Debug.LogError("Not found board...");
                return;
            }

            board.PlaceShapeOnBoard(boardShape);

            var textureData = GameManager.Instance.squareTextureData.NextShapeTextureData();
            shape.Create(boardShape.ShapeData, textureData);
            shape.ActivateShape();
            if (isBonus && shapeIndex == i)
            {
                var squareIndex = Random.Range(0, shape.TotalSquareCnt);
                shape.SetBonus(squareIndex);
            }

            Debug.Log($"i={i}, shapeId={boardShape.ShapeData.id}, shapeData={boardShape.ShapeData}");
        }
    }

      private BoardBlock RandomBoardShape()
    {
        var boardBlockGroups = new Dictionary<int, BoardBlockGroup>();
        var boardBlocks = new List<BoardBlock>();
        foreach (var shapeData in m_UsableShapeData)
        {
            var placed = SearchCanBePlacedOnGrid(shapeData);
            if (placed.Any())
            {
                boardBlocks.Add(new BoardBlock { ID = shapeData.id, ShapeData = shapeData, CanBePlacedList = placed });
            }
        }

        if (!boardBlocks.Any())
        {
            foreach (var shapeData in m_UsableShapeData)
            {
                boardBlocks.Add(new BoardBlock { ID = shapeData.id, ShapeData = shapeData });
            }
        }

        foreach (var boardBlock in boardBlocks)
        {
            var blockGroupId = ReferenceManager.Instance.GetBlockGroupID(boardBlock.ID);
            if (!boardBlockGroups.TryGetValue(blockGroupId, out var boardBlockGroup))
            {
                boardBlockGroups.Add(blockGroupId, 
                    new BoardBlockGroup 
                    {
                        ID = blockGroupId, 
                        Prob = CalcBlockGroupProb(blockGroupId), 
                        Blocks = new List<BoardBlock> { boardBlock }
                    });
            }
            else
            {
                boardBlockGroup.Blocks.Add(boardBlock);
            }
        }

        var totalProb = boardBlockGroups.Values.Sum(x => x.Prob);
        var randomProb = Random.Range(0, totalProb);
        var accumulateProb = 0;

        foreach (var boardBlockGroup in boardBlockGroups.Values)
        {
            accumulateProb += boardBlockGroup.Prob;
            if (randomProb <= accumulateProb)
            {
                //m_UsableShapeData.Remove(shape.ShapeData);

                var randomIndex = Random.Range(0, boardBlockGroup.Blocks.Count);
                var boardBlock = boardBlockGroup.Blocks[randomIndex];

                // if (0 == index)
                //     Debug.Log($"blockGroupId={boardBlockGroup.ID}, accumulateProb={accumulateProb}, randomProb={randomProb}, shapeId={boardBlock.ShapeData.id}");

                return boardBlock;
            }
        }    

        return null;
    }

    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0; 

        var safeIndex = 0;
        while (lastRowIndex + (rows - 1) < 8)
        {
            var rowData = new List<int>();
            for (var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(m_LineData[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;
            if (lastColumnIndex + (columns - 1) >= 8)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }
        }

        return squareList;
    }

    private List<int> GetShapeActivedIndexes(ShapeData shapeData)
    {
        var result = new List<int>();
        var squareIndex = 0;
        for (var rowIndex = 0; rowIndex < shapeData.rows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < shapeData.columns; columnIndex++)
            {
                if (shapeData.board[rowIndex].column[columnIndex])
                {
                    result.Add(squareIndex);
                }

                squareIndex++;
            }
        }
        return result;
    }

    private List<int[]> SearchCanBePlacedOnGrid(ShapeData shapeData)
    {
        var shapeColumns = shapeData.columns;
        var shapeRows = shapeData.rows;

        // 활성화 되어 있는 블럭에 대한 인덱스 번호
        var filledUpSquareIndexes = GetShapeActivedIndexes(shapeData);
        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);
        var canBePlacedList = new List<int[]>();
        foreach (var number in squareList)
        {
            var canBePlaced = true;
            foreach (var squareIndexToCheck in filledUpSquareIndexes)
            {
                if (m_Squares[number[squareIndexToCheck]].SquareOccupied)
                {
                    canBePlaced = false;
                }
            }

            if (canBePlaced)
            {
                canBePlacedList.Add(number);
            }
        }

        return canBePlacedList;
    }

    private List<int[]> SearchCanBePlacedAndLineCompletedOnGrid(ShapeData shapeData)
    {
        var shapeColumns = shapeData.columns;
        var shapeRows = shapeData.rows;

        // 활성화 되어 있는 블럭에 대한 인덱스 번호
        var shapeActivedIndexes = GetShapeActivedIndexes(shapeData);
        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);

        var lineCompleted = new List<int[]>();
        foreach (var number in squareList)
        {
            if (shapeActivedIndexes.All(x => !m_Squares[number[x]].SquareOccupied))
            {
                // 놓을 수 있음 
                var clone = new Board(this);
                foreach (var index in shapeActivedIndexes)
                {
                    clone.m_Squares[number[index]].SquareOccupied = true;
                }

                var lines = clone.GetCanBeLineCompleted();
                if (lines.Any())
                {
                    lineCompleted.Add(number);
                }
            }
        }

        return lineCompleted;
    }

    private List<int[]> GetCanBeLineCompleted()
    {
        var lines = new List<int[]>();
        for (var row = 0; row < m_Rows; row++)
        {
            var isCompleted = true;
            for (var column = 0; column < m_Columns; column++)
            {
                if (!m_Squares[m_LineData[row, column]].SquareOccupied)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                lines.Add(GetHorizontalLine(row));
            }
        }

        for (var column = 0; column < m_Columns; column++)
        {
            var isCompleted = true;
            for (var row = 0; row < m_Rows; row++)
            {
                if (!m_Squares[m_LineData[row, column]].SquareOccupied)
                {
                    isCompleted = false;
                }
            }

            if (isCompleted)
            {
                lines.Add(GetVerticalLine(column));
            }
        }

        return lines;
    }

    private void PlaceShapeOnBoard(BoardBlock shape)
    {
        if (0 == shape.CanBePlacedList.Count)
            return;

        var shapeData = shape.ShapeData;
        var rows = shapeData.rows;
        var columns = shapeData.columns;

        // 랜덤하게 한곳을 선정
        var canBePlacedList = shape.CanBePlacedList;
        var list = canBePlacedList[Random.Range(0, canBePlacedList.Count)];

        foreach (var square in m_Squares)
        {
            if (list.Contains(square.SquareIndex))
            {
                square.SquareOccupied = true;
            }
        }

        var isCheck = ReferenceManager.Instance.IsCheckCanBeLineIsCompleted(GameManager.Instance.TurnCnt);
        if (isCheck)
        {
            var lines = new List<int[]>();
            for (var row = 0; row < rows; row++)
            {
                lines.Add(GetHorizontalLine(row));
            }

            for (var column = 0; column < columns; column++)
            {
                lines.Add(GetVerticalLine(column));
            }

            var lineCompletedList = new List<int[]>();
            foreach (var line in lines)
            {
                if (line.All(x => m_Squares[x].SquareOccupied))
                {
                    lineCompletedList.Add(line);
                }
            }

            foreach (var line in lineCompletedList)
            {
                foreach (var squareIndex in line)
                {
                    m_Squares[squareIndex].SquareOccupied = false;
                }
            }
        }
        
    }

    private (int, int) GetSquarePosition(int squareIndex)
    {
        for (var row = 0; row < m_Rows; row++)
        {
            for (var column = 0; column < m_Columns; column++)
            {
                if (m_LineData[row, column] == squareIndex)
                {
                    return (row, column);
                }
            }
        }

        return (-1, -1);
    }

    private int[] GetVerticalLine(int column)
    {
        var line = new int[m_Rows];

        for (var row = 0; row < m_Rows; row++)
        {
            line[row] = m_LineData[row, column];
        }

        return line;
    }

    private int[] GetHorizontalLine(int row)
    {
        var line = new int[m_Columns];
        for (var column = 0; column < m_Columns; column++)
        {
            line[column] = m_LineData[row, column];
        }

        return line;
    }

    private int CalcBlockGroupProb(int blockGroupId)
    {
        // 빈칸 개수
        var blankCnt = GetBlankSquareOnBoard();
        // 가중치
        var blankValue = ReferenceManager.Instance.GetBlankValue(blockGroupId, blankCnt);

        var refBlockGroup = ReferenceManager.Instance.FindRefBlockGroup(blockGroupId);
        var blockProb = (int)(refBlockGroup.Prob * blankValue);
        //Debug.Log($"blockGroupId={blockGroupId}, blankCnt={blankCnt}, blankValue={blankValue}, blockProb={blockProb}");
        return blockProb;
    }

    private int GetBlankSquareOnBoard()
    {
        var blankCnt = 0;        
        foreach (var square in m_Squares)
        {
            if (!square.SquareOccupied)
            {
                blankCnt++;
            }
        }
        return blankCnt;
    }
}