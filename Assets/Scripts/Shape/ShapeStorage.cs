using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.Collections;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeDataList;
    public List<Shape> shapeList;
    
    private Grid m_Grid;

    void Awake()
    {
        m_Grid = FindObjectOfType<Grid>();   
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                return shape;
        }

        //Debug.LogError("There is no shape selected!");
        return null;
    }

    public int GetUsableShapeCnt()
    {
        var count = 0;
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                count++;
            }
        }
        return count;
    }

    public bool IsAllShapeActived()
    {
        return shapeList.All(x => x.IsOnStartPosition() && x.IsAnyOfShapeSquareActive());
    }

    public void CreateShapes()
    {
        Board.AllocateShapeData(m_Grid, shapeList);
    }

    public void Revive()
    {
        var shapes = new List<Shape>();
        foreach (var shape in shapeList)
        {
            if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
            {
                shape.DeactivateShape();
            }

            shapes.Add(shape);
        }

        Board.AllocateShapeData(m_Grid, shapes);
    }

    public List<T> Shuffle<T>(List<T> list)
    {
        var shuffled = new List<T>(list);
        int n = shuffled.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            (shuffled[n], shuffled[k]) = (shuffled[k], shuffled[n]);
        }
        return shuffled;
    }
}
