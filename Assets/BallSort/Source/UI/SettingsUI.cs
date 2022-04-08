using Manybits;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : Window
{
    [SerializeField] Button closeButton;
    [SerializeField] Button noAdsButton;
    [SerializeField] Button rateUsButton;
    [SerializeField] Button privacyButton;

    [SerializeField] MySlider langSlider;
    [SerializeField] TMP_Text sliderText;

    [SerializeField] Sprite backSprite;

    public override void Init()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        noAdsButton.onClick.AddListener(OnAdsButtonClick);
        rateUsButton.onClick.AddListener(OnRateButtonClick);
        privacyButton.onClick.AddListener(OnPrivacyButtonClick);

        langSlider.onPointerUp.AddListener(AfterLangChanged);
        langSlider.onValueChanged.AddListener(OnLangChanged);
        langSlider.onPointerDown.AddListener(OnLangDown);
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        ScreenManager.Instance.screenBack.sprite = backSprite;
        //ScreenManager.Instance.gameUI.Close();
        ScreenManager.Instance.SetFieldVisible(false);
        ScreenManager.Instance.SetScreenBackVisible(true);
        ScreenManager.Instance.SetSkinBackVisible(false);

        string lang = Localization.Instance.CurrentLanguage;
        langSlider.value = lang == "EN" ? 1f : 0f;
        sliderText.text = lang == "EN" ? "ENG" : "RUS";
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
        //ScreenManager.Instance.gameUI.Open();
        ScreenManager.Instance.SetFieldVisible(true);
        ScreenManager.Instance.SetScreenBackVisible(false);
        ScreenManager.Instance.SetSkinBackVisible(true);
    }

    public void OnCloseButtonClick()
    {
        Close();
    }

    public void OnAdsButtonClick()
    {
        FirebaseManager.Instance.Event("OnAdsButtonClick");
        PurchaseManager.Instance.BuyProductID("no_ads");
    }

    public void OnRateButtonClick()
    {
        ShareAndRate.Instance.RateUs();
    }

    public void OnPrivacyButtonClick()
    {
        ShareAndRate.Instance.OpenPrivacyPolicy();
    }

    public void OnLangChanged(float value)
    {
        string lang = langSlider.value <= 0.5 ? "RUS" : "ENG";
        sliderText.text = lang;

        Debug.Log($"OnLangChanged");
    }

    public void AfterLangChanged()
    {
        float value = langSlider.value <= 0.5 ? 0f : 1f;
        langSlider.value = value;

        string lang = langSlider.value <= 0.5 ? "RU" : "EN";
        Localization.Instance.SetLocalization(lang);
        PlayerPrefs.SetString("language", lang);
        PlayerPrefs.Save();

        Debug.Log($"AfterLangChanged: {lang}");
    }

    public void OnLangDown()
    {
        float value = langSlider.value <= 0.5 ? 0f : 1f;
        langSlider.value = value;
    }
}
