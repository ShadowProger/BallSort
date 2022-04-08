using Firebase;
using Firebase.Analytics;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager: SingletonMonoBehaviour<FirebaseManager>
{
    public bool IsReady { get; private set; }

    public void Init()
    {
        Debug.Log($"[Firebase] Init");
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                Success();
            }
            else
            {
                Fail(task.Result);
            }
        });
    }

    void Success()
    {
        Debug.Log($"[Firebase] Initialized");

        //FirebaseAnalytics.SetUserId(User.GetCurrentUserId().ToString());
        FirebaseAnalytics.SetSessionTimeoutDuration(TimeSpan.FromMinutes(10));
        IsReady = true;
    }

    void Fail(DependencyStatus status)
    {
        Debug.LogError($"Could not resolve all Firebase dependencies: {status}");
        Init();
    }

    public void Event(string newEvent)
    {
        if (!IsReady) return;

        Debug.Log($"[Firebase] Event: {newEvent}");
        FirebaseAnalytics.LogEvent(newEvent);
    }

    public void Event(string newEvent, string parameterName, int parametrValue)
    {
        if (!IsReady) return;

        Debug.Log($"[Firebase] Event: {newEvent} [{parameterName}:{parametrValue}]");
        FirebaseAnalytics.LogEvent(newEvent, parameterName, parametrValue);
    }

    public void Event(string newEvent, string parameterName, float parametrValue)
    {
        if (!IsReady) return;

        Debug.Log($"[Firebase] Event: {newEvent} [{parameterName}:{parametrValue}]");
        FirebaseAnalytics.LogEvent(newEvent, parameterName, parametrValue);
    }

    public void Event(string newEvent, string parameterName, string parametrValue)
    {
        if (!IsReady) return;

        Debug.Log($"[Firebase] Event: {newEvent} [{parameterName}:{parametrValue}]");
        FirebaseAnalytics.LogEvent(newEvent, parameterName, parametrValue);
    }

    public void Event(string newEvent, Hashtable parameters = null)
    {
        if (!IsReady) return;

        if (parameters != null)
        {
            List<Parameter> fbParams = new List<Parameter>();
            foreach (DictionaryEntry pair in parameters)
            {
                Debug.Log($"[{pair.Key}:{pair.Value}]");
                if (!(pair.Key is string)) continue; // ключ должен быть string
                if (pair.Value is int) fbParams.Add(new Parameter((string)pair.Key, (int)pair.Value));
                if (pair.Value is double) fbParams.Add(new Parameter((string)pair.Key, (double)pair.Value));
                if (pair.Value is string) fbParams.Add(new Parameter((string)pair.Key, (string)pair.Value));
            }
            Debug.Log($"[Firebase] Event: {newEvent} with params");
            FirebaseAnalytics.LogEvent(newEvent, fbParams.ToArray());
        }
        else
        {
            Debug.Log($"[Firebase] Event: {newEvent}");
            FirebaseAnalytics.LogEvent(newEvent);
        }
    }
}
