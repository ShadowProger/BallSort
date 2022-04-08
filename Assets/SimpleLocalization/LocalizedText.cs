using TMPro;
using UnityEngine;

namespace Assets.SimpleLocalization
{
	/// <summary>
	/// Localize text component.
	/// </summary>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class LocalizedText : MonoBehaviour
    {
        public string LocalizationKey;

        public void Start()
        {
            LocalizationManager.RegisterLocalizedObject(gameObject);
        }

        public void OnDestroy()
        {
            LocalizationManager.RemoveLocalizedObject(gameObject);
        }
    }
}