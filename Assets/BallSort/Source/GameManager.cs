using DG.Tweening;
using Manybits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEngine;
using System.IO;
using UnityEngine.Purchasing;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] List<BallPack> ballPacks;
    [SerializeField] List<Sprite> backSkins;
    [SerializeField] List<ContainerView> containerSkins;
    GameObject containerPrefab;
    [SerializeField] Transform field;
    [SerializeField] Transform containerPlace;

    [SerializeField] GameUI gameUI;
    [SerializeField] WinUI winUI;

    public const int MAX_REVERSE_COUNT = 5;
    public const int MIN_COINS_COUNT = 30;
    public const int MAX_COINS_COUNT = 50;

    [SerializeField] string defaultBallPack;
    [SerializeField] string defaultContainerSkin;
    [HideInInspector] public BallPack currentBallPack;
    [HideInInspector] public Sprite currentBackSkin;

    private int currentBackSkinNumber;

    List<LevelSettings> levels = new List<LevelSettings>();
    private Level currentLevel;
    [HideInInspector] public int currentLevelNumber;
    private int lastOpenedLevel;
    private int lastLoadedLevel;
    private Ball selectedBall;
    private List<Vector2> containerPositions = new List<Vector2>();
    private Ease moveEase = Ease.OutBack;
    private const float OVERSHOOT = 1.1f;
    private const float MOVE_TIME = 0.1f;
    private const float UPDOWN_TIME = 0.25f;
    private Stack<Turn> turns = new Stack<Turn>();

    private const int LEVEL_SHOW_INTERSTITIAL = 3;
    private const float ADVERT_TIME = 40f;
    private float advertTimePassed;

    private float levelTime;

    private PurchaseManager purchaseManager;

    public int ReverseCount { get; set; }
    public int Coins { get; set; }
    public int LevelCount => levels.Count;
    public bool CanAddContainer => !currentLevel.isAdditionalContainer;
    public int LastOpenedLevel => lastOpenedLevel == -1 ? lastLoadedLevel : lastOpenedLevel;
    public int LastLoadedLevel => lastLoadedLevel;
    public bool NoAds => PlayerPrefs.GetInt("NoAds", 0) == 1;

    public void Init()
    {
        purchaseManager = PurchaseManager.Instance;
        purchaseManager.onPurchaseSuccess += OnPurchaseSuccess;
        LoadLevels();
        LoadProgress();
        SetBallPack(defaultBallPack);
        int levelToLoad = lastOpenedLevel != -1 ? lastOpenedLevel : lastLoadedLevel;
        advertTimePassed = ADVERT_TIME;
        StartLevel(levelToLoad);
    }

    private void OnDestroy()
    {
        purchaseManager.onPurchaseSuccess -= OnPurchaseSuccess;
    }

    private void OnPurchaseSuccess(PurchaseEventArgs args)
    {
        if (args.purchasedProduct.definition.id == "no_ads")
        {
            Debug.Log($"OnPurchaseSuccess: {args.purchasedProduct.definition.id}");
            PlayerPrefs.SetInt("NoAds", 1);
            FirebaseManager.Instance.Event("NoAdsPurchaseSuccess");
        }
    }

    private void Update()
    {
        advertTimePassed += Time.deltaTime;
        levelTime += Time.deltaTime;
    }

    private void LoadLevels()
    {
        Debug.Log("[GameManager] LoadLevels");
        XmlDocument xml = new XmlDocument();

        xml.LoadXml(Resources.Load("Levels").ToString());

        XmlNodeList dataList = xml.GetElementsByTagName("level");

        levels.Clear();
        int levelsCount = 0;
        foreach (XmlNode item in dataList)
        {
            LevelSettings levelSettings = new LevelSettings();
            levelSettings.Load(item);
            levelSettings.status = LevelSettings.Status.Closed;
            levels.Add(levelSettings);
            levelsCount++;
        }

        if (levels.Count > 0)
        {
            levels[0].status = LevelSettings.Status.Open;
        }
    }

    private void LoadProgress()
    {
        Debug.Log("[GameManager] LoadProgress");

        string progress = PlayerPrefs.GetString("progress");
        lastOpenedLevel = PlayerPrefs.GetInt("lastOpenedLevel");
        lastLoadedLevel = PlayerPrefs.GetInt("lastLoadedLevel");

        if (string.IsNullOrEmpty(progress))
        {
            return;
        }

        Debug.Log($"{progress}");
        Debug.Log($"lastOpenedLevel: {lastOpenedLevel}");
        Debug.Log($"lastLoadedLevel: {lastLoadedLevel}");

        var levelstatus = Array.ConvertAll(progress.Split(','), int.Parse);
        int count = Math.Min(levelstatus.Length, levels.Count);
        for (int i = 0; i < count; i++)
        {
            levels[i].status = (LevelSettings.Status)levelstatus[i];
        }
    }

    private void SaveProgress()
    {
        Debug.Log("[GameManager] SaveProgress");

        string progress = string.Join(",", levels.Select(l => (int)l.status));

        Debug.Log($"{progress}");

        PlayerPrefs.SetString("progress", progress);
        PlayerPrefs.SetInt("lastOpenedLevel", lastOpenedLevel);
        PlayerPrefs.SetInt("lastLoadedLevel", lastLoadedLevel);
        PlayerPrefs.Save();
    }

    public void CreateField()
    {
        int containerCount = currentLevel.containers.Count;

        int rows = 1;
        if (containerCount > 5)
        {
            rows = 2;
        }

        int cols1 = 0;
        int cols2 = 0;
        int maxCol;
        if (rows == 1)
        {
            cols1 = containerCount;
            maxCol = cols1;
        }
        else
        {
            cols1 = containerCount % 2 == 0 ? containerCount / 2 : containerCount / 2 + 1;
            cols2 = containerCount / 2;
            maxCol = Math.Max(cols1, cols2);
        }

        float containerWidth = maxCol > 4 ? 180f : 240f;

        Vector2[][] positions = new Vector2[2][];
        positions[0] = new Vector2[cols1];
        positions[1] = new Vector2[cols2];

        Debug.Log($"ContainerView.height = {ContainerView.height}");

        float top = (rows - 1) * ContainerView.height / 2;
        for (int i = 0; i < rows; i++)
        {
            float left = (positions[i].Length - 1) * containerWidth / -2;
            for (int j = 0; j < positions[i].Length; j++)
            {
                positions[i][j] = new Vector2(left + j * containerWidth, top - i * ContainerView.height);
            }
        }

        float maxWidth = Mathf.Max(positions[0].Length, positions[1].Length) * containerWidth;
        float scale = 1f;
        if (maxWidth > CameraManager.Instance.ScreenWidth)
        {
            scale = CameraManager.Instance.ScreenWidth / maxWidth;
        }

        containerPositions.Clear();
        for (int i = 0; i < rows; i++)
        {
            containerPositions.AddRange(positions[i]);
        }

        field.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void CreateContainers()
    {
        var containerCount = currentLevel.containers.Count;
        for (int i = 0; i < containerCount; i++)
        {
            var container = currentLevel.containers[i];
            CreateContainerView(container);
        }
    }

    public ContainerView CreateContainerView(Container container)
    {
        GameObject containerGO = Instantiate(containerPrefab, containerPlace);
        ContainerView containerView = containerGO.GetComponent<ContainerView>();
        containerView.Init(container);
        return containerView;
    }

    public void ReplaceContainers()
    {
        var containerCount = currentLevel.containers.Count;
        for (int i = 0; i < containerCount; i++)
        {
            var containerView = currentLevel.containers[i].view;
            containerView.transform.localPosition = containerPositions[i];
        }
    }

    public void CreateBalls()
    {
        var containerCount = currentLevel.containers.Count;
        for (int i = 0; i < containerCount; i++)
        {
            Container container = currentLevel.containers[i];
            ContainerView containerView = container.view;
            var balls = container.balls;

            for (int j = 0; j < balls.Count; j++)
            {
                var ball = balls[j];
                ball.container = container;
                ball.place = j;
                GameObject ballGO = Instantiate(currentBallPack.ballPrefab, containerView.transform);
                ballGO.transform.localPosition = container.view.places[j];
                BallView ballView = ballGO.GetComponent<BallView>();
                ballView.Init(ball);
            }
        }
    }

    public void DestroyContainers()
    {
        if (currentLevel == null)
        {
            return;
        }

        foreach (var container in currentLevel.containers)
        {
            Destroy(container.view.gameObject);
        }
    }

    public void DestroyBalls()
    {
        if (currentLevel == null)
        {
            return;
        }

        foreach (var container in currentLevel.containers)
        {
            foreach (var ball in container.balls)
            {
                Destroy(ball.view.gameObject);
            }
        }
    }

    public void Restart()
    {
        DestroyBalls();
        DestroyContainers();
        currentLevel.Load(currentLevel.settings);
        SetContainerSkin();
        CreateContainers();
        ReplaceContainers();
        CreateBalls();
        ReverseCount = MAX_REVERSE_COUNT;
        turns.Clear();
        selectedBall = null;
        levelTime = 0f;
        Events.Restart();
        FirebaseManager.Instance.Event("LevelRestart", "levelNumber", currentLevelNumber + 1);
    }

    public void StartLevel(int levelNumber)
    {
        currentLevelNumber = levelNumber;
        lastLoadedLevel = levelNumber;
        DestroyBalls();
        DestroyContainers();
        if (currentLevel != null)
        {
            currentLevel.Clear();
        }
        currentLevel = new Level();
        currentLevel.Load(levels[levelNumber]);
        SetContainerSkin();
        CreateField();
        CreateContainers();
        ReplaceContainers();
        CreateBalls();
        ReverseCount = MAX_REVERSE_COUNT;
        turns.Clear();
        selectedBall = null;
        levelTime = 0f;
        Events.Start(levelNumber);
        SaveProgress();
        FirebaseManager.Instance.Event("LevelStart", "levelNumber", currentLevelNumber + 1);
    }

    public void AddContainer()
    {
        if (currentLevel.isAdditionalContainer)
        {
            return;
        }

        currentLevel.isAdditionalContainer = true;
        var container = currentLevel.AddContainer();
        CreateContainerView(container);
        CreateField();
        ReplaceContainers();
    }

    public int GetRandomColor(int maxColorCount)
    {
        return UnityEngine.Random.Range(0, maxColorCount);
    }

    public void SetBallPack(string name)
    {
        bool contains = false;
        foreach (var pack in ballPacks)
        {
            if (pack.packName == name)
            {
                currentBallPack = pack;
                contains = true;
                break;
            }
        }

        if (!contains)
        {
            Debug.LogError($"There is no ballPack [{name}]");
        }
    }

    public Sprite GetBallSprite(int value)
    {
        return currentBallPack.ballSprites[value];
    }

    public void SetContainerSkin()
    {
        string containerSkinName = PlayerPrefs.GetString("ContainerSkin", defaultContainerSkin);

        Debug.Log($"containerSkinName = {containerSkinName}");

        foreach (var containerSkin in containerSkins)
        {
            if (containerSkin.skinName == containerSkinName && containerSkin.size == currentLevel.settings.containerSize)
            {
                containerPrefab = containerSkin.gameObject;
                float spriteHeight = containerSkin.Sprite.rect.height;
                ContainerView.height = spriteHeight + ContainerView.BUFFER * 2;
            }
        }
    }

    public void SelectContainer(Container container)
    {
        if (!container.CanSelect)
        {
            return;
        }

        if (selectedBall == null)
        {
            // Выбор шарика
            selectedBall = container.GetTopBall();
            if (selectedBall != null)
            {
                var containerView = selectedBall.container.view;
                var swing = containerView.top;
                swing.y -= 25;

                var sequence = DOTween.Sequence();
                sequence.Append(selectedBall.view.Tween = selectedBall.view.transform.DOLocalMove(containerView.top, UPDOWN_TIME).SetEase(moveEase, 0f));
                sequence.Append(selectedBall.view.transform.DOLocalMove(swing, 1f).SetLoops(100, LoopType.Yoyo).SetEase(Ease.InOutQuad));

                selectedBall.view.Tween = sequence;
            }
        }
        else
        {
            var containerView = selectedBall.container.view;
            if (selectedBall.container == container)
            {
                // Отмена выбора, возвращаем шарик на место
                selectedBall.view.Tween = selectedBall.view.transform.DOLocalMove(containerView.places[selectedBall.place], UPDOWN_TIME).SetEase(moveEase, OVERSHOOT);
                selectedBall = null;
            }
            else
            {
                if (container.CanPlaceOnTop(selectedBall))
                {
                    // Перемещаем шарик на выбранную позицию
                    var turn = new Turn(selectedBall, selectedBall.container, container);
                    turns.Push(turn);
                    
                    selectedBall.view.Tween = null;

                    var oldContainer = selectedBall.container;
                    oldContainer.RemoveBall(selectedBall);
                    container.AddBall(selectedBall);
                    selectedBall.view.SetSortingLayer("UpperBall");
                    var movingBall = selectedBall;

                    selectedBall.view.Tween = DOTween.Sequence()
                        .Insert(0f, selectedBall.view.transform.DOLocalMove(container.view.top, MOVE_TIME)
                            .OnComplete(() => 
                            { 
                                movingBall.view.SetSortingLayer("Ball");
                                CheckCompleete(movingBall.container);
                            })
                            .OnKill(() => { movingBall.view.SetSortingLayer("Ball"); }))
                        .Insert(MOVE_TIME + 0.07f, selectedBall.view.transform.DOLocalMove(container.view.places[selectedBall.place], UPDOWN_TIME).SetEase(moveEase, OVERSHOOT));

                    selectedBall = null;

                    // Win
                    var isWin = currentLevel.CheckWin();
                    if (isWin)
                    {
                        Win();
                    }
                }
                else
                {
                    // Выбираем другой шарик, первый возвращаем на место
                    selectedBall.view.Tween = selectedBall.view.transform.DOLocalMove(containerView.places[selectedBall.place], UPDOWN_TIME).SetEase(moveEase, OVERSHOOT);

                    selectedBall = container.GetTopBall();

                    var swing = containerView.top;
                    swing.y -= 25;

                    var sequence = DOTween.Sequence();
                    sequence.Append(selectedBall.view.Tween = selectedBall.view.transform.DOLocalMove(containerView.top, UPDOWN_TIME).SetEase(moveEase, 0f));
                    sequence.Append(selectedBall.view.transform.DOLocalMove(swing, 1f).SetLoops(100, LoopType.Yoyo).SetEase(Ease.InOutQuad));

                    selectedBall.view.Tween = sequence;
                }
            }
        }
    }

    private void CheckCompleete(Container container)
    {
        if (container.IsComplete)
        {
            container.view.DoGlow();
        }
    }

    public bool CanShowInterstitial()
    {
        Debug.Log($"[CanShowInterstitial] level: {currentLevelNumber >= 10}, " +
            $"isLoaded: {AdMob.Instance.IsInterstitialLoaded}, " +
            $"time: {advertTimePassed >= ADVERT_TIME}, noAds: {NoAds}, bought: {purchaseManager.CheckBuyState("no_ads")}");

        return currentLevelNumber >= (LEVEL_SHOW_INTERSTITIAL - 1)
            && AdMob.Instance.IsInterstitialLoaded 
            && advertTimePassed >= ADVERT_TIME
            && !(NoAds || purchaseManager.CheckBuyState("no_ads"))
            ;
    }

    public void ShowInterstitial()
    {
        if (CanShowInterstitial())
        {
            Debug.Log($"Show Interstitial");
            advertTimePassed = 0f;
            FirebaseManager.Instance.Event("ShowInterstitial");
            AdMob.Instance.ShowInterstitial();
        }
    }

    [ContextMenu("Win")]
    public void Win()
    {
        if (currentLevelNumber < 10)
        {
            FirebaseManager.Instance.Event($"LevelComplete_{currentLevelNumber + 1}");
        }
        else
        {
            FirebaseManager.Instance.Event("LevelComplete", new Hashtable()
            {
                {"levelNumber", currentLevelNumber + 1},
                {"time", (int)levelTime}
            });
            if ((currentLevelNumber + 1) % 50 == 0)
            {
                FirebaseManager.Instance.Event($"LevelComplete_{currentLevelNumber + 1}");
            }
        }

        ShowInterstitial();

        int nextLevel = currentLevelNumber + 1;
        Debug.Log($"nextLevel = {nextLevel}");
        if (nextLevel < levels.Count)
        {
            Debug.Log($"current: {currentLevel.settings.status}, next: {levels[nextLevel].status}");
            if (levels[nextLevel].status == LevelSettings.Status.Closed)
            {
                levels[nextLevel].status = LevelSettings.Status.Open;
                lastOpenedLevel = nextLevel;
                Debug.Log($"lastOpenedLevel = {lastOpenedLevel}");
            }
        }
        else
        {
            lastOpenedLevel = -1;
        }
        currentLevel.settings.status = LevelSettings.Status.Complete;
        int reward = UnityEngine.Random.Range(MIN_COINS_COUNT, MAX_COINS_COUNT + 1);
        Coins += reward;
        SaveProgress();
        ScreenManager.Instance.winUI.Open();
    }

    [ContextMenu("Delete Prefs")]
    public void DeletePrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    public void ReverseTurn()
    {
        if (ReverseCount <= 0)
        {
            return;
        }

        if (turns.Count == 0)
        {
            return;
        }

        if (selectedBall != null)
        {
            var containerView = selectedBall.container.view;
            selectedBall.view.Tween = selectedBall.view.transform.DOLocalMove(containerView.places[selectedBall.place], UPDOWN_TIME).SetEase(moveEase, OVERSHOOT);
            selectedBall = null;
        }

        var lastTurn = turns.Pop();

        var from = lastTurn.to;
        var to = lastTurn.from;
        var ball = lastTurn.ball;

        ball.view.Tween = null;

        float pause = 0.04f;
        float timeStamp1 = UPDOWN_TIME + pause;
        float timeStamp2 = timeStamp1 + MOVE_TIME + pause;

        var newPlace = to.GetTopPlace();
        var top1 = from.view.GlobalTop;
        var top2 = to.view.top;

        from.RemoveBall(ball);
        to.AddBall(ball);

        var sequence = DOTween.Sequence();
        sequence
            .Insert(0f, ball.view.transform.DOMove(top1, UPDOWN_TIME).SetEase(moveEase, OVERSHOOT)
                .OnComplete(() => { ball.view.SetSortingLayer("UpperBall"); })
                .OnKill(() => { ball.view.SetSortingLayer("UpperBall"); }))
            .Insert(timeStamp1, ball.view.transform.DOLocalMove(top2, MOVE_TIME)
                .OnComplete(() => { ball.view.SetSortingLayer("Ball"); })
                .OnKill(() => { ball.view.SetSortingLayer("Ball"); }))
            .Insert(timeStamp2, ball.view.transform.DOLocalMove(newPlace, UPDOWN_TIME).SetEase(moveEase, OVERSHOOT));

        ball.view.Tween = sequence;

        ReverseCount--;
    }

    public LevelSettings GetLevel(int levelNumber)
    {
        return levels[levelNumber];
    }

    public void SetFieldVisible(bool isVisible)
    {
        field.gameObject.SetActive(isVisible);
    }

    public void OpenAllLevels()
    {
        foreach (var level in levels)
        {
            level.status = LevelSettings.Status.Complete;
        }
        lastOpenedLevel = -1;
        SaveProgress();
    }
}

public class Turn
{
    public Ball ball;
    public Container from;
    public Container to;

    public Turn(Ball ball, Container from, Container to)
    {
        this.ball = ball;
        this.from = from;
        this.to = to;
    }
}
