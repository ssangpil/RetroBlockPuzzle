using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibrator
{
    #if UNITY_EDITOR
    #elif UNITY_ANDROID
    private AndroidJavaObject m_AndroidJavaObject = null;

    public Vibrator()
    {
        var androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        var androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
        m_AndroidJavaObject = androidJavaObject.Call<AndroidJavaObject>("getSystemService", "vibrator");        
    }

    public void Vibrate(long timeMs)
    {
        if (null != m_AndroidJavaObject)
        {
            m_AndroidJavaObject.Call("vibrate", timeMs);
        }
        else
        {
            // 1초 진동
            Handheld.Vibrate();
        }
    }

    public void Vibrate(long[] pattern, int repeat)
    {
        if (null != m_AndroidJavaObject)
        {
            m_AndroidJavaObject.Call("vibrate", pattern, repeat);
        }
        else
        {
            // 1초 진동
            Handheld.Vibrate();
        }
    }

    public void VibrateEffect(long timeMs)
    {
        if (null != m_AndroidJavaObject)
        {
            var vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            var createOneShotMethod = vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", timeMs, -1);
            m_AndroidJavaObject.Call("vibrate", createOneShotMethod);
        }
        else
        {
            // 1초 진동
            Handheld.Vibrate();
        }
    }

    public void Cancel()
    {
        m_AndroidJavaObject?.Call("cancel");
    }
    #endif 
}
