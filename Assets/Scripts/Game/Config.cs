using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Config
{
    public static int RBBlockSpawnRate
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("RBBlockSpawnRate");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return -1;

            return value;
        }
    }

    public static int GaugePerRBBlock
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("GaugePerRBBlock");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return -1;

            return value;
        }
    }

    public static int MaxBonusGauge
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("MaxBonusGauge");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return -1;

            return value;
        }
    }

    public static int BonusEffectMultiplier
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("BonusEffectMultiplier");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return -1;

            return value;
        }
    }

    public static int BonusTimeDuration
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("BonusTimeDuration");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return -1;

            return value;
        }
    }

    public static int AddBonusTimeSec
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("AddBonusTimeSec");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return 5;

            return value;
        }
    }

    public static int ShowAdPlayCnt
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("ShowAdPlayCnt");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return 3;

            return value;
        }
    }

    public static int LimitReviveCnt
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("LimitReviveCnt");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return 1;

            return value;
        }
    }

    public static int AdLoadingTimeSec
    {
        get 
        {
            var refCommonconfig = ReferenceManager.Instance.FindRefCommonConfig("AdLoadingTimeSec");
            if (null == refCommonconfig || !int.TryParse(refCommonconfig.Value, out var value))
                return 10;

            return value;
        }
    }

    public static bool IsNetworkAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }
}
    public enum ESquareColor
    {
        None = 0,
        Blue,
        Green, 
        Purple,
        Red,
        Yellow,
        Max,
    }

    public enum ECongratulationType
    {
        None = 0,
        GoodJob,
        Great,
        Excellent,
        Perfect,
    }

    public enum EPanelShowBehaviour
    {
        KEEP_PREVIOUS,
        HIDE_PREVIOUS,
    }

    public enum ESfx 
    {
        Move,
        Placed,
        LineCompleted,
        GoodJob,
        Great,
        Excellent,
        Perfect,
        GainStar,
        ButtonClick,
        Filled,
        BonusTimeTick,
        BonusTimed,
        Max
    }

    public enum EBgm
    {
        Main,
        BonusTime,
    }

    public enum ELogLevel
    {
        None = 0,
        DEVELOPMENT,
        ERROR,
        INFO
    }

    public class Error
    {
        public static Error Succeed = new() { ErrorCode = 0, Message = "Succeed." };
        public static Error Failed = new() { ErrorCode = -1, Message = "Failed." };
        public static Error AdFailed = new() { ErrorCode = -2, Message = "Ad loading failed." };
        public int ErrorCode;
        public string Message;
    }