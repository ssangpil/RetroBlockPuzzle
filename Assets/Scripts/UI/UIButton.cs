using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("LunchLunch/UI/Button")]
public class UIButton : MonoBehaviour
{
    private UIEventListener m_EventListener;
    [SerializeField] Image sourceImage;
    [SerializeField] Sprite pressedSprite;

    void OnMouseDown()
    {
        if (null == m_EventListener)
            m_EventListener = GetComponent<UIEventListener>();

        StartCoroutine(ClickCoroutine());
    }

    IEnumerator ClickCoroutine()
    {
        AudioManager.Instance.PlaySfx(ESfx.ButtonClick);
        
        if (null != pressedSprite)
            sourceImage.sprite = pressedSprite;

        yield return new WaitForSeconds(0.1f);
        
        m_EventListener.OnClick();
    }

    public void OnClick()
    {
        if (null == m_EventListener)
            m_EventListener = GetComponent<UIEventListener>();

        AudioManager.Instance.PlaySfx(ESfx.ButtonClick);
        m_EventListener.OnClick();
    }
}