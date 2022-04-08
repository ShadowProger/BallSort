using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : Window
{
    [SerializeField] GameObject content;
    [SerializeField] Button nextButton;
    [SerializeField] TMP_Text winText;

    private Coroutine showRoutine;

    string[] winTextKeys = { "ui.winText1", "ui.winText2", "ui.winText3" };

    public override void Init()
    {
        nextButton.onClick.AddListener(OnNextButtonClick);
    }

    public override void Open()
    {
        base.Open();
        winText.text = Localization.Instance.Localize(winTextKeys[Random.Range(0, winTextKeys.Length)]);
        gameObject.SetActive(true);
        content.SetActive(false);
        if (showRoutine != null)
        {
            StopCoroutine(showRoutine);
        }
        StartCoroutine(ShowContent());
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
        content.SetActive(false);
    }

    public void OnNextButtonClick()
    {
        Close();
        int nextLevel = GameManager.Instance.currentLevelNumber + 1;
        if (nextLevel >= GameManager.Instance.LevelCount)
        {
            nextLevel = 0;
        }
        GameManager.Instance.StartLevel(nextLevel);
    }

    private IEnumerator ShowContent()
    {
        yield return new WaitForSeconds(1.5f);
        content.SetActive(true);
    }
}
