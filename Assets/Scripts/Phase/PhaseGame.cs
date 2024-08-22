using UnityEngine;

public class PhaseGame : PhaseBase
{
    protected override void OnInit()
    {
        
    }
    
    protected override void OnEnter(EPhase prevPhase)
    {
        GameManager.Instance.Init();

        var panelGame = PrefabManager.Get<PanelGame>("PanelGame");
        PanelManager.Play(panelGame);
    }

    protected override void OnLeave(EPhase nextPhase)
    {
        EffectManager.Instance.Clear();
    }
}