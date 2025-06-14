#nullable disable
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame;
using System.Collections.Generic;
using System.Linq;

public class CardEffectManager : MonoBehaviour
{
    public static CardEffectManager Instance { get; private set; }

    [Header("=== 카드별 3분할 효과 시스템 ===")]
    [Tooltip("각 카드마다 3개의 효과 영역을 가집니다: HP, 저주, 기타")]

    [Header("카드 1 (Left) - 효과 영역들")]
    public GameObject card1_HpPanel;
    public GameObject card1_CursePanel;
    public GameObject card1_SpecialPanel;
    public TextMeshProUGUI card1_HpText;
    public TextMeshProUGUI card1_CurseText;
    public TextMeshProUGUI card1_SpecialText;

    [Header("카드 2 (Middle) - 효과 영역들")]
    public GameObject card2_HpPanel;
    public GameObject card2_CursePanel;
    public GameObject card2_SpecialPanel;
    public TextMeshProUGUI card2_HpText;
    public TextMeshProUGUI card2_CurseText;
    public TextMeshProUGUI card2_SpecialText;

    [Header("카드 3 (Right) - 효과 영역들")]
    public GameObject card3_HpPanel;
    public GameObject card3_CursePanel;
    public GameObject card3_SpecialPanel;
    public TextMeshProUGUI card3_HpText;
    public TextMeshProUGUI card3_CurseText;
    public TextMeshProUGUI card3_SpecialText;

    [Header("=== 색상 설정 ===")]
    public Color hpPositiveColor = Color.green;
    public Color hpNegativeColor = Color.red;
    public Color cursePositiveColor = Color.green;  // 저주 감소는 좋은 것
    public Color curseNegativeColor = Color.red;    // 저주 증가는 나쁜 것
    public Color specialColor = Color.yellow;
    public Color noEffectColor = Color.gray;

    [Header("=== 애니메이션 설정 ===")]
    public float fadeInDuration = 0.3f;
    public bool useScaleAnimation = true;
    public float scaleAnimationDuration = 0.2f;

    // 내부 구조체들
    private CardEffectData[] cardEffects = new CardEffectData[3];
    private List<Card> lastDisplayedCards = new List<Card>();

    private struct CardEffectData
    {
        public GameObject hpPanel;
        public GameObject cursePanel;
        public GameObject specialPanel;
        public TextMeshProUGUI hpText;
        public TextMeshProUGUI curseText;
        public TextMeshProUGUI specialText;
    }

    private void Awake()
    {
        Instance = this;

        InitializeCardEffectData();
        HideAllEffects();
    }

    private void Start()
    {
        StartCoroutine(MonitorCardChanges());
    }

    /// <summary>
    /// 카드 효과 데이터 구조 초기화
    /// </summary>
    private void InitializeCardEffectData()
    {
        // 카드 1 (Left)
        cardEffects[0] = new CardEffectData
        {
            hpPanel = card1_HpPanel,
            cursePanel = card1_CursePanel,
            specialPanel = card1_SpecialPanel,
            hpText = card1_HpText,
            curseText = card1_CurseText,
            specialText = card1_SpecialText
        };

        // 카드 2 (Middle)
        cardEffects[1] = new CardEffectData
        {
            hpPanel = card2_HpPanel,
            cursePanel = card2_CursePanel,
            specialPanel = card2_SpecialPanel,
            hpText = card2_HpText,
            curseText = card2_CurseText,
            specialText = card2_SpecialText
        };

        // 카드 3 (Right)
        cardEffects[2] = new CardEffectData
        {
            hpPanel = card3_HpPanel,
            cursePanel = card3_CursePanel,
            specialPanel = card3_SpecialPanel,
            hpText = card3_HpText,
            curseText = card3_CurseText,
            specialText = card3_SpecialText
        };

        Debug.Log("[CardEffectManager] 카드 효과 데이터 초기화 완료");
    }

    /// <summary>
    /// 카드 변화 모니터링
    /// </summary>
    private System.Collections.IEnumerator MonitorCardChanges()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            if (GameManager.Instance != null)
            {
                try
                {
                    var currentCards = GetCurrentCards();

                    // 🔥 전차 효과 중인지 확인
                    bool isChariotSecondPick = IsChariotSecondPick();

                    if (currentCards != null && currentCards.Count > 0)
                    {
                        // 🔥 카드가 변경되었거나 전차 모드에서 아직 효과가 표시되지 않은 경우만 업데이트
                        bool needsUpdate = !AreCardListsEqual(currentCards, lastDisplayedCards);

                        // 🔥 전차 모드에서는 추가 조건 확인하지 않음 (무한루프 방지)
                        if (needsUpdate)
                        {
                            Debug.Log($"[CardEffectManager] 카드 변화 감지: {currentCards.Count}장 (전차: {isChariotSecondPick})");
                            StartCoroutine(UpdateAllCardEffects(currentCards));
                            lastDisplayedCards = new List<Card>(currentCards);
                        }
                    }
                    else if ((currentCards == null || currentCards.Count == 0) && lastDisplayedCards.Count > 0)
                    {
                        Debug.Log("[CardEffectManager] 카드 없음 - 모든 효과 숨김");
                        HideAllEffects();
                        lastDisplayedCards.Clear();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CardEffectManager] 모니터링 중 오류: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 전차 효과가 이미 표시되었는지 확인
    /// </summary>
    private bool AreChariotEffectsDisplayed(List<Card> currentCards)
    {
        var activeSlots = GetActiveChariotSlots();

        // 활성 슬롯 수와 카드 수가 맞지 않으면 아직 표시되지 않음
        if (activeSlots.Length != currentCards.Count)
            return false;

        // 각 슬롯의 효과 패널이 활성화되어 있는지 확인
        for (int i = 0; i < activeSlots.Length && i < currentCards.Count; i++)
        {
            int slotIndex = activeSlots[i];
            if (slotIndex < cardEffects.Length)
            {
                var effectData = cardEffects[slotIndex];

                // HP, 저주, 특수 효과 중 하나라도 표시되어 있으면 효과가 표시된 것으로 간주
                bool hasEffect = (effectData.hpPanel != null && effectData.hpPanel.activeSelf) ||
                               (effectData.cursePanel != null && effectData.cursePanel.activeSelf) ||
                               (effectData.specialPanel != null && effectData.specialPanel.activeSelf);

                if (!hasEffect)
                    return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 전차 효과의 두 번째 선택인지 확인
    /// </summary>
    private bool IsChariotSecondPick()
    {
        if (GameManager.Instance == null) return false;

        try
        {
            var isChariotActiveField = GameManager.Instance.GetType()
                .GetField("isChariotActive", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var isChariotFirstPickField = GameManager.Instance.GetType()
                .GetField("isChariotFirstPick", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (isChariotActiveField != null && isChariotFirstPickField != null)
            {
                bool isChariotActive = (bool)isChariotActiveField.GetValue(GameManager.Instance);
                bool isChariotFirstPick = (bool)isChariotFirstPickField.GetValue(GameManager.Instance);

                return isChariotActive && !isChariotFirstPick;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CardEffectManager] 전차 상태 확인 실패: {e.Message}");
        }

        return false;
    }

    /// <summary>
    /// 모든 카드 효과 업데이트 (메인 메서드)
    /// </summary>
    public System.Collections.IEnumerator UpdateAllCardEffects(List<Card> cards)
    {
        Debug.Log($"[CardEffectManager] {cards.Count}장 카드 효과 업데이트");

        // 전차 효과 중인지 확인
        bool isChariotMode = IsChariotSecondPick();
        Debug.Log($"[CardEffectManager] 전차모드: {isChariotMode}");

        // 모든 효과 먼저 숨김
        HideAllEffects();

        if (isChariotMode)
        {
            // 🔥 전차 모드: 활성화된 슬롯만 효과 표시
            yield return StartCoroutine(UpdateChariotCardEffects(cards));
        }
        else
        {
            // 🔥 일반 모드: 기존 로직
            yield return StartCoroutine(UpdateNormalCardEffects(cards));
        }
    }

    /// <summary>
    /// 전차 모드 카드 효과 업데이트
    /// </summary>
    private System.Collections.IEnumerator UpdateChariotCardEffects(List<Card> cards)
    {
        Debug.Log($"[CardEffectManager] 전차 모드 효과 업데이트: {cards.Count}장");

        // 🔥 전차 모드에서는 활성 슬롯 순서대로 카드 효과 표시 (이름 매칭 대신 순서 매칭)
        var activeSlots = GetActiveChariotSlots();
        Debug.Log($"[CardEffectManager] 전차 활성 슬롯: [{string.Join(", ", activeSlots)}]");

        // 🔥 활성 슬롯 순서와 카드 배열 순서를 1:1 매칭
        for (int i = 0; i < Mathf.Min(activeSlots.Length, cards.Count); i++)
        {
            int slotIndex = activeSlots[i];
            Card cardToShow = cards[i]; // 순서대로 매칭

            // 카드가 앞면이 될 때까지 대기
            int maxWait = 20;
            int waitCount = 0;
            while (!UnifiedCardManager.Instance.isCardFront[slotIndex] && waitCount < maxWait)
            {
                yield return new WaitForSeconds(0.1f);
                waitCount++;
            }

            if (UnifiedCardManager.Instance.isCardFront[slotIndex])
            {
                ShowCardEffects(slotIndex, cardToShow);
                Debug.Log($"[CardEffectManager] 전차 슬롯 {slotIndex}에 '{cardToShow.Name}' 효과 표시 (카드배열인덱스: {i})");
            }
            else
            {
                Debug.LogWarning($"[CardEffectManager] 전차 슬롯 {slotIndex} 카드가 앞면이 되지 않음");
            }
        }
    }

    /// <summary>
    /// 특정 슬롯에 표시된 카드 이름 가져오기
    /// </summary>
    private string GetCardNameInSlot(int slotIndex)
    {
        if (UnifiedCardManager.Instance == null) return "";

        // 🔥 UnifiedCardManager의 cardNameTexts에서 카드 이름 가져오기
        if (slotIndex < UnifiedCardManager.Instance.cardNameTexts.Length &&
            UnifiedCardManager.Instance.cardNameTexts[slotIndex] != null)
        {
            string cardName = UnifiedCardManager.Instance.cardNameTexts[slotIndex].text;
            Debug.Log($"[CardEffectManager] 슬롯 {slotIndex}에서 카드 이름 '{cardName}' 감지");
            return cardName;
        }

        Debug.LogWarning($"[CardEffectManager] 슬롯 {slotIndex}에서 카드 이름을 가져올 수 없음");
        return "";
    }

    /// <summary>
    /// 일반 모드 카드 효과 업데이트
    /// </summary>
    private System.Collections.IEnumerator UpdateNormalCardEffects(List<Card> cards)
    {
        Debug.Log($"[CardEffectManager] 일반 모드 효과 업데이트: {cards.Count}장");

        // 카드 배치 슬롯 인덱스 가져오기
        int[] slotIndices = GetSlotIndices(cards.Count);

        // 배치된 카드들만 효과 표시
        for (int i = 0; i < cards.Count; i++)
        {
            int slotIndex = slotIndices[i];

            if (cards[i] != null)
            {
                // 카드가 앞면이 될 때까지 대기
                int maxWait = 50; // 5초 최대 대기
                int waitCount = 0;
                while (!UnifiedCardManager.Instance.isCardFront[slotIndex] && waitCount < maxWait)
                {
                    yield return new WaitForSeconds(0.3f);
                    waitCount++;
                }

                if (UnifiedCardManager.Instance.isCardFront[slotIndex])
                {
                    ShowCardEffects(slotIndex, cards[i]);
                }
            }
        }
    }

    /// <summary>
    /// 현재 활성화된 전차 슬롯들 가져오기 (정렬된 순서로)
    /// </summary>
    private int[] GetActiveChariotSlots()
    {
        List<int> activeSlots = new List<int>();

        if (UnifiedCardManager.Instance != null)
        {
            // 실제로 카드가 앞면으로 표시된 슬롯들 확인
            for (int i = 0; i < UnifiedCardManager.Instance.isCardFront.Length; i++)
            {
                if (UnifiedCardManager.Instance.isCardFront[i])
                {
                    activeSlots.Add(i);
                }
            }
        }

        // 🔥 슬롯 번호 순으로 정렬 (0, 1, 2 순서)
        activeSlots.Sort();

        var result = activeSlots.ToArray();
        Debug.Log($"[CardEffectManager] 감지된 활성 슬롯 (정렬됨): [{string.Join(", ", result)}]");
        return result;
    }

    /// <summary>
    /// 현재 표시된 카드들 가져오기
    /// </summary>
    private List<Card> GetCurrentCards()
    {
        return GameManager.Instance?.GetType()
            .GetField("currentDrawnCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(GameManager.Instance) as List<Card>;
    }

    /// <summary>
    /// 카드 리스트 비교
    /// </summary>
    private bool AreCardListsEqual(List<Card> cards1, List<Card> cards2)
    {
        if (cards1 == null && cards2 == null) return true;
        if (cards1 == null || cards2 == null) return false;
        if (cards1.Count != cards2.Count) return false;

        for (int i = 0; i < cards1.Count; i++)
        {
            if (cards1[i]?.Name != cards2[i]?.Name)
                return false;
        }
        return true;
    }

    /// <summary>
    /// 카드 수에 따른 슬롯 인덱스 배열 반환 (UnifiedCardManager와 동일한 로직)
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

    /// <summary>
    /// 특정 카드의 3분할 효과 표시
    /// </summary>
    private void ShowCardEffects(int cardIndex, Card card)
    {
        if (cardIndex >= cardEffects.Length) return;

        var effectData = cardEffects[cardIndex];
        Debug.Log($"[CardEffectManager] 카드 {cardIndex} '{card.Name}' 효과 표시");

        // 1. 체력 효과
        ShowHpEffect(effectData, card);

        // 2. 저주 효과
        ShowCurseEffect(effectData, card);

        // 3. 특수 효과
        ShowSpecialEffect(effectData, card);
    }

    /// <summary>
    /// 체력 효과 표시
    /// </summary>
    private void ShowHpEffect(CardEffectData effectData, Card card)
    {
        if (effectData.hpPanel == null || effectData.hpText == null) return;

        // 🔥 항상 표시 (0인 경우에도 +0으로 표시)
        effectData.hpPanel.SetActive(true);

        string hpText;
        Color textColor;

        if (card.HpChange > 0)
        {
            hpText = $"+{card.HpChange}";
            textColor = hpPositiveColor;
        }
        else if (card.HpChange < 0)
        {
            hpText = $"{card.HpChange}";
            textColor = hpNegativeColor;
        }
        else
        {
            hpText = "+0";
            textColor = noEffectColor;
        }

        effectData.hpText.text = $"{hpText}";
        effectData.hpText.color = textColor;

        if (useScaleAnimation)
            StartCoroutine(ScaleAnimation(effectData.hpPanel));
    }

    /// <summary>
    /// 저주 효과 표시
    /// </summary>
    private void ShowCurseEffect(CardEffectData effectData, Card card)
    {
        if (effectData.cursePanel == null || effectData.curseText == null) return;

        // 🔥 항상 표시 (0인 경우에도 +0으로 표시)
        effectData.cursePanel.SetActive(true);

        string curseText;
        Color textColor;

        if (card.CurseChange > 0)
        {
            curseText = $"+{card.CurseChange}";
            textColor = curseNegativeColor; // 저주 증가는 나쁨
        }
        else if (card.CurseChange < 0)
        {
            curseText = $"{card.CurseChange}";
            textColor = cursePositiveColor; // 저주 감소는 좋음
        }
        else
        {
            curseText = "+0";
            textColor = noEffectColor;
        }

        effectData.curseText.text = $"{curseText}";
        effectData.curseText.color = textColor;

        if (useScaleAnimation)
            StartCoroutine(ScaleAnimation(effectData.cursePanel));
    }

    /// <summary>
    /// 특수 효과 표시
    /// </summary>
    private void ShowSpecialEffect(CardEffectData effectData, Card card)
    {
        if (effectData.specialPanel == null || effectData.specialText == null) return;

        if (!string.IsNullOrEmpty(card.Description) || card.Special != null)
        {
            effectData.specialPanel.SetActive(true);

            string specialText = !string.IsNullOrEmpty(card.Description) ? card.Description : "특수 효과";
            effectData.specialText.text = specialText;
            effectData.specialText.color = specialColor;

            if (useScaleAnimation)
                StartCoroutine(ScaleAnimation(effectData.specialPanel));
        }
        else
        {
            effectData.specialPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 특정 카드의 모든 효과 숨김
    /// </summary>
    public void HideCardEffects(int cardIndex)
    {
        if (cardIndex >= cardEffects.Length) return;

        var effectData = cardEffects[cardIndex];

        if (effectData.hpPanel != null)
            effectData.hpPanel.SetActive(false);

        if (effectData.cursePanel != null)
            effectData.cursePanel.SetActive(false);

        if (effectData.specialPanel != null)
            effectData.specialPanel.SetActive(false);
    }

    /// <summary>
    /// 모든 효과 숨김
    /// </summary>
    public void HideAllEffects()
    {
        for (int i = 0; i < cardEffects.Length; i++)
        {
            HideCardEffects(i);
        }
    }

    /// <summary>
    /// 스케일 애니메이션
    /// </summary>
    private System.Collections.IEnumerator ScaleAnimation(GameObject panel)
    {
        if (panel == null) yield break;

        Vector3 originalScale = panel.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        // 확대
        float elapsedTime = 0f;
        while (elapsedTime < scaleAnimationDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (scaleAnimationDuration / 2);
            panel.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // 축소
        elapsedTime = 0f;
        while (elapsedTime < scaleAnimationDuration / 2)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (scaleAnimationDuration / 2);
            panel.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        panel.transform.localScale = originalScale;
    }

    /// <summary>
    /// 수동 새로고침 (테스트용)
    /// </summary>
    [ContextMenu("Force Refresh Effects")]
    public void ForceRefreshEffects()
    {
        var currentCards = GetCurrentCards();
        if (currentCards != null)
        {
            StartCoroutine(UpdateAllCardEffects(currentCards));
            Debug.Log("[CardEffectManager] 수동 새로고침 완료");
        }
    }

    /// <summary>
    /// 연결 상태 확인 (디버그용)
    /// </summary>
    [ContextMenu("Check Connections")]
    public void CheckConnections()
    {
        Debug.Log("[CardEffectManager] 연결 상태 확인:");

        for (int i = 0; i < cardEffects.Length; i++)
        {
            var effect = cardEffects[i];
            Debug.Log($"카드 {i + 1}:");
            Debug.Log($"  - HP Panel: {(effect.hpPanel != null ? "✓" : "✗")}");
            Debug.Log($"  - Curse Panel: {(effect.cursePanel != null ? "✓" : "✗")}");
            Debug.Log($"  - Special Panel: {(effect.specialPanel != null ? "✓" : "✗")}");
            Debug.Log($"  - HP Text: {(effect.hpText != null ? "✓" : "✗")}");
            Debug.Log($"  - Curse Text: {(effect.curseText != null ? "✓" : "✗")}");
            Debug.Log($"  - Special Text: {(effect.specialText != null ? "✓" : "✗")}");
        }
    }
}