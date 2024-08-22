using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    void Awake()
    {
        scoreText.text = "0";
    }
    
    public void UpdateScoreText(int prevScore, int currentScore)
    {
        StartCoroutine(UpdateScoreTextCoroutine(prevScore, currentScore));
    }

    IEnumerator UpdateScoreTextCoroutine(int prev, int after)
    {
        var seq = DOTween.Sequence();
        seq.Append(scoreText.transform.DOScale(1.5f, 0.1f));
        seq.Append(scoreText.transform.DOScale(1f, 0.1f));
        seq.Append(DOTween.To(() => prev, x => prev = x, after, 0.1f)
            .OnUpdate(() =>
            {
                scoreText.text = string.Format("{0:#,##0}", prev);
            }));
        seq.Play();
        yield return seq.WaitForCompletion();
    }

}
