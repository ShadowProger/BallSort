using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : SingletonMonoBehaviour<ScreenManager>
{
    public WinUI winUI;
    public GameUI gameUI;
    public LevelUI levelUI;
    public SettingsUI settingsUI;

    public SpriteRenderer skinBack;
    public SpriteRenderer screenBack;
    public SpriteRenderer screenBackDouble;

    public void Init()
    {
        gameUI.Init();
        winUI.Init();
        levelUI.Init();
        settingsUI.Init();

        gameUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
        levelUI.gameObject.SetActive(false);
        settingsUI.gameObject.SetActive(false);
    }

    public void SetSkinBackVisible(bool isVisible)
    {
        skinBack.gameObject.SetActive(isVisible);
    }

    public void SetScreenBackVisible(bool isVisible)
    {
        screenBack.gameObject.SetActive(isVisible);
        screenBackDouble.gameObject.SetActive(isVisible);
    }

    public void SetFieldVisible(bool isVisible)
    {
        GameManager.Instance.SetFieldVisible(isVisible);
        gameUI.SetTutorialVisible(isVisible);
    }
}
