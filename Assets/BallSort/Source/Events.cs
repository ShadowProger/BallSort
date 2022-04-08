using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Events
{
    public static Action onRestart;
    public static Action<int> onStart;
    public static Action onRewardedAdLoaded;
    public static Action onInterstitialAdLoaded;
    public static Action<bool> onAdInitialized;

    public static void Restart() { onRestart?.Invoke(); }
    public static void Start(int level) { onStart?.Invoke(level); }
    public static void RewardedAdLoaded() { onRewardedAdLoaded?.Invoke(); }
    public static void InterstitialAdLoaded() { onInterstitialAdLoaded?.Invoke(); }
    public static void AdInitialized(bool isInitialized) { onAdInitialized?.Invoke(isInitialized); }
}
