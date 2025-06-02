#nullable disable
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame;
using System.Collections.Generic;

public class CardEffectPreview : MonoBehaviour
{
    [Header("기존 구조 활용 - 카드 효과 미리보기")]
    [Tooltip("UnifiedCardManager와 자동 동기화")]
    public bool autoSync = true;

    [Header("수동 설정 (autoSync가 false일 때만)")]
    public GameObject[] effectPanels = new GameObject[3];
    public TextMeshProUGUI[] effectTexts = new TextMeshProUGUI[3];

    [Header("색상 설정")]
    public Color positiveColor = Color.green;
    public Color negativeColor = Color.red;
    public Color neutralColor = Color.white;
    public Color specialColor = Color.yellow;

    [Header("애니메이션 설정")]
    public float animationDuration = 0.3f;
    public bool useFadeAnimation = true;

    private CanvasGroup[] panelCanvasGroups;
    private UnifiedCardManager unifiedCardManager;
    private List<Card> lastDisplayedCards = new List<Card>();

    private void Awake()
    {
        // UnifiedCardManager 찾기
        unifiedCardManager = FindObjectOfType<UnifiedCardManager>();

        if (autoSync)
        {
            AutoFindCardEffectPanels();
        }

        InitializeCanvasGroups();
        HideAllPanels();
    }

    private void Start()
    {
        // UnifiedCardManager의 DisplayCards가 호출될 때마다 동기화
        StartCoroutine(MonitorUnifiedCardManager());
    }

    /// <summary>
    /// UnifiedCardManager와 동기화 모니터링
    /// </summary>
    private System.Collections.IEnumerator MonitorUnifiedCardManager()
    {
        yield return new WaitForSeconds(1f); // 초기 대기

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (GameManager.Instance != null)
            {
                try
                {
                    // GameManager에서 현재 표시된 카드들 가져오기
                    var currentCards = GameManager.Instance.GetType()
                        .GetField("currentDrawnCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                        ?.GetValue(GameManager.Instance) as List<Card>;

                    if (currentCards != null && currentCards.Count > 0)
                    {
                        if (!AreCardListsEqual(currentCards, lastDisplayedCards))
                        {
                            Debug.Log($"[CardEffectPreview] 카드 변화 감지: {currentCards.Count}장");
                            for (int i = 0; i < currentCards.Count; i++)
                            {
                                Debug.Log($"  - 슬롯 {i}: {currentCards[i]?.Name}");
                            }

                            UpdateCardEffectsFromCards(currentCards);
                            lastDisplayedCards = new List<Card>(currentCards);
                        }
                    }
                    else
                    {
                        // 카드가 없으면 모든 패널 숨기기
                        if (lastDisplayedCards.Count > 0)
                        {
                            Debug.Log("[CardEffectPreview] 카드 없음 - 모든 패널 숨김");
                            HideAllPanels();
                            lastDisplayedCards.Clear();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CardEffectPreview] 동기화 중 오류: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 카드 리스트가 동일한지 확인
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
    /// 카드 객체에서 직접 효과 업데이트
    /// </summary>
    private void UpdateCardEffectsFromCards(List<Card> cards)
    {
        Debug.Log($"[CardEffectPreview] 효과 업데이트 시작 - {cards.Count}장의 카드");

        for (int i = 0; i < effectPanels.Length; i++)
        {
            if (effectPanels[i] == null)
            {
                Debug.LogWarning($"[CardEffectPreview] 슬롯 {i} 패널이 null입니다.");
                continue;
            }

            if (i < cards.Count && cards[i] != null)
            {
                Debug.Log($"[CardEffectPreview] 슬롯 {i}에 '{cards[i].Name}' 효과 표시");
                ShowCardEffect(i, cards[i]);
            }
            else
            {
                Debug.Log($"[CardEffectPreview] 슬롯 {i} 숨김");
                HideCardEffect(i);
            }
        }
    }

    /// <summary>
    /// 카드 효과 패널 자동 찾기
    /// </summary>
    private void AutoFindCardEffectPanels()
    {
        effectPanels = new GameObject[3];
        effectTexts = new TextMeshProUGUI[3];

        string[] cardSlotNames = { "CardLeft", "CardMiddle", "CardRight" };
        string[] panelNames = { "CardEffectPanelLeft", "CardEffectPanelMiddle", "CardEffectPanelRight" };

        for (int i = 0; i < cardSlotNames.Length; i++)
        {
            // 전체 씬에서 카드 슬롯 찾기
            Transform cardSlot = FindInAllChildren(cardSlotNames[i]);

            if (cardSlot != null)
            {
                Debug.Log($"[CardEffectPreview] {cardSlotNames[i]} 찾음");

                Transform effectPanel = cardSlot.Find(panelNames[i]);

                if (effectPanel != null)
                {
                    effectPanels[i] = effectPanel.gameObject;
                    Debug.Log($"[CardEffectPreview] {panelNames[i]} 패널 찾음");

                    Transform effectTextTransform = effectPanel.Find("CardEffectText");
                    if (effectTextTransform != null)
                    {
                        effectTexts[i] = effectTextTransform.GetComponent<TextMeshProUGUI>();

                        if (effectTexts[i] != null)
                        {
                            Debug.Log($"[CardEffectPreview] {cardSlotNames[i]} TextMeshPro 연결 성공");
                        }
                        else
                        {
                            var legacyText = effectTextTransform.GetComponent<Text>();
                            if (legacyText != null)
                            {
                                Debug.LogWarning($"[CardEffectPreview] {cardSlotNames[i]}는 Text 컴포넌트입니다. TextMeshPro 권장.");
                            }
                            else
                            {
                                Debug.LogError($"[CardEffectPreview] {cardSlotNames[i]} 텍스트 컴포넌트 없음!");
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"[CardEffectPreview] {panelNames[i]} 아래에 CardEffectText 없음!");
                    }
                }
                else
                {
                    Debug.LogError($"[CardEffectPreview] {cardSlotNames[i]} 아래에 {panelNames[i]} 없음!");
                }
            }
            else
            {
                Debug.LogError($"[CardEffectPreview] {cardSlotNames[i]} 자체를 찾을 수 없음!");
            }
        }

        int connectedCount = System.Array.FindAll(effectPanels, p => p != null).Length;
        int textCount = System.Array.FindAll(effectTexts, t => t != null).Length;

        Debug.Log($"[CardEffectPreview] 연결 완료 - 패널: {connectedCount}/3, 텍스트: {textCount}/3");
    }

    /// <summary>
    /// 전체 씬에서 오브젝트 찾기
    /// </summary>
    private Transform FindInAllChildren(string objectName)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == objectName && obj.scene.isLoaded)
            {
                return obj.transform;
            }
        }
        return null;
    }

    /// <summary>
    /// CanvasGroup 초기화
    /// </summary>
    private void InitializeCanvasGroups()
    {
        panelCanvasGroups = new CanvasGroup[effectPanels.Length];

        for (int i = 0; i < effectPanels.Length; i++)
        {
            if (effectPanels[i] != null)
            {
                panelCanvasGroups[i] = effectPanels[i].GetComponent<CanvasGroup>();
                if (panelCanvasGroups[i] == null)
                {
                    panelCanvasGroups[i] = effectPanels[i].AddComponent<CanvasGroup>();
                }
            }
        }
    }

    /// <summary>
    /// 카드 효과 표시 (간소화된 버전)
    /// </summary>
    private void ShowCardEffect(int slotIndex, Card card)
    {
        if (slotIndex >= effectPanels.Length || effectPanels[slotIndex] == null || card == null)
        {
            Debug.LogWarning($"[CardEffectPreview] ShowCardEffect 실패 - slotIndex: {slotIndex}, panel: {effectPanels[slotIndex] != null}, card: {card?.Name}");
            return;
        }

        Debug.Log($"[CardEffectPreview] 슬롯 {slotIndex}에 카드 '{card.Name}' 표시 시작");

        effectPanels[slotIndex].SetActive(true);

        if (effectTexts[slotIndex] != null)
        {
            string effectDescription = BuildEffectDescription(card);
            effectTexts[slotIndex].text = effectDescription;

            Debug.Log($"[CardEffectPreview] 슬롯 {slotIndex} 텍스트 설정: '{effectDescription}'");

            Color textColor = GetEffectColor(card.HpChange, card.CurseChange, card.Special != null);
            effectTexts[slotIndex].color = textColor;
        }
        else
        {
            Debug.LogError($"[CardEffectPreview] 슬롯 {slotIndex}의 effectTexts가 null입니다!");
        }

        if (useFadeAnimation && panelCanvasGroups[slotIndex] != null)
        {
            StartCoroutine(FadeInPanel(slotIndex));
        }
        else if (panelCanvasGroups[slotIndex] != null)
        {
            panelCanvasGroups[slotIndex].alpha = 1f;
        }

        Debug.Log($"[CardEffectPreview] 슬롯 {slotIndex} 표시 완료");
    }

    /// <summary>
    /// 효과 설명 텍스트 생성 (실제 카드 데이터 기반)
    /// </summary>
    private string BuildEffectDescription(Card card)
    {
        List<string> effects = new List<string>();

        // HP 변화 (실제 카드 데이터에서)
        if (card.HpChange > 0)
            effects.Add($"<color=#00FF00>체력 +{card.HpChange}</color>");
        else if (card.HpChange < 0)
            effects.Add($"<color=#FF0000>체력 {card.HpChange}</color>");

        // 저주 변화 (실제 카드 데이터에서)
        if (card.CurseChange > 0)
            effects.Add($"<color=#FF0000>저주 +{card.CurseChange}</color>");
        else if (card.CurseChange < 0)
            effects.Add($"<color=#00FF00>저주 {card.CurseChange}</color>");

        // 특수 효과 (카드 Description에서 가져오기)
        if (!string.IsNullOrEmpty(card.Description))
        {
            effects.Add($"<color=#FFFF00>{card.Description}</color>");
        }

        // 효과가 없는 경우
        if (effects.Count == 0)
        {
            return "<color=#888888>효과 없음</color>";
        }

        return string.Join("\n", effects);
    }

    /// <summary>
    /// 효과 색상 결정 (실제 카드 데이터 기반)
    /// </summary>
    private Color GetEffectColor(int hpChange, int curseChange, bool hasSpecial)
    {
        // 전체적인 효과 판단
        bool isPositive = (hpChange > 0) || (curseChange < 0);
        bool isNegative = (hpChange < 0) || (curseChange > 0);

        if (hasSpecial)
            return specialColor;
        else if (isPositive && !isNegative)
            return positiveColor;
        else if (isNegative && !isPositive)
            return negativeColor;
        else
            return neutralColor;
    }

    /// <summary>
    /// 카드 효과 숨김
    /// </summary>
    private void HideCardEffect(int slotIndex)
    {
        if (slotIndex >= effectPanels.Length || effectPanels[slotIndex] == null) return;

        if (useFadeAnimation && panelCanvasGroups[slotIndex] != null)
        {
            StartCoroutine(FadeOutPanel(slotIndex));
        }
        else
        {
            effectPanels[slotIndex].SetActive(false);
        }
    }

    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    private System.Collections.IEnumerator FadeInPanel(int slotIndex)
    {
        var canvasGroup = panelCanvasGroups[slotIndex];
        if (canvasGroup == null) yield break;

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsedTime / animationDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 페이드 아웃 애니메이션
    /// </summary>
    private System.Collections.IEnumerator FadeOutPanel(int slotIndex)
    {
        var canvasGroup = panelCanvasGroups[slotIndex];
        if (canvasGroup == null)
        {
            effectPanels[slotIndex].SetActive(false);
            yield break;
        }

        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / animationDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        effectPanels[slotIndex].SetActive(false);
    }

    /// <summary>
    /// 모든 패널 숨김
    /// </summary>
    private void HideAllPanels()
    {
        for (int i = 0; i < effectPanels.Length; i++)
        {
            if (effectPanels[i] != null)
            {
                effectPanels[i].SetActive(false);
                if (panelCanvasGroups[i] != null)
                {
                    panelCanvasGroups[i].alpha = 0f;
                }
            }
        }
    }

    /// <summary>
    /// 수동 새로고침 (테스트용)
    /// </summary>
    [ContextMenu("Force Refresh")]
    public void ForceRefresh()
    {
        if (GameManager.Instance != null)
        {
            var currentCards = GameManager.Instance.GetType()
                .GetField("currentDrawnCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.GetValue(GameManager.Instance) as List<Card>;

            if (currentCards != null)
            {
                UpdateCardEffectsFromCards(currentCards);
                Debug.Log("[CardEffectPreview] 수동 새로고침 완료");
            }
        }
    }

    /// <summary>
    /// 연결 상태 재검사
    /// </summary>
    [ContextMenu("Re-scan Connections")]
    public void RescanConnections()
    {
        AutoFindCardEffectPanels();
        InitializeCanvasGroups();
        Debug.Log("[CardEffectPreview] 연결 재검사 완료");
    }
}