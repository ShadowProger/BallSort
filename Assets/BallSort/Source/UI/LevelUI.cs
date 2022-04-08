using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUI : Window
{
    [SerializeField] GameObject levelButtonPrefab;
    [SerializeField] Button closeButton;
    [SerializeField] Button homeButton;
    [SerializeField] Button prevButton;
    [SerializeField] Button nextButton;
    [SerializeField] Transform grid;

    private const int LEVELS_IN_GRID = 25;
    private int first;
    private int last;
    private bool isHome;
    private int selectedLevel;
    private List<LevelButton> levelButtons = new List<LevelButton>();

    public override void Init()
    {
        closeButton.onClick.AddListener(OnCloseButtonClick);
        homeButton.onClick.AddListener(OnHomeButtonClick);
        prevButton.onClick.AddListener(OnPrevButtonClick);
        nextButton.onClick.AddListener(OnNextButtonClick);
        isHome = true;
    }

    public override void Open()
    {
        base.Open();
        gameObject.SetActive(true);
        ScreenManager.Instance.SetFieldVisible(false);
        UpdateLevelButtons();
    }

    public override void Close()
    {
        base.Close();
        gameObject.SetActive(false);
        ScreenManager.Instance.SetFieldVisible(true);
    }

    private void UpdateLevelButtons()
    {
        ClearLevelButtons();
        if (isHome)
        {
            selectedLevel = GameManager.Instance.LastOpenedLevel;

            first = selectedLevel - LEVELS_IN_GRID / 2;
            last = selectedLevel + LEVELS_IN_GRID / 2;

            if (GameManager.Instance.LevelCount <= LEVELS_IN_GRID)
            {
                first = 0;
                last = GameManager.Instance.LevelCount;
            }
            else if (first < 0)
            {
                first = 0;
                last = first + LEVELS_IN_GRID - 1;
            }
            else if (last >= GameManager.Instance.LevelCount)
            {
                last = GameManager.Instance.LevelCount - 1;
                first = last - LEVELS_IN_GRID + 1;
            }

            CreateLevelButtons();
        }
        else
        {
            CreateLevelButtons();
        }
    }

    private void ClearLevelButtons()
    {
        foreach (var levelButton in levelButtons)
        {
            Destroy(levelButton.gameObject);
        }
        levelButtons.Clear();
    }

    private void CreateLevelButtons()
    {
        for (int i = first; i <= last; i++)
        {
            CreateLevelButton(i);
        }
    }

    private void CreateLevelButton(int levelNumber)
    {
        GameObject buttonObj = Instantiate(levelButtonPrefab, grid);
        LevelButton levelButton = buttonObj.GetComponent<LevelButton>();
        levelButton.Init(GameManager.Instance.GetLevel(levelNumber), levelNumber);
        levelButtons.Add(levelButton);
    }

    public void OnNextButtonClick()
    {
        isHome = false;
        first = last + 1;
        last = first + LEVELS_IN_GRID - 1;
        if (last >= GameManager.Instance.LevelCount)
        {
            last = GameManager.Instance.LevelCount - 1;
        }
        first = last - LEVELS_IN_GRID + 1;
        UpdateLevelButtons();
    }

    public void OnPrevButtonClick()
    {
        isHome = false;
        last = first - 1;
        first = last - LEVELS_IN_GRID + 1;
        if (first < 0)
        {
            first = 0;
        }
        last = first + LEVELS_IN_GRID - 1;
        UpdateLevelButtons();
    }

    public void OnHomeButtonClick()
    {
        isHome = true;
        UpdateLevelButtons();
    }

    public void OnCloseButtonClick()
    {
        Close();
    }
}
