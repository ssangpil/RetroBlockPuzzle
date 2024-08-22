using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
[System.Serializable]
public class CongratulationTextureData : ScriptableObject
{
    [System.Serializable]
    public class TextureData
    {
        public Sprite texture;
        public ECongratulationType congratulationType;
    }

    public List<TextureData> congratulationTexture;

    public TextureData GetTextureData(ECongratulationType congratulationType)
    {
        for (var i = 0; i < congratulationTexture.Count; i++)
        {
            if (congratulationTexture[i].congratulationType == congratulationType)
                return congratulationTexture[i];
        }

        return null;
    }
}
