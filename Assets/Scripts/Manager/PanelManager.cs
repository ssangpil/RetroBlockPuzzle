
using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Video;

public class PanelManager : MonoSingleton<PanelManager>
{
    private readonly List<PanelInstanceModel> m_Instances = new List<PanelInstanceModel>();

    private PanelPopupCommon panelPopupCommon
    {
        get { return PrefabManager.Get<PanelPopupCommon>(); }
    }

    public bool IsNoticePopup
    {
        get { return panelPopupCommon.gameObject.activeSelf; }
    }

    private void PlayPopup(UIPanelPopup panelPopup, EPanelShowBehaviour behaviour)
    {
        if (null == panelPopup)
            return;

        if (behaviour == EPanelShowBehaviour.HIDE_PREVIOUS && GetAmountPanelsInList() > 0)
        {
            var lastPanel = GetLastPanel();
            lastPanel?.panelPopup.gameObject.SetActive(false);
        }

        m_Instances.Add(new PanelInstanceModel 
        { 
            panelId = panelPopup.name, 
            panelPopup = panelPopup
        });

        panelPopup.In();
    }

    public static void Play(UIPanelPopup panelPopup, EPanelShowBehaviour behaviour = EPanelShowBehaviour.KEEP_PREVIOUS)
    {
       Instance.PlayPopup(panelPopup, behaviour);
    }

    public static void HideLastPanel()
    {
        Instance.HideLastPanelOut();
    }

    private void HideLastPanelOut()
    {
        if (AnyPanelShowing())
        {
            var lastPanel = GetLastPanel();
            lastPanel.panelPopup.Out();   
            m_Instances.Remove(lastPanel);            

            if (GetAmountPanelsInList() > 0)
            {
                lastPanel = GetLastPanel();
                if (null != lastPanel && !lastPanel.panelPopup.gameObject.activeInHierarchy)
                {
                    lastPanel.panelPopup.gameObject.SetActive(true);
                }
            }
        }
    }

    private PanelInstanceModel GetLastPanel()
    {
        return m_Instances[m_Instances.Count - 1];
    }

    public bool AnyPanelShowing()
    {
        return GetAmountPanelsInList() > 0;
    }

    public int GetAmountPanelsInList()
    {
        return m_Instances.Count;
    }

    public void CloseAll()
    {
        foreach (var instance in m_Instances)
        {
            instance.panelPopup.OutNow();
        }

        m_Instances.Clear();
    }

    public T FindPanel<T>(string name) where T : UIPanelPopup
    {
        var panel = PrefabManager.Get<T>(name);
        if (null != panel && panel.gameObject.activeSelf)
            return panel;

        return null;
    }

    public Shape GetCurrentSelectedShape()
    {
        var panelGame = FindPanel<PanelGame>("PanelGame");
        if (null != panelGame)
            return panelGame.GetCurrentSelectedShape();
        
        return null;
    }

    public void DeactivateAllShapes()
    {
        var panelGame = FindPanel<PanelGame>("PanelGame");
        if (null == panelGame)
            return;

        panelGame.DeactivateAllShapes();
    }

    public int GetPlacedOnGridShapeCnt()
    {
        var panelGame = FindPanel<PanelGame>("PanelGame");
        if (null == panelGame)
            return 0;

        return panelGame.GetPlacedOnGridShapeCnt();
    }

    public void SetNoticePopup(string title, string msg, Action okCallback = null, bool isBack = true)
    {
        if (null == panelPopupCommon)
            return;

        if (!panelPopupCommon.gameObject.activeSelf)
            Play(panelPopupCommon);
        
        panelPopupCommon.SetNoticePopup(title, msg, okCallback, isBack);
    }
}
