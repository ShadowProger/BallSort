using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUI : Window
{
    [SerializeField] Button settingsButton;
    [SerializeField] Button closeButton;
    [SerializeField] Button unlockButton;
    [SerializeField] Button AdsButton;

    [SerializeField] MySlider pageSlider;
    [SerializeField] Image sliderImage;

    [SerializeField] SpriteRenderer back;
    [SerializeField] SpriteRenderer backDouble;

    [SerializeField] GameObject[] pages;

    [SerializeField] Sprite[] backSprites;
    [SerializeField] Color[] lightColors;
    [SerializeField] Color[] pageSpriteColors;
    [SerializeField] Sprite[] pageSprites;
    [SerializeField] Graphic[] lights;

    private int lastOpenedPage = -1;
    private Tween tween;
    private Color startBackColor;

    public override void Init()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        settingsButton.onClick.AddListener(OnSettingsButtonClick);

        pageSlider.onValueChanged.AddListener(OnPageSliderChanged);
        pageSlider.onPointerUp.AddListener(AfterPageSliderChanged);
        pageSlider.onPointerDown.AddListener(OnPageSliderDown);
        startBackColor = back.color;
    }

    public override void Open()
    {
        base.Open();

        if (lastOpenedPage < 0)
        {
            SetPage(0);
        }

        gameObject.SetActive(true);
        ScreenManager.Instance.gameUI.Close();
        ScreenManager.Instance.screenBack.sprite = backSprites[lastOpenedPage];
        ScreenManager.Instance.SetFieldVisible(false);
        ScreenManager.Instance.SetScreenBackVisible(true);
        ScreenManager.Instance.SetSkinBackVisible(false);

        pageSlider.value = lastOpenedPage;
        SetPage(lastOpenedPage);
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
        ScreenManager.Instance.gameUI.Open();
        ScreenManager.Instance.SetFieldVisible(true);
        ScreenManager.Instance.SetScreenBackVisible(false);
        ScreenManager.Instance.SetSkinBackVisible(true);
    }

    public void OnCloseButtonClick()
    {
        Close();
    }

    public void OnSettingsButtonClick()
    {
        Close();
        ScreenManager.Instance.settingsUI.Open();
    }

    public void OnPageSliderChanged(float value)
    {
        int page = Mathf.RoundToInt(pageSlider.value);
        SetPage(page);

        Debug.Log($"OnLangChanged");
    }

    public void AfterPageSliderChanged()
    {
        int page = Mathf.RoundToInt(pageSlider.value);
        pageSlider.value = page;

        Debug.Log($"AfterLangChanged: {page}");
    }

    public void OnPageSliderDown()
    {
        int page = Mathf.RoundToInt(pageSlider.value);
        pageSlider.value = page;
        SetPage(page);
    }

    private void SetPage(int page)
    {
        if (page != lastOpenedPage)
        {
            lastOpenedPage = page;
            sliderImage.sprite = pageSprites[page];
            sliderImage.color = pageSpriteColors[page];

            foreach (var light in lights)
            {
                light.color = lightColors[page];
            }

            for (int i = 0; i < pages.Length; i++)
            {
                Debug.Log($"i = {i}, page = {page}");
                pages[i].SetActive(i == page);
            }

            //ScreenManager.Instance.screenBack.sprite = backSprites[page];
            
            if (tween != null)
            {
                tween.Kill();
            }

            backDouble.sprite = backSprites[page];
            Color endBackColor = startBackColor;
            endBackColor.a = 0;
            Sprite backSprite = backSprites[page];

            tween = back.DOColor(endBackColor, 1f)
                .OnComplete(() => 
                {
                    back.sprite = backSprite;
                    back.color = startBackColor;
                })
                .OnKill(() =>
                {
                    back.sprite = backSprite;
                    back.color = startBackColor;
                });
        }
    }
}
