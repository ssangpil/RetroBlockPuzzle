
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ComboWriting : MonoBehaviour
{
    public GameObject comboImage;
    public TextMeshProUGUI comboText;
    public Ease ease;
    
    public Tween ShowCombo(int comboCnt)
    {
        var seq = DOTween.Sequence();
        seq.AppendCallback(() => 
        {
            gameObject.SetActive(true);

            if (1 < comboCnt)
            {
                comboText.text = comboCnt.ToString();
                comboText.gameObject.SetActive(true);
            }
            else
            {
                comboText.text = "0";
                comboText.gameObject.SetActive(false);
            }
        });
        seq.Append(comboText.transform.DOScale(1.5f, 0.15f)).SetEase(ease);
        seq.Append(comboText.transform.DOScale(1f, 0.15f));
        seq.AppendCallback(() => 
        {
            var effect = EffectManager.Instance.ShowEffect(EEffectType.Combo, comboImage.transform);
            effect.SetLifeTime(1f);
        });
        seq.AppendInterval(0.2f);
        seq.AppendCallback(() => gameObject.SetActive(false));
        return seq;
    }
}
