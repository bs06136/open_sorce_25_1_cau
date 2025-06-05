 using UnityEngine;
using CardGame;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Button continueButton;
    public Button returnToMenuButton;

    private bool isChariotActive = false;
    private bool isChariotFirstPick = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameOverCanvas.SetActive(false);
        victoryPanel.SetActive(false);

        GameEvents.OnCardStatusRequested = GetCardStatus;
        GameEvents.OnCardChosen = ApplyCardByIndex;
    }

    public void StartGame()
    {
        if (playerHpUI == null)
            playerHpUI = FindObjectOfType<PlayerHP>();
        if (playerCurseUI == null)
            playerCurseUI = FindObjectOfType<PlayerCurse>();
        if (unifiedCardManager == null)
            unifiedCardManager = FindObjectOfType<UnifiedCardManager>();
        if (turnDisplay == null)
            turnDisplay = FindObjectOfType<TurnDisplay>();

        Debug.Log("✅ StartGame 실행됨");
        UnityPlayer = new PlayerBridge(playerHpUI, playerCurseUI);
        UnityGame = new Game(UnityPlayer);

        UnityPlayer.Hp = 10;
        UnityPlayer.Curse = 0;

        Debug.Log("GameManager: 게임 시작");
        UpdateTurnDisplay();
        StartTurn();

        UpdateRerollState();
        ShowRemainDeckNum();
    }

    public void StartTurn()
    {
        if (UnityPlayer.SkipNextTurn)
        {
            Debug.Log("이번 턴은 스킵됩니다.");
            UnityPlayer.SkipNextTurn = false;
            UnityGame.Turn++;
            UpdateTurnDisplay();
            StartTurn();
            return;
        }

        int drawCount = UnityPlayer.NextDrawNum;

        var cards = UnityGame.DrawCards(drawCount);
        Debug.Log($"[덱 상태] 남은 카드 수: {UnityGame.Deck.Count}");

        if (UnityPlayer.Archangel)
        {
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Name == "죽음")
                {
                    var nonDeathCards = CardLibrary.AllCards.Where(c => c.Name != "죽음").ToList();
                    cards[i] = nonDeathCards[UnityEngine.Random.Range(0, nonDeathCards.Count)];
                    Debug.Log($"[대천사 발동] 죽음 → {cards[i].Name} 교체됨");
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
            Debug.Log($"[무작의 선택] {randomIndex}번 카드 선택");
            UnityPlayer.RandomChoice = false;
            ApplyCardByIndex(randomIndex);
            return;
        }

        unifiedCardManager.DisplayCards(cards);
        UpdateTurnDisplay();
    }

    private (List<int> drawnCards, List<int> HP, List<int> curse, List<string> text, int rerollCount) GetCardStatus()
    {
        var cardIndices = currentDrawnCards.Select(c => CardLibrary.AllCards.IndexOf(c)).ToList();
        var hpChanges = currentDrawnCards.Select(c => c.HpChange).ToList();
        var curseChanges = currentDrawnCards.Select(c => c.CurseChange).ToList();
        var descriptions = currentDrawnCards.Select(c => c.Description).ToList();
        return (cardIndices, hpChanges, curseChanges, descriptions, UnityPlayer.RerollAvailable);
    }

    private void ApplyCardByIndex(int index)
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
                Debug.Log("[전차] 첫 번째 카드 선택 → 두 번째 선택 준비");
                isChariotFirstPick = false;
                unifiedCardManager.DisplayCards(currentDrawnCards);
                UpdateTurnDisplay();
                return;
            }
            else
            {
                Debug.Log("[전차] 두 번째 카드 선택 → 전차 종료");
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
        selectedCard.Apply(UnityPlayer, UnityGame, remainingCards);
        UnityPlayer.LastCard = selectedCard;

        UpdateRerollState();

        HandleDelayedEffects();
        HandleCurseDamage();
        HandleDeathCardInjection();
        HandleCurseIncrease();
        HandleEmberEffect();

        if (UnityPlayer.Hp <= 0)
        {
            GameOverHandler.GameOver(UnityGame);
            return;
        }

        if (UnityGame.Turn >= 5 && UnityPlayer.Hp > 0)
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

        continueButton.onClick.RemoveAllListeners();
        returnToMenuButton.onClick.RemoveAllListeners();

        continueButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            victoryPanel.SetActive(false);
            UnityGame.Turn++;
            StartTurn();
        });

        returnToMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SampleScene"); // 确保 MainMenu 场景存在
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

    public int GetCurrentTurn()
    {
        return UnityGame?.Turn ?? 0;
    }

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
            if (UnityPlayer.Archangel)
            {
                for (int i = 0; i < currentDrawnCards.Count; i++)
                {
                    if (currentDrawnCards[i].Name == "죽음")
                    {
                        var nonDeathCards = CardLibrary.AllCards.Where(c => c.Name != "죽음").ToList();
                        currentDrawnCards[i] = nonDeathCards[UnityEngine.Random.Range(0, nonDeathCards.Count)];
                        Debug.Log($"[대천사 발동] 죽음 → {currentDrawnCards[i].Name} 교체됨");
                    }
                }
            }

            unifiedCardManager.DisplayCards(currentDrawnCards);
            UpdateTurnDisplay();
            UpdateRerollState();
        }
        else
        {
            Debug.Log("리롤이 부족합니다.");
        }
    }

    public void UpdateRerollState()
    {
        if (rerollButtonText != null)
        {
            rerollButtonText.text = $"{UnityPlayer.RerollAvailable}";
        }
        else
        {
            Debug.LogWarning("rerollButtonText가 설정되지 않았습니다.");
        }

        rerollButton.interactable = UnityPlayer.RerollAvailable > 0;
    }

    public void ShowRemainDeckNum()
    {
        remainDeckNum.text = UnityGame.Deck.Count.ToString();
        Debug.Log($"남은 덱 수: {remainDeckNum}");
    }

    public void SetEmberGrayscale()
    {
        emberIcon.material = UnityGame.Player.Ember ? null : grayScaleMaterial;
    }
    public void OnContinueButtonClicked()
{
    Debug.Log("继续游戏");
    victoryPanel.SetActive(false);
    ingameCanvas.SetActive(true);
}

public void OnReturnMenuButtonClicked()
{
    Debug.Log("返回主菜单");
    SceneManager.LoadScene("SampleScene"); // 替换为你主菜单的名字
}

}

    
        












