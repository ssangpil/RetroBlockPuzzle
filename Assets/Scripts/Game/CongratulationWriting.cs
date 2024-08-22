using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class CongratulationWriting : MonoBehaviour
{
    public TextMeshProUGUI addScoreText;
    public Image congratulationImage;   
    public CongratulationTextureData congratulationTextureData; 

    public Tween ShowCongratulation(int score, int lineCnt, Vector3 position)
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => 
        {
            gameObject.SetActive(true);
            transform.position = position;

            addScoreText.text = "+ " + score;
            if (2 <= lineCnt)
            {
                var congratulationType = GetCongratulationType(lineCnt);
                AudioManager.Instance.PlayCongratulationVoice(congratulationType);

                congratulationImage.sprite = congratulationTextureData.GetTextureData(congratulationType).texture;
                congratulationImage.gameObject.SetActive(true);
            }
        });
        seq.Append(congratulationImage.transform.DOScale(1.5f, 0.15f));        
        seq.Append(congratulationImage.transform.DOScale(1f, 0.15f));        
        seq.AppendInterval(0.5f);
        seq.AppendCallback(() => 
        {
            gameObject.SetActive(false);
            congratulationImage.gameObject.SetActive(false);
        });
        return seq;
    }

    private Vector3 Clamp(Vector3 position)
    {
        var width = 670f;
        var height = 674f;

        var minX = -width / 2;
        var maxX = width / 2;
        var minY = -height / 2;
        var maxY = height / 2;

        var clampedX = Mathf.Clamp(position.x, minX, maxX);
        var clampedY = Mathf.Clamp(position.y, minY, maxY);
        Debug.Log($"clamp={clampedX}, {clampedY} - position={position.x}, {position.y}");
        return new Vector3(clampedX, clampedY, position.z);
    }

    private ECongratulationType GetCongratulationType(int lineCnt)
    {
        switch (lineCnt)
        {
            case 2: return ECongratulationType.GoodJob;
            case 3: return ECongratulationType.Great;
            case 4: return ECongratulationType.Excellent;
            case 5: return ECongratulationType.Perfect;
            default:return ECongratulationType.Perfect;
        }
    }
}
