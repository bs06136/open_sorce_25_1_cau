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
    // ✅ 게임 오버 상태 플래그 추가
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

    [Header("Card Page Canvas")]
    public GameObject cardPageCanvas;

    [Header("Victory UI")]
    public GameObject victoryPanel;
    public Button continueButton;
    public Button returnToMenuButton;

    private bool isChariotActive = false;
    private bool isChariotFirstPick = false;

    private bool isRandomPick = false;
    private int lastRandomIndex = -1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);  // 중복 방지
            return;
        }
        Instance = this;

        gameOverCanvas.SetActive(false);
        victoryPanel.SetActive(false);

        GameEvents.OnCardStatusRequested = GetCardStatus;
        GameEvents.OnCardChosen = ApplyCardByIndex;
    }

    public void StartGame()
    {
        if (playerHpUI == null)
        {
            Debug.LogError("❌ PlayerHP가 연결되지 않았습니다! (인스펙터 확인)");
            return;
        }
        if (playerCurseUI == null)
        {
            Debug.LogError("❌ PlayerCurse가 연결되지 않았습니다! (인스펙터 확인)");
            return;
        }
        if (unifiedCardManager == null)
        {
            Debug.LogError("❌ UnifiedCardManager가 연결되지 않았습니다! (인스펙터 확인)");
            return;
        }
        if (turnDisplay == null)
        {
            Debug.LogError("❌ TurnDisplay가 연결되지 않았습니다! (인스펙터 확인)");
            return;
        }

        Debug.Log("✅ StartGame 실행됨");

        UnityPlayer = new PlayerBridge(playerHpUI, playerCurseUI);
        UnityGame = new Game(UnityPlayer);

        UnityPlayer.Hp = 10;
        UnityPlayer.Curse = 0;

        Debug.Log("GameManager: 게임 시작");

        GameObject player = GameObject.Find("Player");
        GameObject floor = GameObject.Find("Floor");

        if (player != null) player.SetActive(true);
        if (floor != null) floor.SetActive(true);

        mainMenuCanvas.SetActive(false);
        ingameCanvas.SetActive(true);

        UpdateTurnDisplay();
        StartTurn();

        UpdateRerollState();
        ShowRemainDeckNum();
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
        if (isGameOver)
        {
            Debug.Log("게임 오버 상태 — StartTurn 중단.");
            return;
        }

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

            // ✅ 랜덤 선택 결과 보관
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

    private (List<int> drawnCards, List<int> HP, List<int> curse, List<string> text, int rerollCount) GetCardStatus()
    {
        var cardIndices = currentDrawnCards.Select(c => CardLibrary.AllCards.IndexOf(c)).ToList();
        var hpChanges = currentDrawnCards.Select(c => c.HpChange).ToList();
        var curseChanges = currentDrawnCards.Select(c => c.CurseChange).ToList();
        var descriptions = currentDrawnCards.Select(c => c.Description).ToList();
        return (cardIndices, hpChanges, curseChanges, descriptions, UnityPlayer.RerollAvailable);
    }

    /* 랜덤 선택의 결과임을 알려주기 위한 변경사항 적용 버전전
    private (List<int> drawnCards, List<int> HP, List<int> curse, List<string> text, int rerollCount, int is_random, int selected_by_random) GetCardStatus()
    {
        var cardIndices = currentDrawnCards.Select(c => CardLibrary.AllCards.IndexOf(c)).ToList();
        var hpChanges = currentDrawnCards.Select(c => c.HpChange).ToList();
        var curseChanges = currentDrawnCards.Select(c => c.CurseChange).ToList();
        var descriptions = currentDrawnCards.Select(c => c.Description).ToList();

        int isRandom = isRandomPick ? 1 : 0;
        int selectedIndex = isRandomPick ? lastRandomIndex : -1;

        return (cardIndices, hpChanges, curseChanges, descriptions, UnityPlayer.RerollAvailable, isRandom, selectedIndex);
    }
    */

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
        if (selectedCard.Name == "죽음")
        {
            Debug.Log("[GameManager] 죽음 카드 선택 — 게임 오버 트리거");
            isGameOver = true;
            GameOverHandler.GameOver(UnityGame);
            return;
        }

        UnityPlayer.HpChangedThisCard = false;
        UnityPlayer.CurseChangedThisCard = false;
        UnityPlayer.DeathCardAddedThisCard = false;

        // 현재값 백업
        int prevHp = UnityPlayer.Hp;
        int prevCurse = UnityPlayer.Curse;
        int prevDeathCard = UnityGame.Deck.Count(c => c.Name == "죽음");

        selectedCard.Apply(UnityPlayer, UnityGame, remainingCards);
        UnityPlayer.LastCard = selectedCard;

        UpdateRerollState();

        HandleDelayedEffects();
        HandleCurseDamage();
        HandleDeathCardInjection();
        HandleCurseIncrease();
        HandleEmberEffect();

        // 보호버프 처리
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

        int afterDeathCard = UnityGame.Deck.Count(c => c.Name == "죽음");
        if (UnityPlayer.NotAddDeath > 0 && afterDeathCard >= prevDeathCard && !UnityPlayer.DeathCardAddedThisCard)
        {
            int diff = afterDeathCard - prevDeathCard;
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



        if (UnityPlayer.Ember)
        {
            Debug.Log("[Ember] 효과 종료");
            UnityPlayer.Ember = false;
            SetEmberGrayscale();
        }

        if (UnityPlayer.Hp <= 0)
        {
            isGameOver = true;
            GameOverHandler.GameOver(UnityGame);
            return;
        }

        if (UnityGame.Turn >= 40 && UnityPlayer.Hp > 0)
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
        remainDeckNum.text = "덱 카드 수: " + UnityGame.Deck.Count.ToString();
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
    public void OpenCardPage()
    {
        mainMenuCanvas.SetActive(false);    // 메인 메뉴 끄기
        cardPageCanvas.SetActive(true);     // 카드 종류 화면 켜기
    }

}