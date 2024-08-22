using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;

[AddComponentMenu("GoogleAdMobAd/InterstitialAdController")]
public class InterstitialAdController : MonoBehaviour
{
#if RELEASE
    private const string m_AdUnitId = "ca-app-pub-4360672271681123/2274164724";
#else
#if UNITY_ANDROID
    private const string m_AdUnitId = "ca-app-pub-3940256099942544/1033173712";
#else
    private const string m_AdUnitId = "unused";
#endif
#endif 

    private InterstitialAd m_InterstitialAd;
    private bool m_IsLoading;
    private IEnumerator m_ShowAdCoroutine;
    private IEnumerator m_ReloadAdCoroutine;

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
        if (m_InterstitialAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading interstitial ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        InterstitialAd.Load(m_AdUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Interstitial ad failed to load an ad with error : " + error);
                UtilCoroutine.PlayCoroutine(ref m_ReloadAdCoroutine, ReloadAd(), this);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Interstitial load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            m_InterstitialAd = ad;

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
    /// Shows the ad.
    /// </summary>
    public void ShowAd(Action<Error> callback)
    {
        UtilCoroutine.PlayCoroutine(ref m_ShowAdCoroutine, ShowAdCoroutine(callback), this);
    }

    IEnumerator ShowAdCoroutine(Action<Error> callback)
    {
        var timeout = Config.AdLoadingTimeSec;
        var startTime = Time.time;

        var count = 1;
        while (m_IsLoading || null == m_InterstitialAd || !m_InterstitialAd.CanShowAd() || !Config.IsNetworkAvailable())
        {
            if (Time.time - startTime > timeout)
            {
                callback(Error.AdFailed);
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

        Debug.Log("Showing interstitial ad.");
        m_InterstitialAd.Show();

        // 콜백 호출
        callback(Error.Succeed);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (m_InterstitialAd != null)
        {
            Debug.Log("Destroying interstitial ad.");
            m_InterstitialAd.Destroy();
            m_InterstitialAd = null;
        }
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (m_InterstitialAd != null)
        {
            var responseInfo = m_InterstitialAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            LoadAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content with error : "
                + error);

            LoadAd();
        };
    }
}