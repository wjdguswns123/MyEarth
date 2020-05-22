using UnityEngine;
using System;
using System.Collections;
using GoogleMobileAds.Api;

public class AdManager : Singleton<AdManager>
{
    BannerView     bannerAd;
    InterstitialAd interstitialAd;

	// Use this for initialization
	void Awake ()
    {
        //RequestBanner();
        RequestInterstitialAd();
    }

    //배너 광고 요청.
    void RequestBanner()
    {
        string addID;

#if UNITY_ANDROID
        addID = "ca-app-pub-7841868880233499/4607789693";
#else
        addID = "";
#endif
        bannerAd = new BannerView(addID, AdSize.SmartBanner, AdPosition.Top);
        AdRequest request = new AdRequest.Builder().Build();
        //테스트 기기 설정.
        //AdRequest request = new AdRequest.Builder().AddTestDevice(AdRequest.TestDeviceSimulator).AddTestDevice("351623070756261").Build();
        bannerAd.LoadAd(request);
    }

    //전면 광고 요청.
    void RequestInterstitialAd()
    {
#if UNITY_ANDROID
        interstitialAd = new InterstitialAd("ca-app-pub-7841868880233499/2835543023");
#else
        interstitialAd = new InterstitialAd("");
#endif
        AdRequest request = new AdRequest.Builder().Build();
        interstitialAd.LoadAd(request);
        interstitialAd.OnAdClosed += OnAdClose;
    }

    //전면 광고 닫기 처리.
    void OnAdClose(object sender, EventArgs e)
    {
        interstitialAd.Destroy();
        RequestInterstitialAd();
    }

    //배너 광고 보여주기.
    public void ShowBannerAd()
    {
        bannerAd.Show();
    }

    //전면 광고 보여주기.
    public void ShowInterstitialAd()
    {
        interstitialAd.Show();
    }
}
