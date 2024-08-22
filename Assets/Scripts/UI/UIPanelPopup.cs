using System.Collections;
using DG.Tweening;
using UnityEngine;

[AddComponentMenu("LunchLunch/UI/Panel")]
public abstract class UIPanelPopup : MonoBehaviour
{
    void Update()
    {
        OnUpdate();
    }

    public void In()
    {
        GameManager.Instance.SetTouchDefense(true);
        StartPanel();
        var intro = OnIn(); 
        if (null != intro)
            StartCoroutine(IntroCoroutine(intro));
        else
            GameManager.Instance.SetTouchDefense(false);
    }

    IEnumerator IntroCoroutine(Tween tween)
    {
        yield return tween.WaitForCompletion();        
        GameManager.Instance.SetTouchDefense(false);
    }

    public void Out()
    {
        if (!gameObject.activeSelf)
            return;

        var outro = OnOut();
        if (null != outro)
            StartCoroutine(OutroCoroutine(outro));
        else
            StopPanel();
    }

    public void OutNow()
    {
        if (!gameObject.activeSelf)
            return;

        OnOut();
        StopPanel();
    }

    IEnumerator OutroCoroutine(Tween tween)
    {
        yield return tween.WaitForCompletion();
        StopPanel();
    }

    public void StartPanel()
    {
        gameObject.SetActive(true);
    }

    public void StopPanel()
    {
        gameObject.SetActive(false);
    }

    protected virtual Tween OnIn()
    {
        return null;
    }

    protected virtual Tween OnOut()
    {
        return null;
    }
    
    protected virtual void OnUpdate()
    {

    }
}