using UnityEngine;
using System;
using System.Collections;
using GoogleMobileAds.Api;

public class AdManager : Singleton<AdManager>
{
    private const int SHOW_INTERSTITIALAD_COUNT = 2;

#if UNITY_ANDROID
    //private string _adUnitId = "ca-app-pub-3940256099942544/1033173712";
    private string _adUnitId = "ca-app-pub-7841868880233499/2835543023";
#else
    private string _adUnitId = "unused";
#endif

    private InterstitialAd _interstitialAd;

    private System.Action _onCloseInterstitialAd;

    private bool _isCloseInterstitialAd;
    private int _showInterstitialAdCount;

    private void Start()
    {
        Debug.Log("AdManager Start.");

        _isCloseInterstitialAd = false;
        _showInterstitialAdCount = 0;

        MobileAds.Initialize((InitializationStatus status) =>
        {
            LoadInterstitialAd();
        });
    }

    private void Update()
    {
        // 전면 광고 닫기 콜백 처리.
        // 스레드 문제 때문에 업데이트에서 처리.
        if(_isCloseInterstitialAd)
        {
            _isCloseInterstitialAd = false;
            LoadInterstitialAd();
            _onCloseInterstitialAd?.Invoke();
        }
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
            _isCloseInterstitialAd = true;
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
        if(_showInterstitialAdCount < SHOW_INTERSTITIALAD_COUNT)
        {
            ++_showInterstitialAdCount;
            onClose?.Invoke();
        }
        else
        {
            _showInterstitialAdCount = 0;
            if (_interstitialAd != null && _interstitialAd.CanShowAd())
            {
                _onCloseInterstitialAd = onClose;
                _interstitialAd.Show();
            }
        }
    }
}
