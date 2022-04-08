using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GDPR
{
    //public static bool Consent
    //{
    //    get
    //    {
    //        return PlayerPrefs.GetInt("GDPR_CONSENT", 0) == 1;
    //    }
    //    set
    //    {
    //        PlayerPrefs.SetInt("GDPR_CONSENT", value ? 1 : 0);
    //        PlayerPrefs.Save();
    //    }
    //}

    public static bool AdsConsent
    {
        get
        {
            return PlayerPrefs.GetInt("GDPR_ADS_CONSENT", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("GDPR_ADS_CONSENT", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool AnalyticsConsent
    {
        get
        {
            return PlayerPrefs.GetInt("GDPR_ANALYTICS_CONSENT", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("GDPR_ANALYTICS_CONSENT", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    //  0 - false, 1 - true
    public static bool ConsentIsSelect
    {
        get
        {
            return PlayerPrefs.GetInt("GDPR_SELECT", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("GDPR_SELECT", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
