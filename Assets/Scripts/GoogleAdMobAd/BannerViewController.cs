using UnityEngine;
using GoogleMobileAds.Api;
using System;
using System.Collections;

[AddComponentMenu("GoogleAdMobAd/BannerViewController")]
public class BannerViewController : MonoBehaviour
{
#if RELEASE
    private const string m_AdUnitId = "ca-app-pub-4360672271681123/7598643598";
#else
#if UNITY_ANDROID
    private const string m_AdUnitId = "ca-app-pub-3940256099942544/6300978111";
#else
    private const string m_AdUnitId = "unused";
#endif
#endif //RELEASE

    private BannerView m_BannerView;
    private IEnumerator m_ReloadAdCoroutine;
    private bool m_IsLoading;
    private bool m_IsPaused;
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

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            m_IsPaused = true;
        }
        else
        {
            if (m_IsPaused)
            {
                m_IsPaused = false;
                LoadAd();
            }
        }
    }

    /// <summary>
    /// Creates a 320x50 banner at top of the screen.
    /// </summary>
    public void CreateBannerView()
    {
        //Debug.Log("Creating banner view.");

        // If we already have a banner, destroy the old one.
        if(m_BannerView != null)
        {
            DestroyAd();
        }

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        // Create a 320x50 banner at top of the screen.
        m_BannerView = new BannerView(m_AdUnitId, adaptiveSize, AdPosition.Bottom);

        // Listen to events the banner may raise.
        ListenToAdEvents();

        //Debug.Log("Banner view created.");
    }
    /// <summary>
    /// Creates the banner view and loads a banner ad.
    /// </summary>
    public void LoadAd()
    {
        if (m_IsLoading)
            return;

        m_IsLoading = true;

        // Create an instance of a banner view first.
        // if(m_BannerView == null)
        // {
        //     CreateBannerView();
        // }

        CreateBannerView();

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        Debug.Log("Loading banner ad.");
        m_BannerView.LoadAd(adRequest);
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public void ShowAd()
    {
        if (m_BannerView != null)
        {
            Debug.Log("Showing banner view.");
            m_BannerView.Show();
        }
    }

    /// <summary>
    /// Hides the ad.
    /// </summary>
    public void HideAd()
    {
        if (m_BannerView != null)
        {
            //Debug.Log("Hiding banner view.");
            m_BannerView.Hide();
        }
    }

    /// <summary>
    /// Destroys the ad.
    /// When you are finished with a BannerView, make sure to call
    /// the Destroy() method before dropping your reference to it.
    /// </summary>
    public void DestroyAd()
    {
        if (m_BannerView != null)
        {
            //Debug.Log("Destroying banner view.");
            m_BannerView.Destroy();
            m_BannerView = null;
        }
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (m_BannerView != null)
        {
            var responseInfo = m_BannerView.GetResponseInfo();
            if (responseInfo != null)
            {
                Debug.Log(responseInfo);
            }
        }
    }

    /// <summary>
    /// Listen to events the banner may raise.
    /// </summary>
    private void ListenToAdEvents()
    {
        // Raised when an ad is loaded into the banner view.
        m_BannerView.OnBannerAdLoaded += () =>
        {
            var msg = "Banner view loaded an ad with response : "
                + m_BannerView.GetResponseInfo();
            Debug.Log(msg);      
            m_IsLoading = false;      
        };
        // Raised when an ad fails to load into the banner view.
        m_BannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            var msg = "Banner view failed to load an ad with error : " + error;
            Debug.Log(msg);            
            UtilCoroutine.PlayCoroutine(ref m_ReloadAdCoroutine, ReloadAd(), this);
        };
        // Raised when the ad is estimated to have earned money.
        m_BannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        m_BannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        m_BannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        // Raised when an ad opened full screen content.
        m_BannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        m_BannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    IEnumerator ReloadAd()
    {
        m_IsLoading = false;
        yield return new WaitForSeconds(3f);
        LoadAd();
    }
}