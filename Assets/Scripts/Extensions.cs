

using System;
using System.CodeDom;
using System.Collections;
using System.Linq;
using DG.Tweening;
using GoogleMobileAds.Ump.Api;
using UnityEngine;

public static class Extensions
{
    public static int GetBlockGroupID(this ReferenceManager @this, int blockId)
    {
        var list = @this.GetRefBlockGroups();
        foreach (var refBlockGroup in list)
        {
            var splits = refBlockGroup.BlockIDList.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in splits)
            {
                var id = Convert.ToInt32(str);
                if (id == blockId)
                {
                    return refBlockGroup.BlockGroupID;
                }
            }
        }
        return -1;
    }

    public static float GetBlankValue(this ReferenceManager @this, int groupId, int blankCnt)
    {
        var list = @this.FindRefBlankValue(groupId);
        if (!list.Any())
            return 1f;

        var refBlankValue = list.FirstOrDefault(x => x.BlankMin <= blankCnt && blankCnt <= x.BlankMax);
        return refBlankValue.Value / 10000f;
    }

    public static bool IsCheckCanBeLineIsCompleted(this ReferenceManager @this, int turnCnt)
    {
        var refBlockSpawnRate = @this.GetRefBlockSpawnRates().FirstOrDefault(x => x.TurnMin <= turnCnt && (-1 == x.TurnMax || turnCnt <= x.TurnMax));
        if (null == refBlockSpawnRate)
            throw new Exception("Not found RefBlockSpawnRate");

        var randomProb = UnityEngine.Random.Range(1, 10001);
        return !(randomProb <= refBlockSpawnRate.SpawnType1Prob);
    }

    public static Color ToColor(this ESquareColor color)
    {
        switch (color)
        {
            case ESquareColor.Blue:
                return NormalizeColor(40, 219, 255, 1f);
            case ESquareColor.Green:
                return NormalizeColor(14, 195, 124, 1f);
            case ESquareColor.Purple:
                return NormalizeColor(193, 84, 255, 1f);
            case ESquareColor.Yellow:
                return NormalizeColor(255, 192, 16, 1f);
            case ESquareColor.Red:
                return NormalizeColor(255, 58, 112, 1f);
            default:
                return Color.white;
        }
    }

    private static Color NormalizeColor(int red, int green, int blue, float alpha)
    {
        return new Color(red / 255f, green / 255f, blue / 255f, alpha);
    }

    public static IEnumerator WaitForCompletionCoroutine(this Tween tween, Action callback = null)
    {
        yield return tween.WaitForCompletion();
        callback?.Invoke();
    }

    public static bool IsVoice(this ESfx sfx)
    {
        switch (sfx)
        {
            case ESfx.GoodJob:
            case ESfx.Great:
            case ESfx.Excellent:
            case ESfx.Perfect:
                return true;
            default:
                return false;
        }
    }
}