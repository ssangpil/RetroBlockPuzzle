using System;
using System.Collections.Generic;
using UnityEngine;

public class PhaseManager : MonoSingleton<PhaseManager>
{
    [Serializable]
    public class PhaseData
    {
        public EPhase phase;
        public AudioClip bgmAudioClip;
        public float bgmVol = 1f;
        public UIPanelPopup backPanel;
    }

    public PhaseData[] phaseDataList;

    private Dictionary<EPhase, PhaseBase> m_DictPhase = new Dictionary<EPhase, PhaseBase>();
    private EPhase m_CurrentPhase = EPhase.None;
    private EPhase m_LastPhase = EPhase.None;

    void Awake()
    {
        InitPhase();
        StartGame();
    }

    protected virtual void InitPhase()
    {
        RegisterPhase(EPhase.Main, FindObjectOfType<PhaseMain>());
        RegisterPhase(EPhase.Game, FindObjectOfType<PhaseGame>());
    }

    protected void RegisterPhase(EPhase phase, PhaseBase phaseBase)
    {
        if (null == phaseBase)
            return;

        if (m_DictPhase.ContainsKey(phase))
            return;

        m_DictPhase[phase] = phaseBase;
        phaseBase.SetActive(false);
    }

    private void StartGame()
    {
        foreach (var phase in m_DictPhase.Values)
        {
            phase.Init();
            phase.SetActive(false);
        }

        ChangePhase(EPhase.Main);
    }

    public bool IsMainMenu()
    {
        return m_CurrentPhase == EPhase.Main;
    }

    public PhaseBase FindPhaseBase(EPhase phase)
    {
        if (!m_DictPhase.ContainsKey(phase))
            return null;
        
        return m_DictPhase[phase];
    }

    public void ChangePhase(EPhase phase)
    {
        var leavePhaseBase = FindPhaseBase(m_CurrentPhase);
        if (null != leavePhaseBase)
        {
            leavePhaseBase.Leave(phase);
            leavePhaseBase.SetActive(false);
        }

        PanelManager.Instance.CloseAll();

        m_LastPhase = m_CurrentPhase;
        m_CurrentPhase = phase;

        var currentPhaseBase = FindPhaseBase(phase);
        if (null != currentPhaseBase)
        {
            currentPhaseBase.SetActive(true);
            currentPhaseBase.Enter(m_LastPhase);
        }

        GC.Collect();
    }

    public void MovePhaseMain()
    {
        GameManager.Instance.isTouchDefense = false;
        ChangePhase(EPhase.Main);
    }

    public void MovePhaseGame()
    {        
        GameManager.Instance.isTouchDefense = false;        
        ChangePhase(EPhase.Game);
    }
}