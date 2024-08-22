using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

[AddComponentMenu("GoogleAdMobAd/RewardedAdController")]
public class RewardedAdController : MonoBehaviour
{
#if RELEASE
    private const string m_AdUnitId = "ca-app-pub-4360672271681123/7662222777";
#else
#if UNITY_ANDROID
    private const string m_AdUnitId = "ca-app-pub-3940256099942544/5224354917";
#else
    private const string m_AdUnitId = "unused";
#endif
#endif 

    private RewardedAd m_RewardedAd;
    private bool m_IsLoading;
    private IEnumerator m_ShowAdCoroutine;
    private IEnumerator m_ReloadAdCoroutine;

    public bool IsLoaded => null != m_RewardedAd && m_RewardedAd.CanShowAd();

    private bool m_NetworkDisconnected;

    void Update()
    {
        if (!Config.IsNetworkAvailable())
        {
            m_NetworkDisconnected = true;
        }

        if (m_NetworkDisconnected && Config.IsNetworkAvailable())
        {
            m_NetworkDisconnected = false;
            LoadAd();
        }        
    }

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        if (m_IsLoading)
            return;

        m_IsLoading = true;

        // Clean up the old ad before loading a new one.
        if (m_RewardedAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading rewarded ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(m_AdUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                UtilCoroutine.PlayCoroutine(ref m_ReloadAdCoroutine, ReloadAd(), this);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            m_RewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);

            m_IsLoading = false;
        });
    }

    IEnumerator ReloadAd()
    {
        m_IsLoading = false;
        yield return new WaitForSeconds(3f);
        LoadAd();
    }


    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (m_RewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            m_RewardedAd.Destroy();
            m_RewardedAd = null;
        }
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (m_RewardedAd != null)
        {
            var responseInfo = m_RewardedAd.GetResponseInfo();
            Debug.Log(responseInfo);
        }
    }

    public void ShowAd(Action<Error> callback)
    {
        UtilCoroutine.PlayCoroutine(ref m_ShowAdCoroutine, ShowAdCoroutine(callback), this);
    }

    IEnumerator ShowAdCoroutine(Action<Error> callback)
    {
        float timeout = Config.AdLoadingTimeSec;
        float startTime = Time.time;

        var count = 1;
        while (m_IsLoading || null == m_RewardedAd || !m_RewardedAd.CanShowAd() || !Config.IsNetworkAvailable())
        {
            if (Time.time - startTime > timeout)
            {
                var error = new Error { ErrorCode = -2, Message = "Ad loading failed. Revive is not possible." };
                callback(error);
                yield break;
            }

            var str = new string('.', count);
            PanelManager.Instance.SetNoticePopup(ELogLevel.INFO.ToString(), "Loading" + str, null, false);

            if (5 < ++count)
                count = 1;

            yield return new WaitForSeconds(1f);
        }

        if (PanelManager.Instance.IsNoticePopup)
            PanelManager.HideLastPanel();

        m_RewardedAd.Show((Reward reward) =>
        {
            Debug.Log(String.Format("Rewarded ad rewarded the user. Type: {0}, amount: {1}.", reward.Type, reward.Amount));
            callback(Error.Succeed);
        });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
            LoadAd();
        };
    }
}
