using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using DG.Tweening.Core;
using System.Linq;

public class Shape : MonoBehaviour
{
    public Ease ease = Ease.OutCubic;   
    public float moveOffsetY = 1.0f;
    public float squareOffset = 0f;
    public float squareScale = 0.4f;
    public GameObject squareImage;

    [HideInInspector] public int bonusSquareIndex = -1;
    [HideInInspector] public ShapeData shapeData;
    [HideInInspector] public SquareTextureData.TextureData textureData;

    [HideInInspector] public int TotalSquareCnt;

    [HideInInspector] public List<GameObject> squares = new List<GameObject>();

    private float m_MoveOffsetY;
    private float m_MoveOffsetX;    
    private bool m_Moving = false;
    private Tween m_ScaleTween = null;
    private Tween m_MoveTween = null;

    private RectTransform m_Transform;
    private Vector3 m_OriginalPosition;
    private Vector2 m_OriginalPivot;
    private Vector2 m_OriginalAnchorMin;
    private Vector2 m_OriginalAnchorMax;
    private bool m_IsShapeActive = true;

    public bool IsMoving => m_Moving;
    
    void Awake()
    {
        m_Transform = GetComponent<RectTransform>();
        m_OriginalPosition = m_Transform.localPosition; 
        m_OriginalPivot = m_Transform.pivot;
        m_OriginalAnchorMin = m_Transform.anchorMin;
        m_OriginalAnchorMax = m_Transform.anchorMax;   
        m_IsShapeActive = true;     
    }
    
    public bool IsOnStartPosition()
    {
        return null != m_Transform && m_Transform.localPosition == m_OriginalPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in squares)
        {
            if (square.activeSelf)
                return true;
        }

        return false;
    }

    public void ActivateShape()
    {
        if (!m_IsShapeActive)
        {      
            foreach (var square in squares)
            {
                square.transform.SetParent(m_Transform);
                square.SetActive(true);
            }
        }

        m_IsShapeActive = true;
    }

    public void DeactivateShape()
    {
        if (m_IsShapeActive)
        {
            foreach (var square in squares)
            {
                square.SetActive(false);
            }

            squares.Clear();
        }

        m_IsShapeActive = false;
    }

    public void Create(ShapeData shapeData, SquareTextureData.TextureData textureData)
    {        
        bonusSquareIndex = -1;
        m_Transform.localPosition = m_OriginalPosition;   
        m_Transform.localScale = new Vector3(squareScale, squareScale, squareScale);
        CreateShape(shapeData, textureData);       
    }

    public void SetBonus(int squareIndex)
    {
        bonusSquareIndex = squareIndex;
        squares[squareIndex].GetComponent<ShapeSquare>().SetBonus();
    }

    private void CreateShape(ShapeData shapeData, SquareTextureData.TextureData textureData)
    {
        this.shapeData = shapeData;
        this.textureData = textureData; 
        TotalSquareCnt = GetNumberOfSquares(shapeData);

        for (var column = 0; column < shapeData.columns; column++)
        {
            for (var row = 0; row < shapeData.rows; row++)
            {
                if (shapeData.board[row].column[column])
                {
                    var shapeSquare = Utility.InstantiateGameObject(squareImage, transform, Vector3.zero);
                    shapeSquare.GetComponent<ShapeSquare>().Create(shapeData, textureData, row, column, squareOffset);
                    squares.Add(shapeSquare);
                }   
            }
        }

        StartCoroutine(ShapeActiveAnimationCoroutine());
    }

    IEnumerator ShapeActiveAnimationCoroutine()
    {
        var seq = DOTween.Sequence();

        foreach (var shape in squares)
        {   
            seq.Join(shape.transform.DOScale(shape.transform.localScale, 0.1f).From(0f));
            seq.OnComplete(() => shape.SetActive(true));
        }

        seq.Play();
        yield return seq.WaitForCompletion();

        foreach (var shape in squares)
        {
            shape.SetActive(true);    
        }

        var effect = EffectManager.Instance.ShowEffect(EEffectType.Shape, transform);
        effect.SetLifeTime(1f);
        effect.Play();
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


    public void MoveToStartPosition()
    {        
        m_Transform.localPosition = m_OriginalPosition;
    }

    private Vector3 GetMousePosition()
    {
        var worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3(worldPosition.x, worldPosition.y, m_Transform.position.z);
    }

    void OnMouseDown()
    {
        if (m_Moving || GameManager.Instance.isTouchDefense || !IsOnStartPosition()) 
            return;

        m_Moving = true;

        // 터치 용 콜라이더 끄기
        GetComponent<BoxCollider2D>().enabled = false;
        foreach (var shape in squares)
        {
            // 콜라이더 켜기
            shape.GetComponent<BoxCollider2D>().enabled = true;
            // 그림자 끄기
            shape.GetComponent<Shadow>().enabled = false;
        }
                
        // 앵커를 중앙 하단으로 이동
        m_Transform.pivot = new Vector2(0.5f, 0);
        m_Transform.anchorMin = new Vector2(0.5f, 0);
        m_Transform.anchorMax = new Vector2(0.5f, 0);

        // 마우스or터치 지점에서 무브 포인트까지의 Y거리 계산
        m_MoveOffsetY = m_Transform.position.y + moveOffsetY - GetMousePosition().y;
        m_MoveOffsetX = m_Transform.position.x - GetMousePosition().x;

        m_ScaleTween = m_Transform.DOScale(1f, 0.1f).SetEase(ease);
        m_MoveTween = m_Transform.DOMoveY(m_Transform.position.y + moveOffsetY, 0.1f).SetEase(ease)
            .OnStart(() => AudioManager.Instance.PlaySfx(ESfx.Move))
            .OnComplete(() => m_Moving = false);        

        m_Transform.SetAsLastSibling();
    }

    private void StopMoving()
    {
        if (m_ScaleTween.IsActive()) 
            m_ScaleTween.Kill();
        if (m_MoveTween.IsActive()) 
            m_MoveTween.Kill();
            
        m_Moving = false;
    }

    void OnMouseDrag()
    {
        if (m_Moving || GameManager.Instance.isTouchDefense)
            return;

        var mousePosition = GetMousePosition();
        var position = new Vector3(mousePosition.x + m_MoveOffsetX, mousePosition.y + m_MoveOffsetY, mousePosition.z);
        m_MoveTween = m_Transform.DOMove(position, 0f).SetEase(Ease.Linear);        
    }

    void OnMouseUp()
    {      
        if (m_Moving)
        {
            StopMoving();
        }

        // 원래 값으로 복원
        m_Transform.pivot = m_OriginalPivot;
        m_Transform.anchorMin = m_OriginalAnchorMin;
        m_Transform.anchorMax = m_OriginalAnchorMax;
        m_Transform.localScale = new Vector3(squareScale, squareScale, squareScale);

        // 블록을 놓을 수 있는지 체크
        GameEvents.CheckIfShapeCanBePlaced();

        // 터치 용 콜라이더 켜기
        GetComponent<BoxCollider2D>().enabled = true;
        foreach (var shape in squares)
        {
            // 콜라이더 끄기
            shape.GetComponent<BoxCollider2D>().enabled = false;
            // 그림자 켜기
            shape.GetComponent<Shadow>().enabled = true;
        }
    }
}
