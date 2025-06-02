
#nullable disable
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame;
using System.Collections.Generic;

public class CardEffectPreview : MonoBehaviour
{
    [Header("기존 구조 활용 - 카드 효과 미리보기")]
    [Tooltip("CardLeft, CardMiddle, CardRight의 CardEffectPanel들을 자동으로 찾습니다")]
    public bool autoFindPanels = true;

    [Header("수동 설정 (autoFindPanels이 false일 때만)")]
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
    private string[] cardSlotNames = { "CardLeft", "CardMiddle", "CardRight" };

    private void Awake()
    {
        if (autoFindPanels)
        {
            AutoFindCardEffectPanels();
        }

        InitializeCanvasGroups();
        HideAllPanels();
    }

    private void Start()
    {
        // 카드 변화 모니터링 시작
        StartCoroutine(MonitorCardChanges());
    }

    /// <summary>
    /// 기존 구조에서 CardEffectPanel들을 자동으로 찾기
    /// </summary>
    private void AutoFindCardEffectPanels()
    {
        effectPanels = new GameObject[3];
        effectTexts = new TextMeshProUGUI[3];

        // UnifiedCardManager 또는 GameManager에서 찾기 시도
        Transform searchRoot = transform;

        // UnifiedCardManager가 있다면 사용
        UnifiedCardManager unifiedCardManager = FindObjectOfType<UnifiedCardManager>();
        if (unifiedCardManager != null)
        {
            searchRoot = unifiedCardManager.transform.parent; // 부모에서 찾기
        }

        for (int i = 0; i < cardSlotNames.Length; i++)
        {
            // 전체 씬에서 카드 슬롯 찾기
            Transform cardSlot = FindInAllChildren(cardSlotNames[i]);

            if (cardSlot != null)
            {
                // CardEffectPanelLeft, CardEffectPanelMiddle, CardEffectPanelRight 찾기
                string panelName = GetPanelName(i);
                Transform effectPanel = cardSlot.Find(panelName);

                if (effectPanel != null)
                {
                    effectPanels[i] = effectPanel.gameObject;

                    // CardEffectText 찾기
                    Transform effectTextTransform = effectPanel.Find("CardEffectText");
                    if (effectTextTransform != null)
                    {
                        effectTexts[i] = effectTextTransform.GetComponent<TextMeshProUGUI>();

                        // Text 컴포넌트인 경우 확인
                        if (effectTexts[i] == null)
                        {
                            var legacyText = effectTextTransform.GetComponent<Text>();
                            if (legacyText != null)
                            {
                                Debug.LogWarning($"[CardEffectPreview] {cardSlotNames[i]}/CardEffectText는 Text 컴포넌트입니다. TextMeshPro 사용을 권장합니다.");
                            }
                        }
                    }

                    Debug.Log($"[CardEffectPreview] {cardSlotNames[i]} 찾음: {effectPanel.name}");
                }
                else
                {
                    Debug.LogWarning($"[CardEffectPreview] {cardSlotNames[i]} 아래에 {panelName}을 찾을 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"[CardEffectPreview] {cardSlotNames[i]}를 찾을 수 없습니다.");
            }
        }

        Debug.Log($"[CardEffectPreview] 자동 탐지 완료 - {System.Array.FindAll(effectPanels, p => p != null).Length}/3개 패널 발견");
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
    /// 카드 슬롯 인덱스에 따른 패널 이름 반환
    /// </summary>
    private string GetPanelName(int index)
    {
        switch (index)
        {
            case 0: return "CardEffectPanelLeft";
            case 1: return "CardEffectPanelMiddle";
            case 2: return "CardEffectPanelRight";
            default: return "CardEffectPanel";
        }
    }

    /// <summary>
    /// 자식 오브젝트에서 이름으로 찾기 (재귀)
    /// </summary>
    private Transform FindInChildren(Transform parent, string name)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.name == name)
            {
                return child;
            }

            Transform found = FindInChildren(child, name);
            if (found != null)
            {
                return found;
            }
        }
        return null;
    }

    /// <summary>
    /// CanvasGroup 컴포넌트 초기화
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
    /// 카드 변화 모니터링
    /// </summary>
    private System.Collections.IEnumerator MonitorCardChanges()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            // GameManager가 준비되었는지 확인
            if (GameManager.Instance != null && GameManager.Instance.UnityGame != null && GameManager.Instance.UnityPlayer != null)
            {
                try
                {
                    var status = GameEvents.OnCardStatusRequested?.Invoke();
                    if (status.HasValue)
                    {
                        var (cardIndices, hpChanges, curseChanges, descriptions, rerollCount) = status.Value;

                        // 데이터 유효성 검사
                        if (cardIndices != null && hpChanges != null && curseChanges != null && descriptions != null)
                        {
                            UpdateCardPreviews(cardIndices, hpChanges, curseChanges, descriptions);
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"[CardEffectPreview] 카드 상태 업데이트 중 오류: {e.Message}");
                }
            }
        }
    }

    /// <summary>
    /// 카드 효과 미리보기 업데이트
    /// </summary>
    public void UpdateCardPreviews(List<int> cardIndices, List<int> hpChanges, List<int> curseChanges, List<string> descriptions)
    {
        for (int i = 0; i < effectPanels.Length; i++)
        {
            if (i < cardIndices.Count && cardIndices[i] >= 0 && cardIndices[i] < CardLibrary.AllCards.Count)
            {
                // 카드가 있는 경우
                var card = CardLibrary.AllCards[cardIndices[i]];
                ShowCardEffect(i, card, hpChanges[i], curseChanges[i], descriptions[i]);
            }
            else
            {
                // 카드가 없는 경우
                HideCardEffect(i);
            }
        }
    }

    /// <summary>
    /// 특정 슬롯의 카드 효과 표시
    /// </summary>
    private void ShowCardEffect(int slotIndex, Card card, int hpChange, int curseChange, string description)
    {
        if (slotIndex >= effectPanels.Length || effectPanels[slotIndex] == null) return;

        // 패널 활성화
        effectPanels[slotIndex].SetActive(true);

        // 효과 텍스트 설정
        if (effectTexts[slotIndex] != null)
        {
            string effectDescription = BuildEffectDescription(card, hpChange, curseChange);
            effectTexts[slotIndex].text = effectDescription;

            // 색상 설정 (전체 효과에 따라)
            Color textColor = GetEffectColor(hpChange, curseChange, card.Special != null);
            effectTexts[slotIndex].color = textColor;
        }

        // 페이드 인 애니메이션
        if (useFadeAnimation && panelCanvasGroups[slotIndex] != null)
        {
            StartCoroutine(FadeInPanel(slotIndex));
        }
        else if (panelCanvasGroups[slotIndex] != null)
        {
            panelCanvasGroups[slotIndex].alpha = 1f;
        }
    }

    /// <summary>
    /// 효과 설명 텍스트 생성
    /// </summary>
    private string BuildEffectDescription(Card card, int hpChange, int curseChange)
    {
        List<string> effects = new List<string>();

        // HP 변화
        if (hpChange > 0)
            effects.Add($"<color=#00FF00>체력 +{hpChange}</color>");
        else if (hpChange < 0)
            effects.Add($"<color=#FF0000>체력 {hpChange}</color>");

        // 저주 변화
        if (curseChange > 0)
            effects.Add($"<color=#FF0000>저주 +{curseChange}</color>");
        else if (curseChange < 0)
            effects.Add($"<color=#00FF00>저주 {curseChange}</color>");

        // 특수 효과
        string specialEffect = GetSpecialEffectDescription(card);
        if (!string.IsNullOrEmpty(specialEffect))
        {
            effects.Add($"<color=#FFFF00>{specialEffect}</color>");
        }

        // 효과가 없는 경우
        if (effects.Count == 0)
        {
            return "<color=#888888>효과 없음</color>";
        }

        return string.Join("\n", effects);
    }

    /// <summary>
    /// 특수 효과 설명 가져오기 (한글 버전)
    /// </summary>
    private string GetSpecialEffectDescription(Card card)
    {
        switch (card.Name)
        {
            case "바보": return "체력을 10으로 초기화";
            case "죽음": return "☠️ 사망";
            case "탑": return "다음 턴 스킵";
            case "연인": return "리롤 기회 +1";
            case "부활": return "덱에서 죽음 카드 제거";
            case "생명": return "덱에 카드 20장 추가";
            case "운명의 수레바퀴": return "체력과 저주 교환";
            case "매달린 남자": return "다음 턴 2장만 뽑음";
            case "심판": return "덱에서 5장 제거";
            case "절제": return "3턴간 저주 피해 면역";
            case "광대": return "체력 무작위 증가 (1-10)";
            case "교황": return "3턴간 저주 감소 불가";
            case "은둔자": return "5턴 후 체력 +7";
            case "마법사": return "3턴 후 저주 -3";
            case "여교황": return "3턴간 체력 증가 불가";
            case "여제": return "덱에서 죽음 카드 5장 제거";
            case "황제": return "체력 & 저주 두 배";
            case "전차": return "다음 턴 2장 선택";
            case "정의": return "체력 & 저주 재분배";
            case "세계": return "모든 카드 효과 실행";
            case "거울": return "마지막 카드 효과 재발동";
            case "일식": return "2턴 후 저주 +2";
            case "암거래": return "5턴간 죽음 카드 추가 금지";
            case "불씨": return "체력 1 이하 시 생존";
            case "저주받은 책": return "";
            case "예언자": return "다음 턴 페널티 무효";
            case "종말의 경전": return "덱 리셋";
            case "강탈자": return "";
            case "대천사": return "죽음 카드 교체";
            case "영혼의 초": return "3턴 후 저주 -2";
            case "그림자의 균열": return "";
            case "영혼 결혼식": return "리롤 기회 +1";
            case "피의 서약": return "2턴간 저주 증가 무효";
            case "운명의 유희": return "다음 턴 무작위 선택";
            case "꿈": return "";
            case "힘": return "";
            case "악마": return "";
            case "별": return "";
            case "달": return "";
            case "태양": return "";
            case "연기": return "";
            default: return string.IsNullOrEmpty(card.Description) ? "" : card.Description;
        }
    }

    /// <summary>
    /// 효과에 따른 색상 결정
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
    /// 특정 슬롯의 카드 효과 숨김
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
    /// 외부에서 호출 가능한 새로고침
    /// </summary>
    public void RefreshPreviews()
    {
        if (GameManager.Instance != null)
        {
            var status = GameEvents.OnCardStatusRequested?.Invoke();
            if (status.HasValue)
            {
                var (cardIndices, hpChanges, curseChanges, descriptions, rerollCount) = status.Value;
                UpdateCardPreviews(cardIndices, hpChanges, curseChanges, descriptions);
            }
        }
    }

    /// <summary>
    /// 테스트용 메서드
    /// </summary>
    [ContextMenu("Test Show Effects")]
    private void TestShowEffects()
    {
        // 테스트용 카드들로 효과 표시
        for (int i = 0; i < 3 && i < CardLibrary.AllCards.Count; i++)
        {
            var card = CardLibrary.AllCards[i];
            ShowCardEffect(i, card, card.HpChange, card.CurseChange, card.Description);
        }
    }

    [ContextMenu("Re-scan Card Structure")]
    private void RescanCardStructure()
    {
        if (autoFindPanels)
        {
            AutoFindCardEffectPanels();
            InitializeCanvasGroups();
            Debug.Log("[CardEffectPreview] 카드 구조 재스캔 완료!");
        }
    }
}