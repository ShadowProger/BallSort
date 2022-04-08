using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMob: MonoBehaviour
{
    private static AdMob instance;

    InterstitialAd interstitialAd;
    RewardedAd rewardedAd;

    [SerializeField] bool test;

    [SerializeField] string interstitialId;
    [SerializeField] string rewardedId;

    [SerializeField] string[] testDevices;

    public bool IsInterstitialLoaded { get; private set; }
    public bool IsRewardedLoaded { get; private set; }
    public bool IsInitialized { get; private set; }
    public bool IsRewardedAvailable => IsInitialized && IsRewardedLoaded;

    public static AdMob Instance => instance;

    void Awake()
    {
        Debug.Log($"[AdMob] Init");

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (test)
        {
            interstitialId = "ca-app-pub-3940256099942544/1033173712";
            rewardedId = "ca-app-pub-3940256099942544/5224354917";
        }

        List<string> deviceIds = new List<string>(testDevices);
        RequestConfiguration requestConfiguration = new RequestConfiguration
            .Builder()
            .SetTestDeviceIds(deviceIds)
            .build();
        MobileAds.SetRequestConfiguration(requestConfiguration);
        MobileAds.Initialize(OnInitialize);

        //RequestAndLoadInterstitialAd();
        //RequestAndLoadRewardedAd();
    }

    private void OnInitialize(InitializationStatus initStatus)
    {
        Execute(() => 
        {
            var map = initStatus.getAdapterStatusMap();
            IsInitialized = true;
            foreach (var pair in map)
            {
                Debug.Log($"key: {pair.Key}, value: {pair.Value.InitializationState}");
                if (pair.Value.InitializationState != AdapterState.Ready)
                {
                    IsInitialized = false;
                    break;
                }
            }

            if (IsInitialized)
            {
                Debug.Log($"[AdMob] Initialized");

                RequestAndLoadInterstitialAd();
                RequestAndLoadRewardedAd();
            }

            Events.AdInitialized(IsInitialized);
        });
    }

    void OnDestroy()
    {
        interstitialAd?.Destroy();
    }

    private AdRequest RequestAd()
    {
        string consent = GDPR.AdsConsent ? "0" : "1";
        return new AdRequest.Builder().AddExtra("npa", consent).Build();
    }

    #region INTERSTITIAL
    public void RequestAndLoadInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }

        interstitialAd = new InterstitialAd(interstitialId);
        IsInterstitialLoaded = false;

        interstitialAd.OnAdClosed += (sender, args) => RequestAndLoadInterstitialAd();
        interstitialAd.OnAdLoaded += (sender, args) => 
        {
            IsInterstitialLoaded = true;
            Events.InterstitialAdLoaded(); 
        };

        AdRequest request = RequestAd();
        interstitialAd.LoadAd(request);
    }

    public void ShowInterstitial()
    {
        if (interstitialAd.IsLoaded())
        {
            interstitialAd.Show();
        }
    }
    #endregion

    #region REWARDED
    public void RequestAndLoadRewardedAd()
    {
        rewardedAd = new RewardedAd(rewardedId);
        IsRewardedLoaded = false;

        rewardedAd.OnAdClosed += (sender, args) => { RequestAndLoadRewardedAd(); };
        rewardedAd.OnAdLoaded += (sender, args) => 
        {
            IsRewardedLoaded = true;
            Events.RewardedAdLoaded(); 
        };

        AdRequest request = RequestAd();
        rewardedAd.LoadAd(request);
    }

    public void ShowRewardedVideo(Action successCallback)
    {
#if UNITY_EDITOR
        successCallback?.Invoke();
        return;
#endif
        if (rewardedAd != null)
        {
            if (rewardedAd.IsLoaded())
            {
                rewardedAd.Show();
                rewardedAd.OnUserEarnedReward += (sender, args) =>
                {
                    Execute(successCallback);
                };
            }
        }
    }
    #endregion

    void Execute(Action callback)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            callback?.Invoke();
            callback = null;
        });
    }
}
