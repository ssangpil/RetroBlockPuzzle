using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class BestScoreBar : MonoBehaviour
{
    public TextMeshProUGUI bestScoreText;
    
    void Awake()
    {
        bestScoreText.text = "0";
    }

    public void UpdateBestScoreText(int prevBestScore, int currentBestScore)
    {        
        StartCoroutine(UpdateBestScoreTextCoroutine(prevBestScore, currentBestScore));
    }

    IEnumerator UpdateBestScoreTextCoroutine(int prev, int after)
    {
        var seq = DOTween.Sequence();
        seq.Append(DOTween.To(() => prev, x => prev = x, after, 0.1f)
            .OnUpdate(() =>
            {
                bestScoreText.text = string.Format("{0:#,##0}", prev);
            }));
        seq.Play();
        yield return seq.WaitForCompletion();    
    }
}
