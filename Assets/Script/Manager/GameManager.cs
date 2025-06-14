using UnityEngine;
using CardGame;
using System.Collections.Generic;
using System.Linq;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CardGame.Effects;

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

    public CharacterType selectedCharacter = CharacterType.Explorer;
    private ICharacterEffect characterEffect;
    private int turnCounter = 0;

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
        turnCounter = 0;

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

        int saved = PlayerPrefs.GetInt("SelectedCareer", (int)CharacterType.Explorer);
        selectedCharacter = (CharacterType)saved;

        characterEffect = CharacterEffectFactory.Create(selectedCharacter);
        characterEffect.OnStartGame(this);

        Debug.LogError("StartGame 선택된 직업: " + selectedCharacter);

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

        turnCounter++;
        characterEffect.OnTurnStart(this);

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

        // 🔥 랜덤 선택 처리
        if (UnityPlayer.RandomChoice)
        {
            int randomIndex = UnityEngine.Random.Range(0, currentDrawnCards.Count);
            UnityPlayer.RandomChoice = false;

            lastRandomIndex = randomIndex;
            isRandomPick = true;

            StartCoroutine(HandleRandomSelection(randomIndex));
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

        var selectedCard = currentDrawnCards[index];

        // 🔥 일반 선택도 강조 애니메이션 추가
        if (!isRandomPick) // 랜덤 선택이 아닐 때만 (랜덤은 이미 처리됨)
        {
            StartCoroutine(HandleNormalSelection(index, selectedCard));
            return;
        }

        // 랜덤 선택인 경우 기존 로직 그대로 진행
        ContinueWithCardApplication(index, selectedCard);
    }

    #region 랜덤 선택 애니메이션

    /// <summary>
    /// 랜덤 선택 전체 프로세스 처리
    /// </summary>
    private System.Collections.IEnumerator HandleRandomSelection(int randomIndex)
    {
        Debug.Log($"[GameManager] 랜덤 선택 프로세스 시작: 인덱스 {randomIndex}");

        // 1. 먼저 모든 카드를 정상적으로 표시
        unifiedCardManager.DisplayCards(currentDrawnCards);
        UpdateTurnDisplay();

        // 2. 카드가 모두 뒤집힐 때까지 대기 (애니메이션 완료 대기)
        yield return new WaitForSeconds(0.3f);

        // 3. 랜덤 선택된 카드 강조 애니메이션
        yield return StartCoroutine(HighlightRandomSelectedCard(randomIndex));

        // 4. 카드 적용
        ContinueWithCardApplication(randomIndex, currentDrawnCards[randomIndex]);
    }

    /// <summary>
    /// 랜덤 선택된 카드 강조 애니메이션
    /// </summary>
    private System.Collections.IEnumerator HighlightRandomSelectedCard(int cardIndex)
    {
        int[] slotIndices = GetSlotIndices(currentDrawnCards.Count);
        int slotIndex = slotIndices[cardIndex];

        Debug.Log($"[GameManager] 랜덤 카드 강조: 카드 인덱스 {cardIndex} → 슬롯 인덱스 {slotIndex}");

        // 1. 스토리 표시
        ShowRandomSelectedCardStory(cardIndex);

        // 2. 다른 카드들 페이드 아웃
        yield return StartCoroutine(FadeOutOtherCards(slotIndex));

        // 3. 선택된 카드 강조 애니메이션 (랜덤용 - 노란색, 큰 확대)
        yield return StartCoroutine(HighlightSelectedCard(slotIndex, true));

        // 4. 잠시 대기 (플레이어가 확인할 시간)
        yield return new WaitForSeconds(0.3f);

        // 5. 선택된 카드도 페이드 아웃
        yield return StartCoroutine(FadeOutSelectedCard(slotIndex));
    }

    /// <summary>
    /// 랜덤으로 선택된 카드의 스토리 표시
    /// </summary>
    private void ShowRandomSelectedCardStory(int randomIndex)
    {
        if (currentDrawnCards != null && randomIndex >= 0 && randomIndex < currentDrawnCards.Count)
        {
            var selectedCard = currentDrawnCards[randomIndex];

            Debug.Log($"[GameManager] 랜덤으로 선택된 카드: 인덱스 {randomIndex}, 이름 '{selectedCard.Name}'");

            if (CardStoryDisplay.Instance != null)
            {
                string displayName = $"[무작위] {selectedCard.Name}";
                CardStoryDisplay.Instance.ShowCardStoryWithCustomName(selectedCard.Name, displayName);
                Debug.Log($"[GameManager] 랜덤 선택 카드 '{selectedCard.Name}' 스토리 표시");
            }
            else
            {
                Debug.LogWarning("[GameManager] CardStoryDisplay.Instance가 null입니다.");
            }
        }
        else
        {
            Debug.LogError($"[GameManager] 랜덤 카드 인덱스 오류: {randomIndex}, 전체 카드 수: {currentDrawnCards?.Count ?? 0}");
        }
    }

    #endregion

    #region 일반 선택 애니메이션

    /// <summary>
    /// 일반 선택 프로세스 처리
    /// </summary>
    private System.Collections.IEnumerator HandleNormalSelection(int cardIndex, Card selectedCard)
    {
        Debug.Log($"[GameManager] 일반 선택 프로세스 시작: 인덱스 {cardIndex}, 카드 '{selectedCard.Name}'");

        // 1. 선택된 카드 강조 애니메이션
        yield return StartCoroutine(HighlightNormalSelectedCard(cardIndex));

        // 2. 카드 적용 계속 진행
        ContinueWithCardApplication(cardIndex, selectedCard);
    }

    /// <summary>
    /// 일반 선택된 카드 강조 애니메이션
    /// </summary>
    private System.Collections.IEnumerator HighlightNormalSelectedCard(int cardIndex)
    {
        int[] slotIndices = GetSlotIndices(currentDrawnCards.Count);
        int slotIndex = slotIndices[cardIndex];

        Debug.Log($"[GameManager] 일반 카드 강조: 카드 인덱스 {cardIndex} → 슬롯 인덱스 {slotIndex}");

        // 1. 스토리는 이미 CardButtonHandler에서 표시됨 (중복 방지)

        // 2. 다른 카드들 페이드 아웃
        yield return StartCoroutine(FadeOutOtherCards(slotIndex));

        // 3. 선택된 카드 강조 애니메이션 (일반용 - 청록색, 작은 확대)
        yield return StartCoroutine(HighlightSelectedCard(slotIndex, false));

        // 4. 잠시 대기 (일반 선택은 좀 더 짧게)
        yield return new WaitForSeconds(0.3f);

        // 5. 선택된 카드도 페이드 아웃
        yield return StartCoroutine(FadeOutSelectedCard(slotIndex));
    }

    #endregion

    #region 공용 애니메이션 메서드

    /// <summary>
    /// 다른 카드들을 페이드 아웃
    /// </summary>
    private System.Collections.IEnumerator FadeOutOtherCards(int selectedSlotIndex)
    {
        float fadeTime = 0.4f;

        for (int i = 0; i < unifiedCardManager.cardImageSlots.Length; i++)
        {
            if (i != selectedSlotIndex && unifiedCardManager.cardImageSlots[i].gameObject.activeSelf)
            {
                var image = unifiedCardManager.cardImageSlots[i];
                var canvasGroup = image.GetComponent<CanvasGroup>();

                if (canvasGroup == null)
                    canvasGroup = image.gameObject.AddComponent<CanvasGroup>();

                // 페이드 아웃 애니메이션
                LeanTween.alphaCanvas(canvasGroup, 0.2f, fadeTime).setEaseOutCubic();
            }
        }

        yield return new WaitForSeconds(fadeTime);
    }

    /// <summary>
    /// 선택된 카드 강조 애니메이션 (통합 버전)
    /// </summary>
    private System.Collections.IEnumerator HighlightSelectedCard(int slotIndex, bool isRandom)
    {
        var cardImage = unifiedCardManager.cardImageSlots[slotIndex];
        var cardTransform = cardImage.transform;

        // 원래 크기 저장
        Vector3 originalScale = cardTransform.localScale;

        // 랜덤 vs 일반 선택에 따른 설정
        float highlightTime = isRandom ? 0.8f : 0.6f;
        Vector3 highlightScale = originalScale * (isRandom ? 1.2f : 1.15f);
        Color highlightColor = isRandom ? new Color(1f, 1f, 0.8f, 1f) : new Color(0.8f, 1f, 1f, 1f);

        // 1. 확대 애니메이션
        LeanTween.scale(cardTransform.gameObject, highlightScale, highlightTime * 0.4f)
            .setEaseOutBack();

        // 2. 빛나는 효과
        var originalColor = cardImage.color;
        LeanTween.value(cardImage.gameObject, originalColor, highlightColor, highlightTime * 0.2f)
            .setOnUpdate((Color color) => cardImage.color = color)
            .setLoopPingPong(1);

        yield return new WaitForSeconds(highlightTime * 0.7f);

        // 3. 원래 크기로 복원
        LeanTween.scale(cardTransform.gameObject, originalScale, highlightTime * 0.3f)
            .setEaseInBack();

        // 4. 원래 색상으로 복원
        LeanTween.value(cardImage.gameObject, cardImage.color, originalColor, highlightTime * 0.3f)
            .setOnUpdate((Color color) => cardImage.color = color);

        yield return new WaitForSeconds(highlightTime * 0.3f);
    }

    /// <summary>
    /// 선택된 카드 페이드 아웃
    /// </summary>
    private System.Collections.IEnumerator FadeOutSelectedCard(int slotIndex)
    {
        var cardImage = unifiedCardManager.cardImageSlots[slotIndex];
        var canvasGroup = cardImage.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
            canvasGroup = cardImage.gameObject.AddComponent<CanvasGroup>();

        float fadeTime = 0.3f;
        LeanTween.alphaCanvas(canvasGroup, 0f, fadeTime).setEaseInCubic();

        yield return new WaitForSeconds(fadeTime);

        // 모든 카드 슬롯을 빈 상태로 설정
        unifiedCardManager.SetAllEmptySlots();

        // CanvasGroup 알파값 복원 (다음 턴을 위해)
        unifiedCardManager.RestoreAllCardAlpha();
    }

    /// <summary>
    /// 카드 수에 따른 슬롯 인덱스 배열 반환
    /// </summary>
    private int[] GetSlotIndices(int cardCount)
    {
        switch (cardCount)
        {
            case 1:
                return new int[] { 1 }; // 가운데만
            case 2:
                return new int[] { 0, 2 }; // 양쪽만
            case 3:
            default:
                return new int[] { 0, 1, 2 }; // 모두
        }
    }

    #endregion

    /// <summary>
    /// 카드 적용 로직 계속 진행
    /// </summary>
    private void ContinueWithCardApplication(int index, Card selectedCard)
    {
        currentDrawnCards.RemoveAt(index);
        ApplyCard(selectedCard, currentDrawnCards);

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

        characterEffect.OnAfterCardApply(this, selectedCard);

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

        if (logicGameMode == GameMode.Normal && UnityGame.Turn >= 20)
        {
            ShowVictoryPanel();
            return;
        }
    }

    private void ShowVictoryPanel()
    {
        Debug.Log("🎉 게임 승리!");
        Time.timeScale = 0f;

        if (victoryPanel == null)
        {
            var go = GameObject.Find("GameClear");
            if (go != null)
                victoryPanel = go;
            else
                Debug.LogError("ShowVictoryPanel: 'GameClear' 오브젝트를 찾을 수 없습니다.");
        }

        if (victoryPanel != null)
            victoryPanel.SetActive(true);

        if (returnToMenuButton == null)
        {
            var btnGO = GameObject.Find("return_to_main");
            if (btnGO != null)
                returnToMenuButton = btnGO.GetComponent<Button>();
            else
                Debug.LogError("ShowVictoryPanel: 'return_to_main' 오브젝트를 찾을 수 없습니다.");
        }

        if (returnToMenuButton != null)
        {
            returnToMenuButton.onClick.RemoveAllListeners();
            returnToMenuButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene("SampleScene");
            });
        }
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
        if (isChariotActive && isChariotFirstPick)
            return;

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
            characterEffect.OnReroll(this);
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
