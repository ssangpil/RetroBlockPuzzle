
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TimeHelper
{
    private static float m_NormalPlayScale = 1.0f;
    private static float m_TimeScale = 1.0f;

    public static float time 
    {
        get { return Time.time;}
    }

    public static float realtimeSinceStartup
    { 
        get { return Time.realtimeSinceStartup; } 
    }

    public static float updateDeltaTime
    {
        get { return Time.deltaTime; }
    }

    public static float updateFixedDeltaTime 
    {
        get { return Time.fixedDeltaTime * Time.timeScale; }
    }

    public static float fixedDeltaTime
    {
        get { return Time.fixedDeltaTime; }
    }

    public static float CheckDeltaTime
    {
        get 
        {
            if (Time.timeScale >= 1f)
                return Time.deltaTime;
            else
                return Time.unscaledDeltaTime;
        }
    }

    public static float timeScale
    {
        get { return Time.timeScale; }
    }

    public static float DurationTime(float _time)
    {
        return time - _time;
    }

    public static void SlowTime(float scale, float minScale = 0f, float maxScale = 1f)
    {
        if (scale >= maxScale)
            scale = maxScale;
        else if (scale <= minScale)
            scale = minScale;

        Time.timeScale = scale;
    }

    public static void NormalTime()
    {
        Time.timeScale = m_NormalPlayScale;
    }

    public static void Pause()
    {
        Debug.Log("Time Pause");
        Time.timeScale = 0.0f;
    }

    public static void Resume()
    {
        Debug.Log("Time Resume");
        if (Time.timeScale != 0.0f)
            return;

        Time.timeScale = m_TimeScale;
    }

    public static void Reset()
    {
        RestoreTimeScale(true);
    }

    public static void RestoreTimeScale(bool isForce = false)
    {
        m_TimeScale = m_NormalPlayScale;
        Time.timeScale = m_TimeScale;
    }

    public static float ConvertDaysToSeconds(int days)
    {
        return days * 60 * 60 * 24;
    }
}