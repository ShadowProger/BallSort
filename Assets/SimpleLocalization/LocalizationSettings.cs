using UnityEngine;
using TMPro;

[CreateAssetMenu]
public class LocalizationSettings : ScriptableObject
{
	public FontNode DefaultLanguage;
	public FontNode[] FontNodes;

	public bool AreSharingSameAsset(SystemLanguage a, SystemLanguage b)
	{
		return GetFontAsset(a) == GetFontAsset(b);
	}

	public bool AreSharingSameAsset(string a, string b)
	{
		return GetFontAsset(a) == GetFontAsset(b);
	}

	public TMP_FontAsset GetFontAsset(SystemLanguage language)
	{
		foreach (var fontNode in FontNodes)
		{
			if (fontNode.language == language)
			{
				return fontNode.TextAsset;
			}
		}

		return DefaultLanguage.TextAsset;
	}

	public TMP_FontAsset GetFontAsset(string language)
	{
		foreach (var fontNode in FontNodes)
		{
			if (fontNode.languageName == language)
			{
				return fontNode.TextAsset;
			}
		}

		return DefaultLanguage.TextAsset;
	}
}

[System.Serializable]
public class FontNode
{
	public SystemLanguage language;
	public string languageName;
	public TMP_FontAsset TextAsset;
}

