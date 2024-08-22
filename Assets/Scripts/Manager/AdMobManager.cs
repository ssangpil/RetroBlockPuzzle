using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoSingleton<AdMobManager>
{
    private BannerViewController m_BannerViewController;
    private InterstitialAdController m_InterstitialAdController;
    private RewardedAdController m_RewardedAdController;    

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) => { });  

        m_BannerViewController = GetComponent<BannerViewController>();
        m_InterstitialAdController = GetComponent<InterstitialAdController>();
        m_RewardedAdController = GetComponent<RewardedAdController>();
        m_BannerViewController.LoadAd();
        m_InterstitialAdController.LoadAd();
        m_RewardedAdController.LoadAd();
    }

    void OnDestroy() 
    {
        m_BannerViewController.DestroyAd();
        m_InterstitialAdController.DestroyAd();
        m_RewardedAdController.DestroyAd();
    }

    public void ShowRewardedAd(Action<Error> callback)
    {
        m_RewardedAdController.ShowAd(callback);
    }

    public void ShowInterstitialAd(Action<Error> callback)
    {
        m_InterstitialAdController.ShowAd(callback);
    }
}
