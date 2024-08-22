
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu]
[System.Serializable]
public class SquareTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData 
    {
        public Sprite texture;
        public ESquareColor squareColor;
    }

    public List<TextureData> activeSquareTexture;
    public TextureData starTextureData;

    private int m_Index = -1;

    public TextureData NextShapeTextureData()
    {
        //var randomIndex = UnityEngine.Random.Range(0, activeSquareTexture.Count);
        if (((int)ESquareColor.Max) - 1 <= ++m_Index)
        {
            m_Index = 0;
        }

        return activeSquareTexture[m_Index];
    }

    public TextureData GetRandomTextureData()
    {
        var index = Random.Range(0, activeSquareTexture.Count);
        return activeSquareTexture[index];
    }

    public TextureData GetTextureData(ESquareColor color)
    {
        if (ESquareColor.None == color)
            return null;

        switch (color)
        {
            case ESquareColor.Max:
                return starTextureData;
            default:
                return activeSquareTexture[(int)color - 1];
        }
    }
}
