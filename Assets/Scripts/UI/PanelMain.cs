using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PanelMain : UIPanelPopup
{
    [SerializeField] GameObject m_BtnStart;
    [SerializeField] GameObject m_BtnSettings;    

    void Awake()
    {
        m_BtnStart.SafeSetClickEvent(OnClick);
        m_BtnSettings.SafeSetClickEvent(OnClick);        
    }

    protected override Tween OnIn()
    {
        EffectManager.Instance.ShowEffect(EEffectType.Background, transform);

        var isNRU = Convert.ToBoolean(PlayerPrefs.GetInt("IsNRU", 1));
        if (isNRU)
        {
            // 신규 유저
            m_BtnStart.SetActive(false);  
            m_BtnSettings.SetActive(false);    
            StartCoroutine(MoveGameCoroutine());
        }
        else
        {
            m_BtnStart.SetActive(true);
            m_BtnSettings.SetActive(true);
        }

        return base.OnIn();
    }

    protected override Tween OnOut()
    {
        return base.OnOut();
    }

    IEnumerator MoveGameCoroutine()
    {
        yield return new WaitForSeconds(1f);

        GameManager.Instance.IncreasePlayCnt();
        PhaseManager.Instance.MovePhaseGame();        
    }

    private void OnClick(GameObject obj)
    {
        if (obj == m_BtnStart)
        {
            PhaseManager.Instance.MovePhaseGame();
        }
        else if (obj == m_BtnSettings)
        {
            var settings = PrefabManager.Get<PanelPopupSettings>("PanelPopupSettings");
            if (null != settings)
            {
                GameManager.Instance.isTouchDefense = true;
                PanelManager.Play(settings, EPanelShowBehaviour.KEEP_PREVIOUS);
            }
        }
    }
}
