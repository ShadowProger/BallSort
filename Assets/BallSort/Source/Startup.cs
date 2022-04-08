using Assets.SimpleLocalization;
using Manybits;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Startup : MonoBehaviour
{
    public ScreenManager screenManager;
    public GameManager gameManager;
    public Localization localization;

    public GameObject admob;
    public GameObject shareAndRate;
    public GameObject firebase;
    public GameObject purchaseManager;

    private void Start()
    {
        Debug.Log($"[Startup] Start");

        if (!GDPR.ConsentIsSelect)
        {
            GDPRWindow.Instance.Show(() =>
            {
                Debug.Log($"[GDPRWindow] Show callback");
                Init();
            });
        }
        else
        {
            Init();
        }
    }

    public void Init()
    {
        Debug.Log($"[Startup] Init");
        var root = GameObject.Find("[Services]").transform;
        Create(shareAndRate, root);
        Create(admob, root);

        Debug.Log($"Analytics Init: consent = {GDPR.AnalyticsConsent}");
        if (GDPR.AnalyticsConsent)
        {
            Create(firebase, root);
            FirebaseManager.Instance.Init();
        }
        Create(purchaseManager, root);

        localization.Init();
        string lang = PlayerPrefs.GetString("language");
        localization.SetLocalization(lang);

        screenManager.Init();
        gameManager.Init();
        screenManager.gameUI.Open();
    }

    private void Create(GameObject prefab, Transform parent = null, bool DontDestroy = true)
    {
        var instance = Instantiate<GameObject>(prefab);
        if (DontDestroy)
        {
            DontDestroyOnLoad(instance);
        }
    }
}
