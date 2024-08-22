using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VibrationManager : MonoSingleton<VibrationManager>
{
    private bool m_IsOn = true;
    private Vibrator m_Vibrator = null;
    private const string VIBRATION_ON = "PlayerPrefs_Vibration_On";

    public bool IsOn
    {
        get { return m_IsOn; }
        set 
        {
            m_IsOn = value;
            PlayerPrefs.SetInt(VIBRATION_ON, m_IsOn ? 1 : 0);
        }
    }

    protected override void OnAwake()
    {
        m_Vibrator = new Vibrator();
        m_IsOn = PlayerPrefs.GetInt(VIBRATION_ON, m_IsOn ? 1 : 0) == 1;
    }

    public void Vibrate(long timeMs)
    {
        if (m_IsOn && null != m_Vibrator)
        {
            #if UNITY_EDITOR
            #elif UNITY_ANDROID
            m_Vibrator.Vibrate(timeMs);
            #endif 
        }
    }

    public void VibrateOneShot()
    {
        long[] patten = { 100, 1, 0 };
        Vibrate(patten, -1);
    }

    public void Vibrate(long[] patten, int repeat)
    {
        if (!IsOn)
            return;

        if (m_IsOn && null != m_Vibrator)
        {
            #if UNITY_EDITOR
            #elif UNITY_ANDROID
            m_Vibrator.Vibrate(patten, repeat);
            #endif 
        }
    }

    public void VibrateEffect(long timeMs)
    {
        if (m_IsOn && null != m_Vibrator)
        {
            #if UNITY_EDITOR
            #elif UNITY_ANDROID
            m_Vibrator.VibrateEffect(timeMs);
            #endif 
        }
    }

    public void Stop()
    {
        if (m_IsOn && null != m_Vibrator)
        {
            #if UNITY_EDITOR
            #elif UNITY_ANDROID
            m_Vibrator.Cancel();
            #endif 
        }
    }
}
