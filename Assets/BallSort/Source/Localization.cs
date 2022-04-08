using Assets.SimpleLocalization;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Localization : SingletonMonoBehaviour<Localization>
{
    [SerializeField] LocalizationSettings localizationSettings;

    public string CurrentLanguage => LocalizationManager.CurrentLanguage;

    private void Awake()
    {
        LocalizationManager.onLocalizationChanged += OnLocalizationChanged;
        LocalizationManager.onLocalizedObjectAdd += OnLocalizedObjectAdd;
    }

    public void Init()
    {
        LocalizationManager.defaultLanguage = localizationSettings.DefaultLanguage.languageName;
        LocalizationManager.Read();
        LocalizationManager.availableLanguages.Clear();
        foreach (var language in localizationSettings.FontNodes)
        {
            LocalizationManager.availableLanguages.Add(language.language, language.languageName);
        }

        string currentLanguage = PlayerPrefs.GetString("language");

        if (string.IsNullOrEmpty(currentLanguage))
        {
            if (LocalizationManager.availableLanguages.ContainsKey(Application.systemLanguage))
            {
                currentLanguage = LocalizationManager.availableLanguages[Application.systemLanguage];
                LocalizationManager.CurrentLanguage = currentLanguage;
                PlayerPrefs.SetString("language", currentLanguage);
                PlayerPrefs.Save();
            }
            else
            {
                LocalizationManager.SetDefaultLanguage();
            }
        }
        else
        {
            LocalizationManager.CurrentLanguage = currentLanguage;
        }
    }

    public string Localize(string localizationKey)
    {
        return LocalizationManager.Localize(localizationKey);
    }

    public string Localize(string localizationKey, params object[] args)
    {
        return LocalizationManager.Localize(localizationKey, args);
    }

    public IEnumerable<string> AvailableLanguages => LocalizationManager.availableLanguages.Values;

    public void SetLocalization(string localization)
    {
        LocalizationManager.CurrentLanguage = localization;
    }

    public void SetNextLanguage()
    {
        LocalizationManager.SetNextLanguage();
    }

    public void OnLocalizationChanged(string language)
    {
        foreach (var localizedObject in LocalizationManager.localizedObjects)
        {
            var textMeshPro = localizedObject.GetComponent<TextMeshProUGUI>();
            var localizedText = localizedObject.GetComponent<LocalizedText>();
            textMeshPro.text = LocalizationManager.Localize(localizedText.LocalizationKey);
            textMeshPro.font = localizationSettings.GetFontAsset(language);
        }
    }

    public void OnLocalizedObjectAdd(GameObject go)
    {
        Debug.Log($"OnLocalizedObjectAdd: {go.name}");
        var textMeshPro = go.GetComponent<TextMeshProUGUI>();
        var localizedText = go.GetComponent<LocalizedText>();
        textMeshPro.text = LocalizationManager.Localize(localizedText.LocalizationKey);
        textMeshPro.font = localizationSettings.GetFontAsset(CurrentLanguage);
    }

    private void OnDestroy()
    {
        LocalizationManager.onLocalizationChanged -= OnLocalizationChanged;
        LocalizationManager.onLocalizedObjectAdd -= OnLocalizedObjectAdd;
    }
}
