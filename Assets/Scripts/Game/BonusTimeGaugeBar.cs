using System;
using System.Collections;
using System.IO;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BonusTimeGaugeBar : MonoBehaviour
{
    [Header("#Star")]
    public GameObject objStar;
    public GameObject[] listStar;

    [Header("#BonusTimeGauge")]
    public GameObject objTimeGauge;
    public GameObject fillInImage;
    public GameObject textAddTime;
    
    private int m_StarIndex = -1;
    private float m_BonusTimeSec = 0f;
    private float m_DecreaseBonusTimePerSecond = 0f;
    private bool m_IsBonusTime = false;
    private EffectLifeTime m_BonusTimeEffect = null;
    private float m_AddTimeSec = 0f;
    private Image m_FillInImage;
    private int m_MaxStarIndex;
    private Tween m_BlinkTween;
    private Tween m_FillInTween;
    
    public bool IsBonusTime => m_IsBonusTime;
    public int StarIndex => m_StarIndex;
    public int MaxStarIndex => m_MaxStarIndex;

    void Awake()
    {
        m_DecreaseBonusTimePerSecond = 100 / (float)Config.BonusTimeDuration;
        m_FillInImage = fillInImage.GetComponent<Image>();
        m_MaxStarIndex = listStar.Length - 1;
        m_StarIndex = -1;
    }

    void Update()
    {
        if (m_IsBonusTime && 0 < m_BonusTimeSec)
        { 
            if (0 < m_AddTimeSec)
            {
                Debug.Log($"m_BonusTimeSec={m_BonusTimeSec}, m_AddTimeSec={m_AddTimeSec}");
                var timeSec = m_AddTimeSec;
                m_BonusTimeSec += timeSec;
                m_AddTimeSec = 0;
            }

            var oldValue = m_BonusTimeSec;
            m_BonusTimeSec = Mathf.Max(m_BonusTimeSec - m_DecreaseBonusTimePerSecond * Time.deltaTime, 0f);
            UpdateBonusTimeGaugeBarCoroutine(oldValue / 100, m_BonusTimeSec / 100);

            if (0 == m_BonusTimeSec)
            {
                StopBonusTime();
                return;
            }
        } 
    }

    public void Clear()
    {
        m_StarIndex = -1;
        m_AddTimeSec = 0f;
        m_BonusTimeSec = 0f;
        m_IsBonusTime = false;
        m_FillInImage.fillAmount = 0;
        for (var i = 0; i < listStar.Length; i++)
        {
            listStar[i].SetActive(false);
        }

        objStar.SetActive(true);
        objTimeGauge.SetActive(false);
    }

    public void EnableStar(int starCnt)
    {
        var maxIndex = Mathf.Min(starCnt - 1, m_MaxStarIndex);
        for (var i = 0; i <= maxIndex; i++)
        {
            listStar[i].SetActive(true);
            m_StarIndex = i;
        }
    }

    public void GainStar(int starCnt)
    {
        var seq = DOTween.Sequence();
        if(m_IsBonusTime)
        {
            seq.Append(AddBonusTime(starCnt));
        }
        else
        {
            var oldIndex = m_StarIndex;
            m_StarIndex += starCnt;
            var remainCnt = Mathf.Max(0, m_StarIndex - m_MaxStarIndex);
            var maxIndex = Mathf.Min(m_MaxStarIndex, m_StarIndex);

            for (var i = oldIndex + 1; i <= maxIndex; i++)
            {
                if (listStar.Length > i)
                {
                    var obj = listStar[i];
                    var inner = DOTween.Sequence();
                    inner.AppendCallback(() => obj.SetActive(true));
                    inner.Append(obj.transform.DOScale(1.5f, 0.1f));
                    inner.Append(obj.transform.DOScale(1f, 0.1f));
                    seq.Join(inner);
                }
            }

            if (m_StarIndex >= m_MaxStarIndex)
            {
                m_StarIndex = -1;
                seq.Append(PanelManager.Instance.FindPanel<PanelGame>("PanelGame").ShowBonusTimeWriting());
                seq.AppendCallback(() => StartBonusTime()); 
            }     
            
            if (0 < remainCnt)
            {
                seq.Append(AddBonusTime(remainCnt));
            }                
        }

        StartCoroutine(seq.WaitForCompletionCoroutine());
    }

    private Tween AddBonusTime(int starCnt)
    {
        var timeSec = starCnt * Config.AddBonusTimeSec;
        m_AddTimeSec += timeSec * m_DecreaseBonusTimePerSecond;

        var seq = DOTween.Sequence();
        seq.AppendCallback(() => 
        {
            textAddTime.GetComponent<TextMeshProUGUI>().text = $"+ {timeSec}";
            textAddTime.SetActive(true);
        });

        seq.AppendInterval(1f);
        seq.AppendCallback(() => textAddTime.SetActive(false));
        return seq;
    }

    public void StartBonusTime()
    {
        // 보너스 타임 시작
        objStar.SetActive(false);
        objTimeGauge.SetActive(true);

        m_IsBonusTime = true;
        m_BonusTimeSec = 100;
        m_BonusTimeEffect = EffectManager.Instance.ShowEffect(EEffectType.BonusTimeGauge, transform);
        m_BonusTimeEffect.SetLifeTime(Config.BonusTimeDuration);

        StartBlinkEffect();

        GameManager.Instance.SetIsBonusTime(true);
    }

    private void StartBlinkEffect()
    {
        if (null != m_BlinkTween && m_BlinkTween.IsActive())
        {
            Debug.Log("Stopping active Tween before starting a new one.");
            m_BlinkTween.Kill();
        }

        // 페이드 아웃 (최대 알파값에서 최소 알파값으로)
        m_BlinkTween = m_FillInImage.DOFade(0.1f, 0.5f)
            .OnComplete(() =>
            {
                // 페이드 인 (최소 알파값에서 최대 알파값으로)
                m_BlinkTween = m_FillInImage.DOFade(1.0f, 0.5f)
                    .OnComplete(() =>
                    {
                        AudioManager.Instance.PlaySfx(ESfx.BonusTimeTick);
                        m_FillInImage.transform.DOPunchScale(new Vector3(0.1f, 0.1f, 0.1f), 0.5f, 10, 0.2f);
                        StartBlinkEffect(); // 재귀 호출로 깜박임 반복
                    });
            });
    }

    private void StopBlinkEffect()
    {
        if (null != m_BlinkTween && m_BlinkTween.IsActive())
        {
            Debug.Log("Stopping Blink Effect.");
            m_BlinkTween.Kill();
        }
        else
        {
            Debug.Log("Blink Effect was not active.");
        }
            
    }

    public void StopBonusTime()
    {
        if (!m_IsBonusTime)
            return;

        m_IsBonusTime = false;
        m_BonusTimeEffect.SetLifeTime(0f);
        m_FillInImage.fillAmount = 0f;        
        m_StarIndex = -1;
        foreach (var star in listStar)
        {
            star.SetActive(false);
        }

        StopBlinkEffect();

        objTimeGauge.SetActive(false);
        objStar.SetActive(true);

        GameManager.Instance.SetIsBonusTime(false);
    }

    private void UpdateBonusTimeGaugeBarCoroutine(float prev, float after)
    {
        if (null != m_FillInTween && m_FillInTween.IsActive())
            m_FillInTween.Kill();

        m_FillInTween = DOTween.To(() => prev, x => prev = x, after, 0.1f)
            .OnUpdate(() =>
            {
                m_FillInImage.fillAmount = prev;
            });
    }

}
