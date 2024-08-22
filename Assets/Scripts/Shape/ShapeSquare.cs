

using System;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSquare : MonoBehaviour
{
    public bool Selected { get; private set; }
    public int HoveredSquareIndex { get; private set; }
    public bool IsBonusIncluded { get; private set; }

    private RectTransform m_Transform; 
    private BoxCollider2D m_Collider;
    private Image m_Image;
    private Shadow m_Shadow;

    void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
        m_Collider = GetComponent<BoxCollider2D>();
        m_Image = GetComponent<Image>();
        m_Shadow = GetComponent<Shadow>();
    }

    public void Create(ShapeData shapeData, SquareTextureData.TextureData textureData, int row, int column, float squareOffset)
    {
        var moveDistance = new Vector2(
            m_Transform.rect.width * m_Transform.localScale.x + squareOffset, 
            m_Transform.rect.height * m_Transform.localScale.y + squareOffset);

        m_Image.sprite = textureData.texture;
        m_Shadow.enabled = true;                    
        m_Collider.enabled = false;   
        m_Transform.localPosition = 
            new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance), GetYPositionForShapeSquare(shapeData, row, moveDistance));                
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        var shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSquareIndex = shapeData.rows / 2;
                var distanceFromMiddle = Math.Abs(row - middleSquareIndex);

                shiftOnY = (row < middleSquareIndex ? 1 : -1) * moveDistance.y * distanceFromMiddle;
            }
            else
            {
                var middleSquareIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows / 2 - 1;
                var middleSquareIndex2 = (shapeData.rows == 2) ? 1 :shapeData.rows / 2;

                if (row == middleSquareIndex1 || row == middleSquareIndex2)
                {
                    shiftOnY = (row == middleSquareIndex1 ? 1 : -1) * moveDistance.y / 2;
                }
                else
                {
                    shiftOnY = (row < middleSquareIndex1 ? 1 : -1) * moveDistance.y * 1.5f;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        var shiftOnX = 0f;

        if (shapeData.columns > 1) // Vertical position calculation
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = shapeData.columns / 2;
                var distanceFromMiddle = Math.Abs(column - middleSquareIndex);
                shiftOnX = (column < middleSquareIndex ? -1 : 1) * moveDistance.x * distanceFromMiddle;
            }
            else
            {
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns / 2 - 1;
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : shapeData.columns / 2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    shiftOnX = (column == middleSquareIndex1 ? -1 : 1) * moveDistance.x / 2;
                }
                else
                {
                    shiftOnX = (column < middleSquareIndex1 ? -1 : 1) * moveDistance.x * 1.5f;
                }
            }
        }

        return shiftOnX;
    }

    public void SetBonus()
    {
        IsBonusIncluded = true;
        m_Image.sprite = GameManager.Instance.squareTextureData.starTextureData.texture;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var comp = collision.GetComponent<GridSquare>();
        if (null != comp && !comp.SquareOccupied)
        {
            Selected = true;
            HoveredSquareIndex = comp.SquareIndex;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        var comp = collision.GetComponent<GridSquare>();
        if (null != comp && !comp.SquareOccupied)
        {
            Selected = true;
            HoveredSquareIndex = comp.SquareIndex;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Selected = false;
        HoveredSquareIndex = 0;
    }
}
