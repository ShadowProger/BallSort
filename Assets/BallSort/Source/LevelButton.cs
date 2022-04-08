using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] Image backSprite;
    [SerializeField] Image lockSprite;
    [SerializeField] Image lightSprite;
    [SerializeField] TMP_Text numberText;
    [SerializeField] Color backColor1;
    [SerializeField] Color backColor2;
    [SerializeField] Color backColor3;
    [SerializeField] Color textColor1;
    [SerializeField] Color textColor2;
    [SerializeField] Color textColor3;

    private Button button;
    private int number;
    private LevelSettings level;
    private Tween tween;
    private Color lockSpriteColor;
    private Color lightSpriteColor;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(SelectLevel);
        lockSpriteColor = lockSprite.color;
        lightSpriteColor = lightSprite.color;
    }

    public void Init(LevelSettings level, int number)
    {
        this.level = level;
        this.number = number;
        numberText.text = $"{number + 1}";
        lockSprite.gameObject.SetActive(false);
        lightSprite.gameObject.SetActive(false);
        Color backColor = backColor1;
        Color textColor = textColor1;
        switch (level.status)
        {
            case LevelSettings.Status.Open:
                backColor = backColor2;
                textColor = textColor2;
                break;
            case LevelSettings.Status.Closed:
                backColor = backColor3;
                textColor = textColor3;
                break;
        }
        backSprite.color = backColor;
        numberText.color = textColor;
        //button.interactable = level.status != LevelSettings.Status.Closed;
    }

    public void SelectLevel()
    {
        if (level.status != LevelSettings.Status.Closed)
        {
            ScreenManager.Instance.levelUI.Close();
            GameManager.Instance.StartLevel(number);
        }
        else
        {
            if (tween != null)
            {
                tween.Kill();
            }

            lockSprite.gameObject.SetActive(level.status == LevelSettings.Status.Closed);
            lightSprite.gameObject.SetActive(level.status == LevelSettings.Status.Closed);

            Color startLockColor = lockSpriteColor;
            Color endLockColor = lockSpriteColor;
            startLockColor.a = 0f;
            lockSprite.color = startLockColor;

            Color startLightColor = lightSpriteColor;
            Color endLightColor = lightSpriteColor;
            startLightColor.a = 0f;
            lightSprite.color = startLightColor;

            Color startTextColor = textColor3;
            Color endTextColor = textColor3;
            endTextColor.a = 0f;

            float duration = 0.5f;
            float pause = 0.5f;

            tween = DOTween.Sequence()
                .Insert(0f, lockSprite.DOColor(endLockColor, duration))
                .Insert(0f, lightSprite.DOColor(endLightColor, duration))
                .Insert(0f, backSprite.DOColor(backColor1, duration))
                .Insert(0f, numberText.DOColor(endTextColor, duration))
                .Insert(duration + pause, lockSprite.DOColor(startLockColor, duration))
                .Insert(duration + pause, lightSprite.DOColor(startLightColor, duration))
                .Insert(duration + pause, backSprite.DOColor(backColor3, duration))
                .Insert(duration + pause, numberText.DOColor(startTextColor, duration));
        }
    }
}
