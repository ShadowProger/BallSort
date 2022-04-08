using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Assets.SimpleLocalization
{
	/// <summary>
	/// Localization manager.
	/// </summary>
    public static class LocalizationManager
    {
        /// <summary>
        /// Fired when localization changed.
        /// </summary>
        public static event Action<string> onLocalizationChanged;
        public static event Action<GameObject> onLocalizedObjectAdd;
 
        public static List<GameObject> localizedObjects = new List<GameObject>();

        public static Dictionary<SystemLanguage, string> availableLanguages = new Dictionary<SystemLanguage, string>();

        private static readonly Dictionary<string, Dictionary<string, string>> Dictionary = new Dictionary<string, Dictionary<string, string>>();
        public static string defaultLanguage = "EN";
        private static string _language = defaultLanguage;

        private static void LocalizationChanged(string language) { onLocalizationChanged?.Invoke(language); }
        private static void LocalizedObjectAdd(GameObject go) { onLocalizedObjectAdd?.Invoke(go); }

		/// <summary>
		/// Get or set language.
		/// </summary>
        public static string CurrentLanguage
        {
            get { return _language; }
            set 
            {
                var langList = Dictionary.Keys.ToList();
                _language = langList.Contains(value) ? value : defaultLanguage;

                LocalizationChanged(_language); 
            }
        }

		/// <summary>
		/// Set default language.
		/// </summary>
        public static void SetDefaultLanguage()
        {
            CurrentLanguage = defaultLanguage;
        }

        public static void SetNextLanguage()
        {
            var langCount = availableLanguages.Keys.Count;
            var langList = availableLanguages.Values.ToList();
            var currentIndex = langList.IndexOf(_language);
            var nextIndex = (currentIndex + 1) % langCount;
            var nextLanguage = langList[nextIndex];

            CurrentLanguage = nextLanguage;
        }

        public static void RegisterLocalizedObject(GameObject go)
        {
            Debug.Log($"Register {go.name}");
            localizedObjects.Add(go);
            LocalizedObjectAdd(go);
        }

        public static void RemoveLocalizedObject(GameObject go)
        {
            localizedObjects.Remove(go);
        }

        /// <summary>
        /// Read localization spreadsheets.
        /// </summary>
        public static void Read(string path = "Localization")
        {
            if (Dictionary.Count > 0) return;

            var textAssets = Resources.LoadAll<TextAsset>(path);

            foreach (var textAsset in textAssets)
            {
                var text = ReplaceMarkers(textAsset.text);
                var matches = Regex.Matches(text, "\"[\\s\\S]*?\"");

                foreach (Match match in matches)
                {
					text = text.Replace(match.Value, match.Value
                        //.Replace("\"", null)
                        .Replace(",", "[comma]")
                        .Replace("\n", "[newline]"));
                }

                var lines = text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
				var languages = lines[0].Split(',').Select(i => i.Trim()).ToList();

				for (var i = 1; i < languages.Count; i++)
                {
                    if (!Dictionary.ContainsKey(languages[i]))
                    {
                        if (!Dictionary.ContainsKey(languages[i]))
                        {
                            Dictionary.Add(languages[i], new Dictionary<string, string>());
                        }
                    }
                }

                for (var i = 1; i < lines.Length; i++)
                {
					var columns = lines[i].Split(',').Select(j => j.Trim()).Select(j => j.Replace("[comma]", ",").Replace("[newline]", "\n")).ToList();
					var key = columns[0];

                    for (var j = 1; j < languages.Count; j++)
                    {
                        if (!Dictionary[languages[j]].ContainsKey(key))
                        {
                            var value = columns[j];

                            if (value.Contains("\""))
                            {
                                value = value.Substring(1, value.Length - 2).Replace("\"\"", "\"");
                            }

                            if (string.IsNullOrEmpty(value))
                            {
                                value = "No text";
                            }
                            Dictionary[languages[j]].Add(key, value);
                        }
                    }
                }
            }

            SetDefaultLanguage();
        }

		/// <summary>
		/// Get localized value by localization key.
		/// </summary>
        public static string Localize(string localizationKey)
        {
            if (Dictionary.Count == 0)
            {
                Read();
            }

            if (!Dictionary.ContainsKey(CurrentLanguage)) throw new KeyNotFoundException("Language not found: " + CurrentLanguage);
            if (!Dictionary[CurrentLanguage].ContainsKey(localizationKey)) return "Wrong Key"; //  throw new KeyNotFoundException("Translation not found: " + localizationKey);

            return Dictionary[CurrentLanguage][localizationKey];
        }

	    /// <summary>
	    /// Get localized value by localization key.
	    /// </summary>
		public static string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }

        private static string ReplaceMarkers(string text)
        {
            return text.Replace("[Newline]", "\n");
        }
    }
}