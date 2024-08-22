
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System.Collections;
using UnityEngine.UI;
using System;

public class Grid : MonoBehaviour
{
    public int columns = 8;
    public int rows = 8;
    public float everySquareOffset = 0.5f;    
    public GameObject gridSquare;    
    [HideInInspector] public List<GameObject> gridSquares = new List<GameObject>();

    private LineIndicator m_LineIndicator;

    void Awake()
    {
        m_LineIndicator = GetComponent<LineIndicator>();
        GenerateGrid();        
    }

    void OnDestroy()
    {
        foreach (var square in gridSquares)
        {
            Destroy(square);
        }
    }

    void Update()
    {
        CheckIfSquareCanBeHovered();
        CheckIfSquareCanBeLineIsCompleted();
    }

    public void Clear()
    {
        foreach (var square in gridSquares)
        {
            if (square.TryGetComponent<GridSquare>(out var component))
            {
                component.DeactivateSquare();
            }
        }
    }

    public GridSquare GetGridSquare(int row, int column)
    {
        var squareIndex = GetSquareIndex(row, column);
        var obj = gridSquares[squareIndex];
        return obj.GetComponent<GridSquare>();
    }

    private void GenerateGrid()
    {
        var squareIndex = 0;
        var square_rect = gridSquare.GetComponent<RectTransform>();
        var offset = new Vector2(
            square_rect.rect.width * square_rect.transform.localScale.x + everySquareOffset,
            square_rect.rect.height * square_rect.transform.localScale.y + everySquareOffset);

        var totalX = columns * offset.x;
        var totalY = rows * offset.y;
        var startPosition = new Vector2(-totalX / 2 + offset.x / 2, totalY / 2 - offset.y / 2);
        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var position = new Vector3(startPosition.x + (column * offset.x), startPosition.y - (row * offset.y), 0f);
                //Debug.Log($"row={row}, column={column}, position={position.x},{position.y}");
               
                var square = Utility.InstantiateGameObject(gridSquare, transform, position);
                square.GetComponent<GridSquare>().SquareIndex = squareIndex++;                                
                gridSquares.Add(square);
            }
        }

        //PanelManager.Instance.CreateShapes();
    }

    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.CheckIfShapeCanBeHovered += CheckIfSquareCanBeHovered;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.CheckIfShapeCanBeHovered -= CheckIfSquareCanBeHovered;
    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();
        foreach (var gridSquare in gridSquares)
        {
            var comp = gridSquare.GetComponent<GridSquare>();
            if (comp.IsHovered())
            {
                squareIndexes.Add(comp.SquareIndex);
                comp.Selected = false;
            }
        }

        // 선택중인 블록 정보
        var shape = PanelManager.Instance.GetCurrentSelectedShape();
        if (null == shape) 
            return; // there is no selected shape           

        var isCheck = false;
        var lines = GetAllSquaresCombination(shape.shapeData.columns, shape.shapeData.rows);
        foreach (var line in lines)
        {
            if (squareIndexes.All(x => line.Contains(x)))
            {
                isCheck = true;
                break;
            }
        }

        if (isCheck && shape.TotalSquareCnt == squareIndexes.Count)
        {   
            GameManager.Instance.SetTouchDefense(true);

            // 진동
            VibrationManager.Instance.VibrateOneShot();
            // 효과음
            AudioManager.Instance.PlaySfx(ESfx.Placed);

            foreach (var squareIndex in squareIndexes)
            {
                var isBonusIncluded = false;
                var textureData = shape.textureData;
                if (-1 != shape.bonusSquareIndex)
                {
                    isBonusIncluded = squareIndex == shape.squares[shape.bonusSquareIndex].GetComponent<ShapeSquare>().HoveredSquareIndex;
                    if (isBonusIncluded)
                        textureData = GameManager.Instance.squareTextureData.starTextureData;
                }
                
                var gridSquare = gridSquares[squareIndex].GetComponent<GridSquare>();
                gridSquare.PlaceShapeOnBoard(isBonusIncluded, textureData);
            }

            // 현재 선택된 블럭 비활성
            shape.DeactivateShape();

            var checkLines = new List<int[]>();
            // 세로 라인
            foreach (var squareIndex in m_LineIndicator.columnSquareIndexes)
            {
                checkLines.Add(m_LineIndicator.GetVerticalLine(squareIndex));
            }

            // 가로 라인
            for (var row = 0; row < rows; row++)
            {
                var indexes = new List<int>();
                for (var column = 0; column < columns; column++)
                {
                    indexes.Add(m_LineIndicator.lineData[row, column]);
                }

                checkLines.Add(indexes.ToArray());
            }

            if (CheckSquaresAreCompleted(checkLines, out var completedLines))
            {
                AudioManager.Instance.PlaySfx(ESfx.LineCompleted);

                var starSquares = new List<GridSquare>();
                foreach (var line in completedLines)
                {
                    foreach (var squareIndex in line)
                    {
                        var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                        if (comp.IsBonusIncluded)
                        {
                            starSquares.Add(comp);
                        }

                        comp.DeactivateSquare();
                    }

                    // 세로 이펙트
                    var vertical = m_LineIndicator.CheckVerticalLine(line);
                    if (-1 != vertical)
                    {
                        var gridSquare = gridSquares[line[3]];
                        var effect = EffectManager.Instance.ShowEffect(EEffectType.LineY, transform, new Vector3(gridSquare.transform.localPosition.x , 0f, 0f), Vector3.zero, 1f);
                        effect.SetLifeTime(3f);
                        effect.SetParticleColor(shape.textureData.squareColor.ToColor());
                    }

                    // 가로 이펙트
                    var horizontal = m_LineIndicator.CheckHorizontalLine(line);
                    if (-1 != horizontal)
                    {
                        var gridSquare = gridSquares[line[3]];
                        var effect = EffectManager.Instance.ShowEffect(EEffectType.LineX, transform, new Vector3(0f, gridSquare.transform.localPosition.y, 0f), Vector3.zero, 1f);
                        effect.SetLifeTime(3f);
                        effect.SetParticleColor(shape.textureData.squareColor.ToColor());
                    }
                }

                StarAnimation(starSquares);
                GameManager.Instance.ResultShapePlaced(squareIndexes.Count, completedLines.Count, shape.transform.position);
            }
            else
            {
                GameManager.Instance.ResultShapePlaced(squareIndexes.Count, 0, shape.transform.position);
            }
        }
        else
        {
            // 현재 블럭 시작 위치 귀환
            shape.MoveToStartPosition();
        }
    }

    private bool CheckSquaresAreCompleted(List<int[]> lines, out List<int[]> completedLines)
    {
        completedLines = new List<int[]>();
        foreach (var line in lines)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = this.gridSquares[squareIndex].GetComponent<GridSquare>();
                if (!comp.SquareOccupied)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }
        
        if (completedLines.Any())
            return true;

        completedLines = null;
        return false;
    }   


    private void StarAnimation(List<GridSquare> gridSquares)
    {
        if (!gridSquares.Any())
            return;

        var seq = DOTween.Sequence();
        var starCnt = gridSquares.Count;
        for (var i = 0; i < starCnt; i++)
        {
            var gridSquare = gridSquares[i];
            var inner = DOTween.Sequence();
            inner.AppendCallback(() => 
            {
                var obj = EffectManager.Instance.ShowEffect(EEffectType.Star, transform, gridSquare.transform.localPosition);
                obj.SetLifeTime(2f);
            });
            seq.Join(inner);
        }

        seq.AppendCallback(() => GameManager.Instance.GainStar(starCnt));
        StartCoroutine(seq.WaitForCompletionCoroutine());
        StartCoroutine(GainStarSfxCoroutine());
    }     

    IEnumerator GainStarSfxCoroutine()
    {
        yield return new WaitForSeconds(0.8f);
        AudioManager.Instance.PlaySfx(ESfx.GainStar);
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
                    rowData.Add(m_LineIndicator.lineData[row, column]);
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

    public bool CheckIfShapeCanBePlacedOnGrid(ShapeData shapeData)
    {
        var shapeColumns = shapeData.columns;
        var shapeRows = shapeData.rows;

        // 활성화 되어 있는 블럭에 대한 인덱스 번호
        var filledUpSquareIndexes = new List<int>();
        var squareIndex = 0;
        for (var rowIndex = 0; rowIndex < shapeRows; rowIndex++)
        {
            for (var columnIndex = 0; columnIndex < shapeColumns; columnIndex++)
            {
                if (shapeData.board[rowIndex].column[columnIndex])
                {
                    filledUpSquareIndexes.Add(squareIndex);
                }

                squareIndex++;
            }
        }

        if (GetNumberOfSquares(shapeData) != filledUpSquareIndexes.Count)
            Debug.LogError("Number of filled up square are not the same as the original shape have.");

        var squareList = GetAllSquaresCombination(shapeColumns, shapeRows);
        var canBePlaced = false;
        foreach (var number in squareList)
        {
            var isCheck = true;
            foreach (var squareIndexToCheck in filledUpSquareIndexes)
            {
                var comp = gridSquares[number[squareIndexToCheck]].GetComponent<GridSquare>();
                if (comp.SquareOccupied)
                {
                    isCheck = false;
                }
            }

            if (isCheck)
            {
                //Debug.Log("number=[" + string.Join(",", number) + "]" + ", original=" + string.Join(",", originalSquareIndexes));
                canBePlaced = true;
                break;
            }
        }

        return canBePlaced;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        var number = 0;

        foreach (var row in shapeData.board)
        {
            foreach (var active in row.column)
            {
                if (active)
                    number++;
            }
        }

        return number;
    }

    private void CheckIfSquareCanBeHovered()
    {
        var currentSelectedShape = PanelManager.Instance.GetCurrentSelectedShape();
        if (null == currentSelectedShape)
            return;

        var hoveredSquareIndexes = new List<int>();
        foreach (var square in currentSelectedShape.squares)
        {
            var comp = square.GetComponent<ShapeSquare>();
            if (comp.Selected)
            {
                hoveredSquareIndexes.Add(comp.HoveredSquareIndex);
            }          
        }

        var isCheck = false;
        if (currentSelectedShape.squares.Count == hoveredSquareIndexes.Count)
        {
            var lines = GetAllSquaresCombination(currentSelectedShape.shapeData.columns, currentSelectedShape.shapeData.rows);
            
            foreach (var line in lines)
            {
                //Debug.Log($"line={string.Join(",", line)}, hoveredLine={string.Join(",", hoveredSquareIndexes)}");
                if (hoveredSquareIndexes.All(x => line.Contains(x)))
                {
                    isCheck = true;
                    break;
                }
            }
        }

        if (isCheck)
        {
            foreach (var index in hoveredSquareIndexes)
            {
                var comp = gridSquares[index].GetComponent<GridSquare>();
                comp.hoverImage.sprite = currentSelectedShape.textureData.texture;
                comp.hoverImage.gameObject.SetActive(true);
            }
        }
           
    }

    private void CheckIfSquareCanBeLineIsCompleted()
    {
        var currentSelectedShape = PanelManager.Instance.GetCurrentSelectedShape();
        if (null == currentSelectedShape)
            return;

        var completedLines = new List<int[]>();
        var notCompletedLines = new List<int[]>();

        for (var row = 0; row < rows; row++)
        {
            var completed = true;
            var squareIndexes = new List<int>();
            for (var column = 0; column < columns; column++)
            {
                var comp = gridSquares[m_LineIndicator.lineData[row, column]].GetComponent<GridSquare>();                
                if (!comp.SquareOccupied && !comp.IsHovered())
                {
                    completed = false;
                }               

                squareIndexes.Add(comp.SquareIndex);
            }

            if (completed)
                completedLines.Add(squareIndexes.ToArray());
            else
                notCompletedLines.Add(squareIndexes.ToArray());                
        }

        for (var column = 0; column < columns; column++)
        {
            var completed = true;
            var squareIndexes = new List<int>();
            for (var row = 0; row < rows; row++)
            {
                var comp = gridSquares[m_LineIndicator.lineData[row, column]].GetComponent<GridSquare>();                
                if (!comp.SquareOccupied && !comp.hoverImage.IsActive())
                {
                    completed = false;
                }               

                squareIndexes.Add(comp.SquareIndex);
            }

            if (completed)
                completedLines.Add(squareIndexes.ToArray());
            else
                notCompletedLines.Add(squareIndexes.ToArray());       
        }

        // 라인 상태를 해제합니다.
        foreach (var line in notCompletedLines)
        {
            foreach (var squareIndex in line)
            {
                var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.lineImage.IsActive())
                {
                    comp.lineImage.gameObject.SetActive(false);
                    if (comp.SquareOccupied)
                    {
                        comp.activeImage.gameObject.SetActive(true);
                    }
                }   
            }
        }

        // 라인 상태를 활성화합니다.
        foreach (var line in completedLines)
        {
            foreach (var squareIndex in line)
            {
                var comp = gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied && !comp.IsBonusIncluded)
                {
                    comp.activeImage.gameObject.SetActive(false);
                    comp.lineImage.sprite = currentSelectedShape.textureData.texture;                    
                    comp.lineImage.gameObject.SetActive(true); 
                }  
            }
        }
    }

    public int GetSquareIndex(int row, int column)
    {
        return m_LineIndicator.lineData[row, column];
    }
}
