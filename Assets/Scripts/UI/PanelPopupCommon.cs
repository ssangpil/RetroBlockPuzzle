using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelPopupCommon : UIPanelPopup
{
    public GameObject btnBack;
    public GameObject btnOk;
    public GameObject textTitle;
    public GameObject textMessage;

    private Action m_OkCallback;

    void Awake()
    {
        btnBack.SafeSetClickEvent(OnClick);
        btnOk.SafeSetClickEvent(OnClick);
    }

    void OnClick(GameObject go)
    {
        if (go == btnBack || go == btnOk)
        {
            m_OkCallback.SafeInvoke();
            m_OkCallback = null;
            OutNow();
        }
    }

    public void SetNoticePopup(string title, string msg, Action okCallback, bool isBack)
    {
        textTitle.SafeText(title);
        textMessage.SafeText(msg);
        m_OkCallback = okCallback;

        if (null == okCallback)
            btnOk.SetActive(false);
        else
            btnOk.SetActive(true);

        btnBack.SetActive(isBack);
    }
}
