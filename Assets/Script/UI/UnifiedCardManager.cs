using CardGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnifiedCardManager : MonoBehaviour
{
    public static UnifiedCardManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 혹시 중복 생기면 제거
        }

    }

    [Header("=== 카드 스프라이트 설정 ===")]
    [TextArea(4, 6)]
    public string setupInstructions = "1. PNG 파일들을 Project에 Import하세요\n2. 각 PNG를 선택하고 Inspector에서 Texture Type을 'Sprite (2D and UI)'로 변경\n3. Apply 버튼 클릭\n4. 변환된 Sprite들을 아래 배열에 드래그";

    [Tooltip("PNG를 Sprite로 변환한 후 CardLibrary.AllCards와 같은 순서로 배열해주세요")]
    public Sprite[] cardSprites = new Sprite[41];

    [Header("=== 기본 스프라이트 ===")]
    public Sprite defaultCardSprite;
    public Sprite emptySlotSprite;

    [Header("=== 게임 UI 참조 ===")]
    public Image[] cardImageSlots;
    public Button[] cardButtons;
    public Text[] cardNameTexts;

    [Header("=== 카드 효과 표시 ===")]
    [Tooltip("카드 효과를 표시할 패널들")]
    public GameObject[] effectPanels = new GameObject[3];
    [Tooltip("카드 효과 텍스트들")]
    public TextMeshProUGUI[] effectTexts = new TextMeshProUGUI[3];

    [Header("=== 리롤 시스템 (선택사항) ===")]
    public TextMeshProUGUI rerollText;
    public Button rerollButton;

    [Header("=== 카드 뒷면 스프라이트 ===")]
    public Sprite cardBackSprite;

    // 카드 이름 배열 (CardLibrary와 동일한 순서 - 검증용)
    private readonly string[] cardNames = new string[]
    {
        "바보", "마법사", "여교황", "여제", "황제", "교황", "연인", "전차",
        "힘", "은둔자", "운명의 수레바퀴", "정의", "매달린 남자", "죽음", "절제",
        "악마", "탑", "별", "달", "태양", "심판", "세계", "부활",
        "생명", "악동", "거울", "연기", "일식", "암거래", "불씨",
        "저주받은 책", "예언자", "종말의 경전", "강탈자", "대천사", "영혼의 초",
        "그림자의 균열", "영혼 결혼식", "피의 서약", "운명의 유희", "꿈"
    };

    // 카드가 앞면인지 여부를 관리
    public bool[] isCardFront = new bool[3];

    private void Start()
    {
        CheckSetup();

        // 리롤 버튼 이벤트 연결 (있는 경우에만)
        if (rerollButton != null)
            Debug.Log("[UnifiedCardManager] 리롤 버튼이 설정됨");
        // rerollButton.onClick.AddListener(OnRerollClicked);

        UpdateRerollUI();
    }

    #region 설정 검증

    private void CheckSetup()
    {
        int expectedCount = CardLibrary.AllCards.Count;
        int actualCount = cardSprites.Length;

        Debug.Log($"[UnifiedCardManager] 설정 확인:");
        Debug.Log($"  - 전체 카드 수: {expectedCount}");
        Debug.Log($"  - 스프라이트 배열 크기: {actualCount}");

        if (actualCount != expectedCount)
        {
            Debug.LogWarning($"[UnifiedCardManager] 스프라이트 배열 크기를 {expectedCount}로 조정해주세요!");
        }

        // 카드 순서 검증
        for (int i = 0; i < Mathf.Min(expectedCount, cardNames.Length); i++)
        {
            if (i < CardLibrary.AllCards.Count)
            {
                string libraryName = CardLibrary.AllCards[i].Name;
                string arrayName = cardNames[i];

                if (libraryName != arrayName)
                {
                    Debug.LogError($"[UnifiedCardManager] 인덱스 {i}에서 카드 이름 불일치! Library: '{libraryName}', Array: '{arrayName}'");
                }
            }
        }
    }

    #endregion

    #region 핵심 카드 표시 시스템

    /// <summary>
    /// 게임에서 뽑힌 카드들을 UI에 표시하는 메인 메서드
    /// </summary>
    public void DisplayCards(List<Card> cards)
    {
        Debug.Log($"[UnifiedCardManager] DisplayCards 호출됨 - {cards.Count}장의 카드");

        CardEffectManager.Instance?.HideAllEffects(); // 효과 초기화


        // 모든 슬롯 업데이트 (이미지 + 효과)
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (i < cards.Count)
            {
                // 카드 뒷면으로 초기화
                if (cardBackSprite != null)
                    cardImageSlots[i].sprite = cardBackSprite;
                isCardFront[i] = false;

                SetCardButtonActive(i, true);
                HideCardEffect(i); // 효과는 뒤집히기 전까지 숨김
            }
            else
            {
                SetEmptySlot(i);
                SetCardButtonActive(i, false);
                HideCardEffect(i); // 🔥 효과도 함께 숨김
            }
        }
        AnimateDrawCards(cards); // cards를 넘겨줌
    }

    // 드로우 애니메이션용
    private void AnimateDrawCards(List<Card> cards)
    {
        StartCoroutine(AnimateDraw(cards));
    }

    private IEnumerator AnimateDraw(List<Card> cards)
    {
        float moveDuration = 0.5f; // 올라오는 시간
        float delayBetweenCards = 0.2f; // 카드마다 딜레이
        float flipDelay = 0.5f; // 다 올라오고 뒤집기까지 대기 시간
        float flipDuration = 0.3f; // 뒤집는 시간

        // 카드 슬롯 전부 애니메이션
        for (int i = 0; i < cardImageSlots.Length; i++)
        {

            if (i >= 3) break; // 3장까지만

            RectTransform rt = cardImageSlots[i].GetComponent<RectTransform>();
            if (rt == null) continue;
            Vector2 originalPos = rt.anchoredPosition;
            Vector2 startPos = originalPos + new Vector2(0, -Screen.height);
            rt.anchoredPosition = startPos;

            // 슬라이드 업
            LeanTween.move(rt, originalPos, moveDuration).setEaseOutCubic();
            yield return new WaitForSeconds(delayBetweenCards);
        }

        // 전부 올라온 후 약간 대기
        yield return new WaitForSeconds(flipDelay);

        // 3장 동시에 뒤집기(1단계: 0→90도)
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (i >= 3) break; // 3장까지만

            RectTransform rt = cardImageSlots[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            // Y축 회전 90도
            LeanTween.rotateY(rt.gameObject, 90f, flipDuration / 2).setEaseInOutSine();
        }

        yield return new WaitForSeconds(flipDuration / 2);

        // 90도에서 앞면 이미지로 교체 + 효과 표시
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (i >= 3) break;
            if (i < cards.Count)
            {
                SetCardSlot(i, cards[i]);
                isCardFront[i] = true;
                // ShowCardEffect(i, cards[i]); // 이 시점에만 효과 표시
            }
        }

        // 2단계: 90→0도
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (i >= 3) break;
            RectTransform rt = cardImageSlots[i].GetComponent<RectTransform>();
            if (rt == null) continue;

            // 0도로 회전
            LeanTween.rotateY(rt.gameObject, 0f, flipDuration / 2).setEaseInOutSine();
        }
    }

    private void SetCardSlot(int slotIndex, Card card)
    {
        if (slotIndex >= cardImageSlots.Length) return;

        var imageSlot = cardImageSlots[slotIndex];
        int cardIndex = GetCardIndex(card);

        Debug.Log($"[UnifiedCardManager] 슬롯 {slotIndex}에 '{card.Name}' 카드 표시 (인덱스: {cardIndex})");

        // 카드 이미지 설정
        if (cardIndex >= 0 && cardIndex < cardSprites.Length && cardSprites[cardIndex] != null)
        {
            imageSlot.sprite = cardSprites[cardIndex];
            imageSlot.color = Color.white;
        }
        else
        {
            SetDefaultCardImage(imageSlot, card);
        }

        // 카드 이름 텍스트 설정
        if (slotIndex < cardNameTexts.Length && cardNameTexts[slotIndex] != null)
        {
            cardNameTexts[slotIndex].text = card.Name;
        }
    }

    private void SetDefaultCardImage(Image imageSlot, Card card)
    {
        Debug.LogWarning($"[UnifiedCardManager] '{card.Name}' 스프라이트 없음");

        if (defaultCardSprite != null)
        {
            imageSlot.sprite = defaultCardSprite;
            imageSlot.color = GetCardTypeColor(card);
        }
        else
        {
            imageSlot.sprite = null;
            imageSlot.color = GetCardTypeColor(card);
        }
    }

    public void SetEmptySlot(int slotIndex)
    {
        if (slotIndex >= cardImageSlots.Length) return;

        var imageSlot = cardImageSlots[slotIndex];

        if (emptySlotSprite != null)
        {
            imageSlot.sprite = emptySlotSprite;
            imageSlot.color = new Color(1, 1, 1, 0.0f);
        }
        else
        {
            imageSlot.sprite = null;
            imageSlot.color = new Color(0.5f, 0.5f, 0.5f, 0.0f);
        }

        if (slotIndex < cardNameTexts.Length && cardNameTexts[slotIndex] != null)
        {
            cardNameTexts[slotIndex].text = "";
        }
    }

    public void SetAllEmptySlots()
    {
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            SetEmptySlot(i);
            isCardFront[i] = false; // 모든 슬롯을 뒷면으로 초기화
        }
    }

    public void SetCardButtonActive(int slotIndex, bool active)
    {
        if (slotIndex < cardButtons.Length && cardButtons[slotIndex] != null)
        {
            cardButtons[slotIndex].interactable = active;
        }
    }

    #endregion

    #region 🔥 카드 효과 표시 시스템

    /// <summary>
    /// 카드 효과 표시
    /// </summary>
    private void ShowCardEffect(int slotIndex, Card card)
    {

        // 앞면이 아닐 때는 효과를 무조건 숨김
        if (slotIndex >= isCardFront.Length || !isCardFront[slotIndex])
        {
            HideCardEffect(slotIndex);
            return;
        }

        if (slotIndex >= effectPanels.Length || effectPanels[slotIndex] == null || card == null)
        {
            Debug.LogWarning($"[UnifiedCardManager] 효과 표시 실패 - 슬롯: {slotIndex}, 카드: {card?.Name}");
            return;
        }

        Debug.Log($"[UnifiedCardManager] 슬롯 {slotIndex}에 '{card.Name}' 효과 표시");

        // 패널 활성화
        effectPanels[slotIndex].SetActive(true);

        // 효과 텍스트 설정
        if (effectTexts[slotIndex] != null)
        {
            string effectDescription = BuildEffectDescription(card);
            effectTexts[slotIndex].text = effectDescription;

            Debug.Log($"[UnifiedCardManager] 효과 텍스트: '{effectDescription}'");
        }
        else
        {
            Debug.LogError($"[UnifiedCardManager] 슬롯 {slotIndex}의 effectTexts가 null!");
        }
    }

    /// <summary>
    /// 카드 효과 숨김
    /// </summary>
    private void HideCardEffect(int slotIndex)
    {

        if (slotIndex >= effectPanels.Length || effectPanels[slotIndex] == null) return;

        effectPanels[slotIndex].SetActive(false);
        Debug.Log($"[UnifiedCardManager] 슬롯 {slotIndex} 효과 숨김");
    }

    /// <summary>
    /// 효과 설명 텍스트 생성 (실제 카드 데이터 기반)
    /// </summary>
    private string BuildEffectDescription(Card card)
    {
        List<string> effects = new List<string>();

        // HP 변화
        if (card.HpChange > 0)
            effects.Add($"<color=#00FF00>체력 +{card.HpChange}</color>");
        else if (card.HpChange < 0)
            effects.Add($"<color=#FF0000>체력 {card.HpChange}</color>");

        // 저주 변화
        if (card.CurseChange > 0)
            effects.Add($"<color=#FF0000>저주 +{card.CurseChange}</color>");
        else if (card.CurseChange < 0)
            effects.Add($"<color=#00FF00>저주 {card.CurseChange}</color>");

        // 특수 효과 (카드 Description에서)
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

    #endregion

    #region 리롤 시스템 (GameManager와 연동)

    public void OnRerollClicked()
    {
        // GameManager를 통해 리롤 처리
        if (GameManager.Instance != null && GameManager.Instance.UnityPlayer != null)
        {
            if (GameManager.Instance.UnityPlayer.RerollAvailable > 0)
            {
                GameManager.Instance.UnityPlayer.RerollAvailable--;
                GameManager.Instance.StartTurn(); // 새로운 카드 뽑기
                UpdateRerollUI();
                Debug.Log("[UnifiedCardManager] 리롤 사용됨");
            }
            else
            {
                Debug.Log("[UnifiedCardManager] 리롤 기회 없음");
                if (rerollText != null)
                    rerollText.text = "0";
            }
        }
        else
        {
            // 테스트 모드
            TestDisplayRandomCards();
        }
    }

    public void UpdateRerollUI()
    {
        Debug.Log("[UnifiedCardManager] UpdateRerollUI 호출됨");
        if (rerollText != null && GameManager.Instance != null && GameManager.Instance.UnityPlayer != null)
        {
            int availableRerolls = GameManager.Instance.UnityPlayer.RerollAvailable;
            rerollText.text = $"{availableRerolls}";
            Debug.Log($"[UnifiedCardManager] 현재 리롤 가능 횟수: {availableRerolls}");
        }
    }

    #endregion

    #region 유틸리티 메서드

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

    public void SetCardSprite(string cardName, Sprite sprite)
    {
        int index = GetCardIndexByName(cardName);
        if (index >= 0 && index < cardSprites.Length)
        {
            cardSprites[index] = sprite;
            Debug.Log($"[UnifiedCardManager] '{cardName}' 스프라이트 업데이트됨");
        }
        else
        {
            Debug.LogError($"[UnifiedCardManager] '{cardName}' 카드를 찾을 수 없습니다");
        }
    }

    private int GetCardIndexByName(string cardName)
    {
        for (int i = 0; i < CardLibrary.AllCards.Count; i++)
        {
            if (CardLibrary.AllCards[i].Name == cardName)
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 외부에서 UnifiedCardData 생성 (필요시 사용)
    /// </summary>
    public UnifiedCardData CreateCardData(Card card)
    {
        int cardIndex = GetCardIndex(card);
        return new UnifiedCardData
        {
            cardName = card.Name,
            description = card.Description,
            cardImage = (cardIndex >= 0 && cardIndex < cardSprites.Length) ? cardSprites[cardIndex] : defaultCardSprite,
            hpChange = card.HpChange,
            curseChange = card.CurseChange
        };
    }

    #endregion

    #region 테스트 메서드들

    [ContextMenu("Check Sprite Setup")]
    public void CheckSpriteSetup()
    {
        Debug.Log("[UnifiedCardManager] 스프라이트 설정 검사:");

        int validSprites = 0;
        int totalSprites = cardSprites.Length;

        for (int i = 0; i < cardSprites.Length; i++)
        {
            if (cardSprites[i] != null)
            {
                validSprites++;
                Debug.Log($"  ✓ [{i}] {cardNames[i]} - {cardSprites[i].name}");
            }
            else
            {
                Debug.LogWarning($"  ✗ [{i}] {cardNames[i]} - 스프라이트 없음");
            }
        }

        Debug.Log($"[UnifiedCardManager] 결과: {validSprites}/{totalSprites} 스프라이트 설정됨");
    }

    [ContextMenu("Test Display Random Cards")]
    public void TestDisplayRandomCards()
    {
        List<Card> testCards = new List<Card>();

        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, CardLibrary.AllCards.Count);
            testCards.Add(CardLibrary.AllCards[randomIndex]);
        }

        Debug.Log("[UnifiedCardManager] 테스트 카드 표시");
        DisplayCards(testCards);
    }

    [ContextMenu("Test Display Special Cards")]
    public void TestDisplaySpecialCards()
    {
        List<Card> testCards = new List<Card>
        {
            CardLibrary.AllCards.Find(c => c.Name == "매달린 남자"),
            CardLibrary.AllCards.Find(c => c.Name == "전차"),
            CardLibrary.AllCards.Find(c => c.Name == "죽음")
        };

        Debug.Log("[UnifiedCardManager] 특수 카드 테스트 표시");
        DisplayCards(testCards);
    }

    #endregion
}

// 통합된 카드 데이터 구조 (간소화)
[System.Serializable]
public class UnifiedCardData
{
    public string cardName;
    public string description;
    public Sprite cardImage;
    public int hpChange;
    public int curseChange;

    // 기존 CardData와의 호환성을 위한 변환 메서드
    public CardData ToCardData()
    {
        return new CardData(cardName, description, cardImage);
    }
}

