using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsController : MonoBehaviour
{
    [Header("App IDS")]
    public string AndroidAppID;
    public string IosAppID;

    public List<string> AndroidBlocks;
    public List<string> IOSBlocks;
   
    public static bool EnableDebugAds = true;

    public static List<AdsModule> modulesList;

    public void Start()
    {
        modulesList = new List<AdsModule>();

        switch (Application.platform) {
            case RuntimePlatform.Android:
                AndroidBlocks.ForEach(x => modulesList.Add(new AdsModule(x)));
                MobileAds.Initialize(AndroidAppID);
                break;
            case RuntimePlatform.IPhonePlayer:
                IOSBlocks.ForEach(x => modulesList.Add(new AdsModule(x)));
                MobileAds.Initialize(IosAppID);
                break;
        }

        foreach (AdsModule module in modulesList)
            module.LoadAd(() => { });
    }

    public static void Show(Action<bool> onAdResult)
    {
        Loading.Show();

        #if UNITY_EDITOR
            if (EnableDebugAds) { TimeUtility.Delay(2, () => { Loading.Hide(); onAdResult.Invoke(true); return; }); }
        #endif

        if (modulesList.Exists(x => x.rewardedAd.IsLoaded()))
        {
            Debug.Log("[ADS CONTROLLER] Loaded module exists!");
            foreach (AdsModule module in modulesList)
            {
                if (module.rewardedAd.IsLoaded())
                {
                    module.Show(x => TimeUtility.Delay(2, () => {
                        Loading.Hide();
                        onAdResult.Invoke(x);
                    }));
                    break;
                }
            }
        }
        else
        {
            Debug.Log("[ADS CONTROLLER] Loaded module NOT exists!");
            modulesList[modulesList.Count - 1].Show(x => TimeUtility.Delay(2, () => {
                Loading.Hide();
                onAdResult.Invoke(x);
            }));
        }
    }

}

public class AdsModule
{
    public RewardedAd rewardedAd;
    public string rewardedAdId;

    public Action onAdLoaded = null;
    public Action<bool> onAdResult = null;

    private void HandleUserEarnedReward(object sender, Reward e)
    {
        Debug.Log("[ " + this.rewardedAdId + " ] User Earned Reward");

        if (onAdResult != null) onAdResult.Invoke(true);
        if (!rewardedAd.IsLoaded()) LoadAd(() => { });
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e)
    {
        Debug.Log("[ " + this.rewardedAdId + " ] Rewarded Ad Closed");

        if (onAdResult != null) onAdResult.Invoke(false);
        if (!rewardedAd.IsLoaded()) LoadAd(() => { });
    }

    private void HandleRewardedAdLoaded(object sender, EventArgs e)
    {
        Debug.Log("[ " + this.rewardedAdId + " ] Rewarded Ad Loaded");
        if (onAdLoaded != null) onAdLoaded.Invoke();
    }

    public void LoadAd(Action onAdLoaded)
    {
        Debug.Log("[ " + this.rewardedAdId + " ] Called LoadAd()");

        rewardedAd = new RewardedAd(rewardedAdId);
        rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        rewardedAd.OnAdClosed += HandleRewardedAdClosed;
        rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        AdRequest request = new AdRequest.Builder().Build();
        rewardedAd.LoadAd(request);

        this.onAdLoaded = onAdLoaded;
    }

    public void Show(Action<bool> onAdResult)
    {
        Debug.Log("[ " + this.rewardedAdId + " ] Called Show()");
        this.onAdResult = onAdResult;
        if (rewardedAd.IsLoaded()) rewardedAd.Show();
        else LoadAd(() => rewardedAd.Show());
    }

    public AdsModule(string rewardedAdId)
    {
        this.rewardedAdId = rewardedAdId;
    }
}