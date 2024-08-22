using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;
    public Image normalImage;
    public Image lineImage;

    [HideInInspector] public int SquareIndex;
    [HideInInspector] public bool Selected;
    public bool SquareOccupied;
    [HideInInspector] public bool IsBonusIncluded;
    [HideInInspector] public ESquareColor ActiveSquareColor;

    public bool IsHovered()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard(bool isBonusIncluded, SquareTextureData.TextureData textureData)
    {
        IsBonusIncluded = isBonusIncluded;
        ActivateSquare(textureData);
    }

    public void ActivateSquare(SquareTextureData.TextureData textureData)
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        activeImage.sprite = textureData.texture;
        ActiveSquareColor = textureData.squareColor;
        Selected = true;
        SquareOccupied = true;
    }

    public void DeactivateSquare()
    {
        activeImage.gameObject.SetActive(false);    
        lineImage.gameObject.SetActive(false);
        ActiveSquareColor = ESquareColor.None;
        Selected = false;
        SquareOccupied = false;
        IsBonusIncluded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!SquareOccupied)
        {
            Selected = true;
        }   
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Selected = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!SquareOccupied)
        {
            Selected = false;
            hoverImage.gameObject.SetActive(false);
        }    
    }

    
}
