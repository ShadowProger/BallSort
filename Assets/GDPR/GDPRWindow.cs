using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manybits
{
    public class GDPRWindow : MonoBehaviour
    {
        static GDPRWindow instance;
        [SerializeField] Button playButton;
        [SerializeField] Button privacyButton;
        [SerializeField] Toggle confirmToggle;
        [SerializeField] Text titleText;
        [SerializeField] Text descriptionText;
        [SerializeField] Text confirmationText;

        [SerializeField] string companyName;
        [SerializeField] string productName;

        [HideInInspector] public bool gdprConsent;
        [SerializeField] string privacyUrl;

        private string title = "Thank you for installing\n{0}!";
        private string description = "Here at {0}, we respect and value your privacy and data security and we would like to get your consent on processing your game data in making {1} better and show you only relevant ads";
        private string confirmation = "I agree to the terms of {0} and their Partners, and I confirm that I'm older than 16 years or have my guardians permission";

        static Action actionAfterClose;

        public static GDPRWindow Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject obj = Instantiate(Resources.Load<GameObject>("GDPRWindow"));
                    instance = obj.GetComponent<GDPRWindow>();
                    obj.SetActive(false);
                }

                return instance;
            }
        }



        private void Awake()
        {
            playButton.onClick.AddListener(OnPlayClick);
            privacyButton.onClick.AddListener(OnPrivacyClick);
            confirmToggle.onValueChanged.AddListener(OnConfirmValueChanged);
            titleText.text = string.Format(title, productName);
            descriptionText.text = string.Format(description, companyName, productName);
            confirmationText.text = string.Format(confirmation, companyName);
        }

        public void OnPlayClick()
        {
            Close();
            GDPR.AdsConsent = confirmToggle.isOn;
            GDPR.AnalyticsConsent = confirmToggle.isOn;
            GDPR.ConsentIsSelect = true;
            actionAfterClose?.Invoke();
            actionAfterClose = null;
        }

        public void OnPrivacyClick()
        {
            OpenPrivacyPolicy();
        }

        public void OnConfirmValueChanged(bool value)
        {
            playButton.interactable = value;
        }

        public void Show(Action callback)
        {
            actionAfterClose = callback;
            gameObject.SetActive(true);
        }

        public void Close()
        {
            Destroy(gameObject);
        }

        public void OpenPrivacyPolicy()
        {
#if UNITY_ANDROID
            Application.OpenURL(privacyUrl);
#elif UNITY_IPHONE
			Application.OpenURL("itms-apps://itunes.apple.com/app/idYOUR_ID");
#endif
        }

        private void OnDestroy()
        {
            actionAfterClose = null;
            instance = null;
        }
    }
}
