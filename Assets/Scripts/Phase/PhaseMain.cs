using UnityEngine;

public class PhaseMain : PhaseBase
{
    private PanelMain m_PanelMain;
    
    protected override void OnInit()
    {
        m_PanelMain = FindObjectOfType<PanelMain>(true);
    }
    
    protected override void OnEnter(EPhase prevPhase)
    {
        PanelManager.Play(m_PanelMain);
    }

    protected override void OnLeave(EPhase nextPhase)
    {
        
    }
}