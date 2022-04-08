using Assets.SimpleLocalization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : Window
{
    [SerializeField] Button restartButton;
    [SerializeField] Button addButton;
    [SerializeField] Button reverseButton;
    [SerializeField] Button menuButton;
    [SerializeField] Button levelButton;
    [SerializeField] TMP_Text reverseCount;
    [SerializeField] TMP_Text level;
    [SerializeField] TMP_Text tutorial;

    [SerializeField] Color activeTextColor;
    [SerializeField] Color inactiveTextColor;
    [SerializeField] Graphic[] addGraphics;
    [SerializeField] Graphic[] reverseGraphics;

    [SerializeField] GameObject addAdsIcon;
    [SerializeField] GameObject reverseAdsIcon;
    [SerializeField] GameObject reverseIcon;
    [SerializeField] GameObject reverseAdsCount;

    [Header("Debug")]
    [SerializeField] GameObject debugPanel;
    [SerializeField] Button OpenAllButton;
    [SerializeField] Button WinButton;
    [SerializeField] Button NextButton;

    private string localizationKey;
    private bool isTutorialVisible;

    public override void Init()
    {
        restartButton.onClick.AddListener(OnRestartButtonClick);
        addButton.onClick.AddListener(OnAddButtonClick);
        reverseButton.onClick.AddListener(OnReverseButtonClick);
        menuButton.onClick.AddListener(OnMenuButtonClick);
        levelButton.onClick.AddListener(OnLevelButtonClick);
        Events.onRestart += OnRestart;
        Events.onStart += OnStart;
        Events.onRewardedAdLoaded += OnRewardedLoaded;
        Events.onAdInitialized += OnAdInitialized;
        LocalizationManager.onLocalizationChanged += OnLocalizationChanged;

#if MYDEBUG
        debugPanel.SetActive(true);
        OpenAllButton.onClick.AddListener(OnOpenAllButtonClick);
        WinButton.onClick.AddListener(OnWinButtonClick);
        NextButton.onClick.AddListener(OnNextButtonClick);
#endif
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        ScreenManager.Instance.SetScreenBackVisible(false);
        ScreenManager.Instance.SetSkinBackVisible(true);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
    }

    public void SetAddButtonState(bool forceInactive = false)
    {
        bool isAvailable = AdMob.Instance.IsRewardedAvailable && GameManager.Instance.CanAddContainer;

        if (forceInactive)
        {
            isAvailable = false;
        }

        foreach (var graphic in addGraphics)
        {
            graphic.color = isAvailable ? activeTextColor : inactiveTextColor;
        }

        addAdsIcon.SetActive(isAvailable);
    }

    public void SetReverseButtonState()
    {
        bool isAvailable = AdMob.Instance.IsRewardedAvailable;
        bool canReverse = GameManager.Instance.ReverseCount > 0;

        foreach (var graphic in reverseGraphics)
        {
            graphic.color = isAvailable || canReverse ? activeTextColor : inactiveTextColor;
        }

        reverseAdsIcon.SetActive(isAvailable && !canReverse);
        reverseCount.text = $"{GameManager.Instance.ReverseCount}";
        reverseAdsCount.SetActive(isAvailable && !canReverse);
        reverseCount.gameObject.SetActive(!(isAvailable && !canReverse));
        reverseIcon.SetActive(!(isAvailable && !canReverse));
    }

    void OnRewardedLoaded()
    {
        SetAddButtonState();
        SetReverseButtonState();
    }

    void OnAdInitialized(bool isInitialized)
    {
        SetAddButtonState();
        SetReverseButtonState();
    }

    void OnRestart()
    {
        SetAddButtonState();
        SetReverseButtonState();
    }

    void OnStart(int level)
    {
        OnLocalizationChanged(Localization.Instance.CurrentLanguage);
        SetAddButtonState();
        SetReverseButtonState();

        if (level == 0)
        {
            tutorial.gameObject.SetActive(true);
            isTutorialVisible = true;
            localizationKey = "tutorial.first";
            tutorial.text = Localization.Instance.Localize(localizationKey);
        }
        else if (level == 1)
        {
            tutorial.gameObject.SetActive(true);
            isTutorialVisible = true;
            localizationKey = "tutorial.second";
            tutorial.text = Localization.Instance.Localize(localizationKey);
        }
        else
        {
            tutorial.gameObject.SetActive(false);
            isTutorialVisible = false;
        }
    }

    public void OnRestartButtonClick()
    {
        GameManager.Instance.ShowInterstitial();
        GameManager.Instance.Restart();
    }

    public void OnLevelButtonClick()
    {
        ScreenManager.Instance.levelUI.Open();
    }

    public void OnMenuButtonClick()
    {
        //Localization.Instance.SetNextLanguage();
        ScreenManager.Instance.settingsUI.Open();
    }

    public void OnAddButtonClick()
    {
        if (!GameManager.Instance.CanAddContainer)
        {
            return;
        }

        if (AdMob.Instance.IsRewardedLoaded)
        {
            FirebaseManager.Instance.Event("RewardedVideo", "placement", "addContainer");
            AdMob.Instance.ShowRewardedVideo(() => { GameManager.Instance.AddContainer(); });
        }

        SetAddButtonState(true);
    }

    public void OnReverseButtonClick()
    {
        if (GameManager.Instance.ReverseCount > 0)
        {
            GameManager.Instance.ReverseTurn();
        }
        else
        {
            if (AdMob.Instance.IsRewardedLoaded)
            {
                FirebaseManager.Instance.Event("RewardedVideo", "placement", "reverse");
                AdMob.Instance.ShowRewardedVideo(() => 
                { 
                    GameManager.Instance.ReverseCount = GameManager.MAX_REVERSE_COUNT;
                    SetReverseButtonState();
                });
            }
        }

        SetReverseButtonState();
    }

    public void OnOpenAllButtonClick()
    {
        GameManager.Instance.OpenAllLevels();
    }

    public void OnWinButtonClick()
    {
        GameManager.Instance.Win();
    }

    public void OnNextButtonClick()
    {
        GameManager.Instance.StartLevel(GameManager.Instance.currentLevelNumber + 1);
    }

    public void OnLocalizationChanged(string language)
    {
        level.text = $"{Localization.Instance.Localize("ui.level")} {GameManager.Instance.currentLevelNumber + 1}";
        if (!string.IsNullOrEmpty(localizationKey))
        {
            tutorial.text = Localization.Instance.Localize(localizationKey);
        }
    }

    public void SetTutorialVisible(bool isVisible)
    {
        tutorial.gameObject.SetActive(isVisible ? isTutorialVisible : isVisible);
    }

    private void OnDestroy()
    {
        Events.onRestart -= OnRestart;
        Events.onStart -= OnStart;
        Events.onAdInitialized -= OnAdInitialized;
        Events.onRewardedAdLoaded -= OnRewardedLoaded;
        LocalizationManager.onLocalizationChanged -= OnLocalizationChanged;
    }
}
