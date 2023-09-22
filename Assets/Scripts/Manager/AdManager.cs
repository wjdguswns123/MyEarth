using UnityEngine;
using System;
using System.Collections;
using GoogleMobileAds.Api;

public class AdManager : Singleton<AdManager>
{
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-3940256099942544/6978759866";
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    private System.Action _onCloseInterstitialAd;

    private void Start()
    {
        Debug.Log("AdManager Start.");
        MobileAds.Initialize((InitializationStatus status) =>
        {
            LoadInterstitialAd();
        });
    }

    /// <summary>
    /// 전면 광고 로드.
    /// </summary>
    private void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("InterstitialAd Load Start.");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_adUnitId, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("interstitial ad failed to load an ad with error : " + error);
                return;
            }
            _interstitialAd = ad;
            RegisterEventHandlers(ad);

            Debug.Log("InterstitialAd Load Complete.");
        });
    }

    /// <summary>
    /// 전면 광고 이벤트 등록.
    /// </summary>
    /// <param name="ad"></param>
    private void RegisterEventHandlers(InterstitialAd ad)
    {
        ad.OnAdPaid += (AdValue value) =>
        {
            Debug.Log(string.Format("Rewarded interstitial ad paid {0} {1}.", value.Value, value.CurrencyCode));
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded interstitial ad recorded an impression");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded interstitial ad was clicked.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded interstitial ad full screen content closed.");
            LoadInterstitialAd();
            _onCloseInterstitialAd?.Invoke();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded interstitial ad failed to open full screen content with error : " + error);
        };
    }

    /// <summary>
    /// 전면 광고 보여주기.
    /// </summary>
    /// <param name="onClose"> 광고 종료 콜백. </param>
    public void ShowInterstitialAd(System.Action onClose)
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            _onCloseInterstitialAd = onClose;
            _interstitialAd.Show();
        }
    }
}
