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

    /// <summary>
    /// ApplyCardByIndex 메서드 수정 - 디버그 로그 추가
    /// </summary>
    public void ApplyCardByIndex(int index)
    {
        Debug.Log($"[GameManager] ApplyCardByIndex 호출: index={index}");
        Debug.Log($"[GameManager] 현재 상태: currentDrawnCards.Count={currentDrawnCards?.Count ?? 0}");
        Debug.Log($"[GameManager] 전차 상태: isChariotActive={isChariotActive}, isChariotFirstPick={isChariotFirstPick}");

        UnityPlayer.NextDrawNum = 3;
        UnityPlayer.Archangel = false;

        if (index < 0 || index >= currentDrawnCards.Count)
        {
            Debug.LogError($"[GameManager] 잘못된 인덱스: {index}, 카드 수: {currentDrawnCards.Count}");
            return;
        }

        var selectedCard = currentDrawnCards[index];
        Debug.Log($"[GameManager] 선택된 카드: '{selectedCard.Name}'");

        // 🔥 일반 선택도 강조 애니메이션 추가
        if (!isRandomPick) // 랜덤 선택이 아닐 때만 (랜덤은 이미 처리됨)
        {
            Debug.Log("[GameManager] 일반 선택 애니메이션 시작");
            StartCoroutine(HandleNormalSelection(index, selectedCard));
            return;
        }

        // 랜덤 선택인 경우 기존 로직 그대로 진행
        Debug.Log("[GameManager] 랜덤 선택 - 직접 카드 적용");
        ContinueWithCardApplication(index, selectedCard);
    }

    #region 랜덤 선택 애니메이션

    /// <summary>
    /// 랜덤 선택 전체 프로세스 처리 (타이밍 수정)
    /// </summary>
    private System.Collections.IEnumerator HandleRandomSelection(int randomIndex)
    {
        Debug.Log($"[GameManager] 랜덤 선택 프로세스 시작: 인덱스 {randomIndex}");

        // 1. 먼저 모든 카드를 정상적으로 표시
        unifiedCardManager.DisplayCards(currentDrawnCards);
        UpdateTurnDisplay();

        // 2. 카드 뒤집기 애니메이션이 완전히 완료될 때까지 충분히 대기
        float cardAnimationTime = 0.5f + 0.2f * currentDrawnCards.Count + 0.5f + 0.3f; // 슬라이드업 + 딜레이 + 플립딜레이 + 플립시간
        Debug.Log($"[GameManager] 카드 애니메이션 완료 대기: {cardAnimationTime}초");
        yield return new WaitForSeconds(cardAnimationTime);

        // 3. 모든 카드가 앞면으로 뒤집혔는지 확인
        yield return StartCoroutine(WaitForCardsToFlip());

        // 4. 랜덤 선택된 카드 강조 애니메이션
        Debug.Log($"[GameManager] 랜덤 카드 강조 시작: 인덱스 {randomIndex}");
        yield return StartCoroutine(HighlightRandomSelectedCard(randomIndex));

        // 5. 카드 적용
        ContinueWithCardApplication(randomIndex, currentDrawnCards[randomIndex]);
    }

    /// <summary>
    /// 모든 카드가 앞면으로 뒤집힐 때까지 대기
    /// </summary>
    private System.Collections.IEnumerator WaitForCardsToFlip()
    {
        int maxWaitTime = 50; // 5초 최대 대기
        int waitCount = 0;

        while (waitCount < maxWaitTime)
        {
            bool allCardsFlipped = true;

            // 현재 배치된 카드 슬롯들이 모두 앞면인지 확인
            int[] slotIndices = GetSlotIndices(currentDrawnCards.Count);

            foreach (int slotIndex in slotIndices)
            {
                if (!unifiedCardManager.isCardFront[slotIndex])
                {
                    allCardsFlipped = false;
                    break;
                }
            }

            if (allCardsFlipped)
            {
                Debug.Log($"[GameManager] 모든 카드 뒤집기 완료! 대기 시간: {waitCount * 0.1f}초");
                break;
            }

            yield return new WaitForSeconds(0.1f);
            waitCount++;
        }

        if (waitCount >= maxWaitTime)
        {
            Debug.LogWarning("[GameManager] 카드 뒤집기 대기 시간 초과");
        }

        // 추가 안전 대기 시간 (0.2초)
        yield return new WaitForSeconds(0.2f);
    }

    /// <summary>
    /// 랜덤 선택된 카드 강조 애니메이션 (타이밍 조정)
    /// </summary>
    private System.Collections.IEnumerator HighlightRandomSelectedCard(int cardIndex)
    {
        int[] slotIndices = GetSlotIndices(currentDrawnCards.Count);
        int slotIndex = slotIndices[cardIndex];

        Debug.Log($"[GameManager] 랜덤 카드 강조: 카드 인덱스 {cardIndex} → 슬롯 인덱스 {slotIndex}");

        // 1. 스토리 표시
        ShowRandomSelectedCardStory(cardIndex);

        // 2. 약간의 대기 (스토리 표시 시간)
        yield return new WaitForSeconds(0.3f);

        // 3. 다른 카드들 페이드 아웃
        yield return StartCoroutine(FadeOutOtherCards(slotIndex));

        // 4. 선택된 카드 강조 애니메이션 (랜덤용 - 노란색, 큰 확대)
        yield return StartCoroutine(HighlightSelectedCard(slotIndex, true));

        // 5. 잠시 대기 (플레이어가 확인할 시간) - 랜덤 선택이므로 좀 더 길게
        yield return new WaitForSeconds(1.2f);

        // 6. 선택된 카드도 페이드 아웃
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
    /// 현재 활성화된 전차 슬롯들 가져오기 (정렬된 순서로)
    /// </summary>
    private int[] GetActiveChariotSlots()
    {
        List<int> activeSlots = new List<int>();

        if (unifiedCardManager != null)
        {
            // 실제로 카드가 앞면으로 표시된 슬롯들 확인
            for (int i = 0; i < unifiedCardManager.isCardFront.Length; i++)
            {
                if (unifiedCardManager.isCardFront[i])
                {
                    activeSlots.Add(i);
                }
            }
        }

        // 🔥 슬롯 번호 순으로 정렬 (0, 1, 2 순서)
        activeSlots.Sort();

        var result = activeSlots.ToArray();
        Debug.Log($"[GameManager] 감지된 활성 슬롯 (정렬됨): [{string.Join(", ", result)}]");
        return result;
    }

    /// <summary>
    /// 일반 선택된 카드 강조 애니메이션 (전차 부분 완전 수정)
    /// </summary>
    private System.Collections.IEnumerator HighlightNormalSelectedCard(int cardIndex)
    {
        int slotIndex;

        // 🔥 전차 두 번째 선택인지 확인
        if (isChariotActive && !isChariotFirstPick)
        {
            // 🔥 전차 두 번째 선택: 활성화된 슬롯에서 해당 카드 인덱스의 슬롯 찾기
            Debug.Log($"[GameManager] 전차 두 번째 선택 - 카드 인덱스 {cardIndex}");

            var activeSlots = GetActiveChariotSlots();
            if (cardIndex >= 0 && cardIndex < activeSlots.Length)
            {
                slotIndex = activeSlots[cardIndex];
                Debug.Log($"[GameManager] 전차 두 번째 선택 매핑: 카드 인덱스 {cardIndex} → 슬롯 {slotIndex}");
            }
            else
            {
                Debug.LogError($"[GameManager] 전차 두 번째 선택 오류 - 잘못된 카드 인덱스: {cardIndex}, 활성 슬롯 수: {activeSlots.Length}");
                yield break;
            }
        }
        else
        {
            // 일반 상황: 기존 로직 사용
            int[] slotIndices = GetSlotIndices(currentDrawnCards.Count);
            slotIndex = slotIndices[cardIndex];
            Debug.Log($"[GameManager] 일반 카드 강조: 카드 인덱스 {cardIndex} → 슬롯 인덱스 {slotIndex}");
        }

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

        yield return new WaitForSeconds(highlightTime * 0.4f);

        // 3. 원래 크기로 복원
        LeanTween.scale(cardTransform.gameObject, originalScale, highlightTime * 0.3f)
            .setEaseInBack();

        // 4. 원래 색상으로 복원
        LeanTween.value(cardImage.gameObject, cardImage.color, originalColor, highlightTime * 0.3f)
            .setOnUpdate((Color color) => cardImage.color = color);

        yield return new WaitForSeconds(highlightTime * 0.3f);
    }

    /// <summary>
    /// 선택된 카드 페이드 아웃 (전차 부분 수정)
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

        // 🔥 전차 효과 첫 번째 선택인지 확인
        if (isChariotActive && isChariotFirstPick)
        {
            Debug.Log("[GameManager] 전차 첫 번째 선택 - 선택된 카드만 빈 슬롯으로 설정");
            // 전차 첫 번째 선택: 선택된 카드만 빈 슬롯으로 설정
            unifiedCardManager.SetEmptySlot(slotIndex);

            // 🔥 나머지 카드들의 투명도 복원
            unifiedCardManager.RestoreAllCardAlpha();

            // 선택된 슬롯은 다시 빈 상태로 (투명하게)
            unifiedCardManager.SetEmptySlot(slotIndex);
        }
        else
        {
            Debug.Log("[GameManager] 일반 상황 또는 전차 두 번째 선택 - 모든 카드 슬롯 비우기");
            // 일반 상황 또는 전차 두 번째 선택: 모든 카드 슬롯을 빈 상태로 설정
            unifiedCardManager.SetAllEmptySlots();

            // CanvasGroup 알파값 복원 (다음 턴을 위해)
            unifiedCardManager.RestoreAllCardAlpha();
        }
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
    /// 카드 적용 로직 (전차 부분 수정)
    /// </summary>
    private void ContinueWithCardApplication(int index, Card selectedCard)
    {
        Debug.Log($"[GameManager] ===== ContinueWithCardApplication =====");
        Debug.Log($"[GameManager] 인덱스: {index}, 카드: '{selectedCard.Name}'");
        Debug.Log($"[GameManager] 전차 상태: active={isChariotActive}, firstPick={isChariotFirstPick}");
        Debug.Log($"[GameManager] 현재 카드들: [{string.Join(", ", currentDrawnCards.ConvertAll(c => c.Name))}]");

        // 🔥 인덱스 유효성 검사
        if (index < 0 || index >= currentDrawnCards.Count)
        {
            Debug.LogError($"[GameManager] 잘못된 카드 인덱스: {index}, 전체 카드 수: {currentDrawnCards.Count}");
            return;
        }

        // 선택된 카드를 목록에서 제거
        currentDrawnCards.RemoveAt(index);
        Debug.Log($"[GameManager] 카드 '{selectedCard.Name}' 제거 후 남은 카드: [{string.Join(", ", currentDrawnCards.ConvertAll(c => c.Name))}]");

        // 카드 효과 적용
        ApplyCard(selectedCard, currentDrawnCards);

        if (isChariotActive)
        {
            if (isChariotFirstPick)
            {
                Debug.Log("[GameManager] 전차 첫 번째 선택 완료 - 두 번째 선택 준비");
                isChariotFirstPick = false;

                // 🔥 전차 효과: 첫 번째 선택 후 남은 2장을 특정 슬롯에 배치
                DisplayChariotRemainingCards(index, currentDrawnCards);
                UpdateTurnDisplay();
                return;
            }
            else
            {
                Debug.Log("[GameManager] 전차 두 번째 선택 완료 - 전차 효과 종료");
                isChariotActive = false;
                UnityGame.Turn++;
                UpdateTurnDisplay();
                StartTurn();
                return;
            }
        }

        Debug.Log("[GameManager] 일반 카드 선택 완료 - 다음 턴 시작");
        UnityGame.Turn++;
        UpdateTurnDisplay();
        StartTurn();
    }

    /// <summary>
    /// 전차 효과: 첫 번째 선택 후 남은 2장 배치 (원래 위치 그대로 유지)
    /// </summary>
    private void DisplayChariotRemainingCards(int selectedIndex, List<Card> remainingCards)
    {
        Debug.Log($"[GameManager] ===== 전차 효과 시작 =====");
        Debug.Log($"[GameManager] 첫 번째 선택 인덱스: {selectedIndex}");
        Debug.Log($"[GameManager] 남은 카드: {string.Join(", ", remainingCards.ConvertAll(c => c.Name))}");

        if (remainingCards.Count != 2)
        {
            Debug.LogError($"[GameManager] 전차 효과 오류 - 남은 카드가 2장이 아닙니다: {remainingCards.Count}장");
            return;
        }

        // 🔥 선택된 슬롯만 빈 상태로 만들고, 나머지는 그대로 유지
        int[] originalSlots = GetSlotIndices(3); // [0, 1, 2]
        int selectedSlotIndex = originalSlots[selectedIndex];

        // 선택된 슬롯만 비우기
        unifiedCardManager.SetEmptySlot(selectedSlotIndex);
        unifiedCardManager.SetCardButtonActive(selectedSlotIndex, false);
        unifiedCardManager.isCardFront[selectedSlotIndex] = false;

        // 🔥 남은 카드들의 슬롯 인덱스 계산 (원래 위치 유지)
        List<int> remainingSlotsList = new List<int>();
        for (int i = 0; i < 3; i++)
        {
            if (i != selectedIndex) // 선택되지 않은 슬롯들
            {
                remainingSlotsList.Add(originalSlots[i]);
            }
        }
        int[] remainingSlots = remainingSlotsList.ToArray();

        Debug.Log($"[GameManager] 선택된 슬롯: {selectedSlotIndex} (비움)");
        Debug.Log($"[GameManager] 남은 카드 슬롯: [{string.Join(", ", remainingSlots)}] (그대로 유지)");

        // 🔥 남은 카드들의 버튼만 활성화 (이미지는 이미 표시되어 있음)
        for (int i = 0; i < remainingSlots.Length; i++)
        {
            int slotIndex = remainingSlots[i];
            unifiedCardManager.SetCardButtonActive(slotIndex, true);
            unifiedCardManager.RestoreCardAlpha(slotIndex);

            Debug.Log($"[GameManager] 슬롯 {slotIndex} 버튼 활성화 (카드: '{remainingCards[i].Name}')");
        }

        // 🔥 currentDrawnCards는 남은 카드들의 원래 순서 유지
        // remainingCards는 이미 올바른 순서로 되어 있음 (selectedIndex 제거 후)
        currentDrawnCards = new List<Card>(remainingCards);

        Debug.Log($"[GameManager] 전차 배치 완료:");
        for (int i = 0; i < remainingSlots.Length; i++)
        {
            Debug.Log($"  - 슬롯 {remainingSlots[i]}: '{remainingCards[i].Name}' (원래 위치 유지)");
        }
        Debug.Log($"  - currentDrawnCards: [{string.Join(", ", currentDrawnCards.ConvertAll(c => c.Name))}]");
        Debug.Log($"[GameManager] ===== 전차 효과 완료 =====");
    }



    /// <summary>
    /// 카드들을 슬롯 순서대로 재정렬
    /// </summary>
    private List<Card> ReorderCardsBySlots(List<Card> cards, int[] slotIndices)
    {
        List<Card> reorderedCards = new List<Card>();

        // 슬롯 인덱스와 카드를 매핑
        var slotCardPairs = new List<(int slotIndex, Card card)>();
        for (int i = 0; i < Math.Min(cards.Count, slotIndices.Length); i++)
        {
            slotCardPairs.Add((slotIndices[i], cards[i]));
        }

        // 슬롯 인덱스 순으로 정렬 (0, 1, 2 순서)
        slotCardPairs.Sort((a, b) => a.slotIndex.CompareTo(b.slotIndex));

        // 정렬된 순서대로 카드 배열 생성
        foreach (var pair in slotCardPairs)
        {
            reorderedCards.Add(pair.card);
            Debug.Log($"[GameManager] 슬롯 {pair.slotIndex} → 카드 '{pair.card.Name}'");
        }

        return reorderedCards;
    }

    /// <summary>
    /// 카드를 한 슬롯에서 다른 슬롯으로 이동 (수정됨)
    /// </summary>
    private void MoveCardToSlot(int fromSlot, int toSlot, Card card)
    {
        Debug.Log($"[GameManager] 카드 '{card.Name}' 이동: 슬롯 {fromSlot} → {toSlot}");

        // 🔥 원래 슬롯을 빈 상태로 설정
        unifiedCardManager.SetEmptySlot(fromSlot);
        unifiedCardManager.SetCardButtonActive(fromSlot, false);
        unifiedCardManager.isCardFront[fromSlot] = false;

        // 🔥 새 슬롯에 카드 설정 (새로운 메서드 사용)
        unifiedCardManager.SetCardSlotDirect(toSlot, card);
        // 버튼 활성화는 DisplayChariotRemainingCards에서 일괄 처리

        // 🔥 투명도 복원
        unifiedCardManager.RestoreCardAlpha(toSlot);

        Debug.Log($"[GameManager] 카드 이동 완료: '{card.Name}' → 슬롯 {toSlot}");
    }

    /// <summary>
    /// 전차 효과에서 첫 번째 선택에 따른 남은 2장의 배치 슬롯 반환
    /// </summary>
    private int[] GetChariotRemainingSlots(int selectedIndex)
    {
        // 원래 3장이 [0, 1, 2] 슬롯에 배치되어 있었다고 가정
        switch (selectedIndex)
        {
            case 0: // Left 선택 → Middle, Right 슬롯에 배치
                return new int[] { 1, 2 };
            case 1: // Middle 선택 → Left, Right 슬롯에 배치  
                return new int[] { 0, 2 };
            case 2: // Right 선택 → Left, Middle 슬롯에 배치
                return new int[] { 0, 1 };
            default:
                Debug.LogError($"[GameManager] 잘못된 선택 인덱스: {selectedIndex}");
                return new int[] { 0, 1 }; // 기본값
        }
    }

    /// <summary>
    /// 전차 효과 남은 카드들 애니메이션
    /// </summary>
    private System.Collections.IEnumerator AnimateChariotRemainingCards(List<Card> remainingCards, int[] targetSlots)
    {
        float moveDuration = 0.5f;
        float flipDelay = 0.5f;
        float flipDuration = 0.3f;

        // 1. 카드들을 아래에서 올라오게 하는 애니메이션
        for (int i = 0; i < 2; i++)
        {
            int slotIndex = targetSlots[i];
            RectTransform rt = unifiedCardManager.cardImageSlots[slotIndex].GetComponent<RectTransform>();
            if (rt == null) continue;

            Vector2 originalPos = rt.anchoredPosition;
            Vector2 startPos = originalPos + new Vector2(0, -Screen.height);
            rt.anchoredPosition = startPos;

            // 슬라이드 업
            LeanTween.move(rt, originalPos, moveDuration).setEaseOutCubic();

            if (i == 0) // 첫 번째 카드만 딜레이 후 두 번째 카드
                yield return new WaitForSeconds(0.2f);
        }

        // 2. 전부 올라온 후 대기
        yield return new WaitForSeconds(flipDelay);

        // 3. 카드 뒤집기 (1단계: 0→90도)
        for (int i = 0; i < 2; i++)
        {
            int slotIndex = targetSlots[i];
            RectTransform rt = unifiedCardManager.cardImageSlots[slotIndex].GetComponent<RectTransform>();
            if (rt == null) continue;

            LeanTween.rotateY(rt.gameObject, 90f, flipDuration / 2).setEaseInOutSine();
        }

        yield return new WaitForSeconds(flipDuration / 2);

        // 4. 90도에서 앞면 이미지로 교체
        for (int i = 0; i < 2; i++)
        {
            int slotIndex = targetSlots[i];
            SetChariotCardSlot(slotIndex, remainingCards[i]);
            unifiedCardManager.isCardFront[slotIndex] = true;
        }

        // 5. 2단계: 90→0도
        for (int i = 0; i < 2; i++)
        {
            int slotIndex = targetSlots[i];
            RectTransform rt = unifiedCardManager.cardImageSlots[slotIndex].GetComponent<RectTransform>();
            if (rt == null) continue;

            LeanTween.rotateY(rt.gameObject, 0f, flipDuration / 2).setEaseInOutSine();
        }

        yield return new WaitForSeconds(flipDuration / 2);
    }

    /// <summary>
    /// 전차 효과용 카드 슬롯 설정
    /// </summary>
    private void SetChariotCardSlot(int slotIndex, Card card)
    {
        if (slotIndex >= unifiedCardManager.cardImageSlots.Length) return;

        var imageSlot = unifiedCardManager.cardImageSlots[slotIndex];
        int cardIndex = GetCardIndex(card);

        Debug.Log($"[GameManager] 전차 슬롯 {slotIndex}에 '{card.Name}' 카드 표시 (인덱스: {cardIndex})");

        // 카드 이미지 설정
        if (cardIndex >= 0 && cardIndex < unifiedCardManager.cardSprites.Length && unifiedCardManager.cardSprites[cardIndex] != null)
        {
            imageSlot.sprite = unifiedCardManager.cardSprites[cardIndex];
            imageSlot.color = Color.white;
        }
        else
        {
            SetDefaultCardImage(imageSlot, card);
        }

        // 카드 이름 텍스트 설정
        if (slotIndex < unifiedCardManager.cardNameTexts.Length && unifiedCardManager.cardNameTexts[slotIndex] != null)
        {
            unifiedCardManager.cardNameTexts[slotIndex].text = card.Name;
        }
    }

    /// <summary>
    /// 카드 인덱스 가져오기 (CardLibrary에서)
    /// </summary>
    private int GetCardIndex(Card card)
    {
        for (int i = 0; i < CardLibrary.AllCards.Count; i++)
        {
            if (CardLibrary.AllCards[i].Name == card.Name)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 기본 카드 이미지 설정
    /// </summary>
    private void SetDefaultCardImage(Image imageSlot, Card card)
    {
        Debug.LogWarning($"[GameManager] '{card.Name}' 스프라이트 없음");

        if (unifiedCardManager.defaultCardSprite != null)
        {
            imageSlot.sprite = unifiedCardManager.defaultCardSprite;
            imageSlot.color = GetCardTypeColor(card);
        }
        else
        {
            imageSlot.sprite = null;
            imageSlot.color = GetCardTypeColor(card);
        }
    }

    /// <summary>
    /// 카드 타입별 색상 (UnifiedCardManager에서 복사)
    /// </summary>
    private Color GetCardTypeColor(Card card)
    {
        switch (card.Name)
        {
            case "죽음": return Color.black;
            case "바보": return new Color(1f, 0.5f, 0f); // 주황색
            case "전차":
            case "매달린 남자": return Color.cyan;
            case "세계": return Color.magenta;
            default:
                if (card.HpChange > 0) return Color.green;
                else if (card.HpChange < 0) return Color.red;
                else if (card.CurseChange > 0) return new Color(0.5f, 0f, 0.5f); // 보라색
                else if (card.CurseChange < 0) return Color.blue;
                else return Color.yellow;
        }
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