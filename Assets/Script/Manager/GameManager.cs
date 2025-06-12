using UnityEngine;
using CardGame;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameMode
{
    Normal,
    Infinite
}

public class GameManager : MonoBehaviour
{
    private bool isGameOver = false;
    public static GameManager Instance { get; private set; }

    [Header("UI 컴포넌트")]
    public PlayerHP playerHpUI;
    public PlayerCurse playerCurseUI;
    public UnifiedCardManager unifiedCardManager;
    public TurnDisplay turnDisplay;

    public Game UnityGame { get; private set; }
    public PlayerBridge UnityPlayer { get; private set; }

    private List<Card> currentDrawnCards = new();

    [Header("Reroll Button")]
    public Button rerollButton;
    public TextMeshProUGUI rerollButtonText;

    [Header("Player Status")]
    public TextMeshProUGUI remainDeckNum;
    public Image emberIcon;
    public Material grayScaleMaterial;

    [Header("Canvas handling")]
    public GameObject gameOverCanvas;
    public GameObject ingameCanvas;
    public GameObject mainMenuCanvas;
    public GameObject cardPageCanvas;
    public GameObject creditCanvas;
    public GameObject settingCanvas;
    public GameObject gameClearCanvas;
    public GameObject ingameSettingCanvas;
    public GameObject storyCanvas;
    public GameObject CharacterCanvas;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Button continueButton;
    public Button returnToMenuButton;

    [Header("GameMode")]
    public TextMeshProUGUI gameModeText;
    
    [Header("Mode Notice UI")]
    public GameObject infiniteModeNotice;

    private bool isChariotActive = false;
    private bool isChariotFirstPick = false;

    private bool isRandomPick = false;
    private int lastRandomIndex = -1;

    private string[] gameModes = { "일반모드", "무한모드" };
    private int currentGameModeIndex = 0;

    private GameMode logicGameMode = GameMode.Normal;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        gameOverCanvas.SetActive(false);
        victoryPanel.SetActive(false);
        ingameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(true);
        cardPageCanvas.SetActive(false);
        creditCanvas.SetActive(false);
        settingCanvas.SetActive(false);
        gameClearCanvas.SetActive(false);
        ingameSettingCanvas.SetActive(false);
        storyCanvas.SetActive(false);
        CharacterCanvas.SetActive(false);

        UpdateGameModeUI();

        GameEvents.OnCardStatusRequested = GetCardStatus;
        GameEvents.OnCardChosen = ApplyCardByIndex;
    }

    public void StartGame()
    {
        GameObject player = GameObject.Find("Player");
        GameObject floor = GameObject.Find("Floor");

        if (player != null) player.SetActive(false);
        if (floor != null) floor.SetActive(false);

        if (playerHpUI == null || playerCurseUI == null || unifiedCardManager == null || turnDisplay == null)
        {
            Debug.LogError("❌ 필수 UI 컴포넌트 연결 누락");
            return;
        }

        Debug.Log("✅ StartGame 실행됨");

        UnityPlayer = new PlayerBridge(playerHpUI, playerCurseUI);
        UnityGame = new Game(UnityPlayer);

        UnityPlayer.Hp = 10;
        UnityPlayer.Curse = 0;

        if (player != null) player.SetActive(true);
        if (floor != null) floor.SetActive(true);

        mainMenuCanvas.SetActive(false);
        ingameCanvas.SetActive(true);

        UpdateTurnDisplay();
        StartTurn();

        UpdateRerollState();
        ShowRemainDeckNum();

        if (logicGameMode == GameMode.Infinite)
{
    if (infiniteModeNotice != null)
        infiniteModeNotice.SetActive(true);
}
else
{
    if (infiniteModeNotice != null)
        infiniteModeNotice.SetActive(false);
}

    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene" && Instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void StartTurn()
    {
        if (isGameOver) return;

        if (UnityPlayer.SkipNextTurn)
        {
            UnityPlayer.SkipNextTurn = false;
            UnityGame.Turn++;
            UpdateTurnDisplay();
            StartTurn();
            return;
        }

        int drawCount = UnityPlayer.NextDrawNum;
        var cards = UnityGame.DrawCards(drawCount);

        if (UnityPlayer.Archangel)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Name == "죽음")
                {
                    var nonDeathCards = CardLibrary.AllCards.Where(c => c.Name != "죽음").ToList();
                    cards[i] = nonDeathCards[UnityEngine.Random.Range(0, nonDeathCards.Count)];
                }
            }
        }

        if (UnityPlayer.Chariot)
        {
            isChariotActive = true;
            isChariotFirstPick = true;
            UnityPlayer.Chariot = false;
        }

        currentDrawnCards = cards;

        if (UnityPlayer.RandomChoice)
        {
            int randomIndex = UnityEngine.Random.Range(0, currentDrawnCards.Count);
            UnityPlayer.RandomChoice = false;

            lastRandomIndex = randomIndex;
            isRandomPick = true;

            ApplyCardByIndex(randomIndex);
            return;
        }

        isRandomPick = false;
        lastRandomIndex = -1;

        unifiedCardManager.DisplayCards(cards);
        UpdateTurnDisplay();
    }

    private (List<int>, List<int>, List<int>, List<string>, int) GetCardStatus()
    {
        var indices = currentDrawnCards.Select(c => CardLibrary.AllCards.IndexOf(c)).ToList();
        var hp = currentDrawnCards.Select(c => c.HpChange).ToList();
        var curse = currentDrawnCards.Select(c => c.CurseChange).ToList();
        var text = currentDrawnCards.Select(c => c.Description).ToList();
        return (indices, hp, curse, text, UnityPlayer.RerollAvailable);
    }

    public void ApplyCardByIndex(int index)
    {
        UnityPlayer.NextDrawNum = 3;
        UnityPlayer.Archangel = false;

        if (index < 0 || index >= currentDrawnCards.Count) return;

        var selected = currentDrawnCards[index];
        currentDrawnCards.RemoveAt(index);
        ApplyCard(selected, currentDrawnCards);

        if (isChariotActive)
        {
            if (isChariotFirstPick)
            {
                isChariotFirstPick = false;
                unifiedCardManager.DisplayCards(currentDrawnCards);
                UpdateTurnDisplay();
                return;
            }
            else
            {
                isChariotActive = false;
                UnityGame.Turn++;
                UpdateTurnDisplay();
                StartTurn();
                return;
            }
        }

        UnityGame.Turn++;
        UpdateTurnDisplay();
        StartTurn();
    }

    private void ApplyCard(Card selectedCard, List<Card> remainingCards)
    {
        if (selectedCard.Name == "죽음")
        {
            isGameOver = true;
            GameOverHandler.GameOver(UnityGame);
            return;
        }

        int prevHp = UnityPlayer.Hp;
        int prevCurse = UnityPlayer.Curse;
        int prevDeath = UnityGame.Deck.Count(c => c.Name == "죽음");

        selectedCard.Apply(UnityPlayer, UnityGame, remainingCards);
        UnityPlayer.LastCard = selectedCard;

        UpdateRerollState();

        HandleDelayedEffects();
        HandleCurseDamage();
        HandleDeathCardInjection();
        HandleCurseIncrease();

        if (UnityPlayer.NonHpIncreaseTurn > 0 && UnityPlayer.Hp > prevHp && !UnityPlayer.HpChangedThisCard)
            UnityPlayer.Hp = prevHp;
        if (UnityPlayer.NonHpIncreaseTurn > 0 && !UnityPlayer.HpChangedThisCard)
            UnityPlayer.NonHpIncreaseTurn--;

        if (UnityPlayer.NonHpDecreaseTurn > 0 && UnityPlayer.Hp < prevHp && !UnityPlayer.HpChangedThisCard)
            UnityPlayer.Hp = prevHp;
        if (UnityPlayer.NonHpDecreaseTurn > 0 && !UnityPlayer.HpChangedThisCard)
            UnityPlayer.NonHpDecreaseTurn--;

        if (UnityPlayer.NonCurseIncreaseTurn > 0 && UnityPlayer.Curse > prevCurse && !UnityPlayer.CurseChangedThisCard)
            UnityPlayer.Curse = prevCurse;
        if (UnityPlayer.NonCurseIncreaseTurn > 0 && !UnityPlayer.CurseChangedThisCard)
            UnityPlayer.NonCurseIncreaseTurn--;

        if (UnityPlayer.NonCurseDecreaseTurn > 0 && UnityPlayer.Curse < prevCurse && !UnityPlayer.CurseChangedThisCard)
            UnityPlayer.Curse = prevCurse;
        if (UnityPlayer.NonCurseDecreaseTurn > 0 && !UnityPlayer.CurseChangedThisCard)
            UnityPlayer.NonCurseDecreaseTurn--;

        int afterDeath = UnityGame.Deck.Count(c => c.Name == "죽음");
        if (UnityPlayer.NotAddDeath > 0 && afterDeath >= prevDeath && !UnityPlayer.DeathCardAddedThisCard)
        {
            int diff = afterDeath - prevDeath;
            int count = 0;
            for (int i = UnityGame.Deck.Count - 1; i >= 0 && count < diff; i--)
            {
                if (UnityGame.Deck[i].Name == "죽음")
                {
                    UnityGame.Deck.RemoveAt(i);
                    count++;
                }
            }
        }

        HandleEmberEffect();

        if (UnityPlayer.Hp <= 0)
        {
            isGameOver = true;
            GameOverHandler.GameOver(UnityGame);
            return;
        }

        // ✅ 胜利条件只在 일반모드 时启用
        if (logicGameMode == GameMode.Normal && UnityGame.Turn >= 5)
        {
            ShowVictoryPanel();
            return;
        }
    }

    private void ShowVictoryPanel()
    {
        Debug.Log("🎉 게임 승리!");
        Time.timeScale = 0f;
        victoryPanel.SetActive(true);

        returnToMenuButton.onClick.RemoveAllListeners();
        returnToMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SampleScene");
        });
    }

    private void HandleDelayedEffects()
    {
        var newList = new List<(int, Action)>();
        foreach (var (delay, effect) in UnityPlayer.DelayedEffects)
        {
            if (delay > 0)
                newList.Add((delay - 1, effect));
            else
                effect();
        }
        UnityPlayer.DelayedEffects = newList;
    }

    private void HandleCurseDamage()
    {
        if (UnityPlayer.NonCurseDamageTurn > 0)
        {
            UnityPlayer.NonCurseDamageTurn--;
        }
        else if (UnityPlayer.Curse > 0)
        {
            UnityPlayer.Hp -= UnityPlayer.Curse;
            UnityPlayer.Hp = UnityPlayer.Hp;
        }
    }

    private void HandleDeathCardInjection()
    {
        if (UnityPlayer.NotAddDeath > 0)
        {
            UnityPlayer.NotAddDeath--;
        }
        else if (UnityPlayer.Curse >= 6)
        {
            int deathAdd = UnityPlayer.Curse - 5;
            UnityGame.InsertDeathCards(deathAdd);
        }
    }

    private void HandleCurseIncrease()
    {
        if (UnityGame.Turn % 5 == 0)
        {
            int inc = 1 + (UnityGame.Turn / 5 - 1);
            UnityPlayer.Curse += inc;
            UnityPlayer.Curse = UnityPlayer.Curse;
        }
    }

    private void HandleEmberEffect()
    {
        if (UnityPlayer.Ember && UnityPlayer.Hp <= 1)
        {
            UnityPlayer.Hp = 1;
            UnityPlayer.Curse = 0;
            UnityPlayer.Ember = false;
        }
        SetEmberGrayscale();
    }

    private void UpdateTurnDisplay()
    {
        if (turnDisplay != null)
        {
            turnDisplay.ForceUpdateTurn();
        }
    }

    public int GetCurrentTurn() => UnityGame?.Turn ?? 0;

    public void ShowGameOver()
    {
        ingameCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        gameOverCanvas.SetActive(true);
    }

    public void rerollButtonClicked()
    {
        if (UnityPlayer.RerollAvailable > 0)
        {
            UnityPlayer.RerollAvailable--;
            currentDrawnCards = UnityGame.DrawCards(UnityPlayer.NextDrawNum);
            unifiedCardManager.DisplayCards(currentDrawnCards);
            UpdateTurnDisplay();
            UpdateRerollState();
        }
    }

    public void UpdateRerollState()
    {
        if (rerollButtonText != null)
        {
            rerollButtonText.text = $"{UnityPlayer.RerollAvailable}";
        }

        rerollButton.interactable = UnityPlayer.RerollAvailable > 0;
    }

    public void ShowRemainDeckNum()
    {
        remainDeckNum.text = "덱 카드 수: " + UnityGame.Deck.Count.ToString();
    }

    public void SetEmberGrayscale()
    {
        emberIcon.material = UnityGame.Player.Ember ? null : grayScaleMaterial;
    }

    public void OpenCardPage() => cardPageCanvas.SetActive(true);
    public void CloseCardPage() => cardPageCanvas.SetActive(false);

    public void OnGameModeClick()
    {
        currentGameModeIndex = (currentGameModeIndex + 1) % gameModes.Length;
        UpdateGameModeUI();

        // ✅ 同步逻辑模式
        logicGameMode = (currentGameModeIndex == 0) ? GameMode.Normal : GameMode.Infinite;
        Debug.Log($"게임모드 변경됨: {logicGameMode}");
    }

    private void UpdateGameModeUI()
    {
        if (gameModeText != null)
            gameModeText.text = gameModes[currentGameModeIndex];
    }

    public bool IsInfiniteMode() => logicGameMode == GameMode.Infinite;
}
