using UnityEngine;

public enum EPhase
{
    None = 0,
    Main,
    Game,
}

public abstract class PhaseBase : MonoBehaviour
{
    private bool m_IsEnter;

    public bool IsEnter => m_IsEnter;

    public void Init()
    {
        OnInit();
    }

    public void Enter(EPhase prevPhase)
    {
        m_IsEnter = true;
        OnEnter(prevPhase);
    }

    public void Leave(EPhase nextPhase)
    {
        OnLeave(nextPhase);
        m_IsEnter = false;
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }

    protected virtual void OnInit() { }
    protected virtual void OnEnter(EPhase prevPhase) { }
    protected virtual void OnLeave(EPhase nextPhase) { }
}