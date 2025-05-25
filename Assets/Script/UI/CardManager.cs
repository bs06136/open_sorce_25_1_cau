using UnityEngine;
using UnityEngine.UI;
using CardGame;
using System.Collections.Generic;
using System.Linq;

public class CardManager : MonoBehaviour
{
    [Header("Setup Instructions")]
    [TextArea(4, 6)]
    public string setupInstructions = "1. PNG 파일들을 Project에 Import하세요\n2. 각 PNG를 선택하고 Inspector에서 Texture Type을 'Sprite (2D and UI)'로 변경\n3. Apply 버튼 클릭\n4. 변환된 Sprite들을 아래 배열에 드래그";

    [Header("Card Sprites (Total: 41 cards)")]
    [Tooltip("PNG를 Sprite로 변환한 후 CardLibrary.AllCards와 같은 순서로 배열해주세요")]
    public Sprite[] cardSprites = new Sprite[41];

    [Header("Default Sprites")]
    public Sprite defaultCardSprite;
    public Sprite emptySlotSprite;

    [Header("UI References")]
    public Image[] cardImageSlots;
    public Button[] cardButtons;
    public Text[] cardNameTexts; // 카드 이름 표시용 (선택사항)

    // 카드 목록 (CardLibrary.AllCards와 동일한 순서)
    private readonly string[] cardNames = new string[]
    {
        // 인덱스 0-34: 주요 카드들
        "바보",           // 0
        "죽음",           // 1  
        "탑",             // 2
        "연인",           // 3
        "부활",           // 4
        "생명",           // 5
        "운명의 수레바퀴", // 6
        "매달린 남자",     // 7
        "심판",           // 8
        "절제",           // 9
        "광대",           // 10
        "교황",           // 11
        "은둔자",         // 12
        "마법사",         // 13
        "여교황",         // 14
        "여제",           // 15
        "황제",           // 16
        "전차",           // 17
        "정의",           // 18
        "세계",           // 19
        "거울",           // 20
        "일식",           // 21
        "암거래",         // 22
        "불씨",           // 23
        "저주받은 책",     // 24
        "예언자",         // 25
        "종말의 경전",     // 26
        "강탈자",         // 27
        "대천사",         // 28
        "영혼의 초",       // 29
        "그림자의 균열",   // 30
        "영혼 결혼식",     // 31
        "피의 서약",       // 32
        "운명의 유희",     // 33
        "꿈",             // 34
        
        // 인덱스 35-40: 기본 카드들
        "힘",             // 35
        "악마",           // 36
        "별",             // 37
        "달",             // 38
        "태양",           // 39
        "연기"            // 40
    };

    private void Start()
    {
        CheckSetup();
    }

    private void CheckSetup()
    {
        int expectedCount = CardLibrary.AllCards.Count;
        int actualCount = cardSprites.Length;

        Debug.Log($"[CardManager] 설정 확인:");
        Debug.Log($"  - 전체 카드 수: {expectedCount}");
        Debug.Log($"  - 스프라이트 배열 크기: {actualCount}");
        Debug.Log($"  - 카드 이름 배열 크기: {cardNames.Length}");

        if (actualCount != expectedCount)
        {
            Debug.LogWarning($"[CardManager] 스프라이트 배열 크기를 {expectedCount}로 조정해주세요!");
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
                    Debug.LogError($"[CardManager] 인덱스 {i}에서 카드 이름 불일치! Library: '{libraryName}', Array: '{arrayName}'");
                }
            }
        }
    }

    public void DisplayCards(List<Card> cards)
    {
        Debug.Log($"[CardManager] DisplayCards 호출됨 - {cards.Count}장의 카드");

        // 카드 정보 로그 출력
        for (int i = 0; i < cards.Count; i++)
        {
            int cardIndex = GetCardIndex(cards[i]);
            Debug.Log($"  슬롯 {i}: {cards[i].Name} (인덱스: {cardIndex}, HP {cards[i].HpChange}, Curse {cards[i].CurseChange})");
        }

        // UI 슬롯에 카드 표시
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (i < cards.Count)
            {
                SetCardSlot(i, cards[i]);
                SetCardButtonActive(i, true);
            }
            else
            {
                SetEmptySlot(i);
                SetCardButtonActive(i, false);
            }
        }
    }

    private void SetCardSlot(int slotIndex, Card card)
    {
        if (slotIndex >= cardImageSlots.Length) return;

        var imageSlot = cardImageSlots[slotIndex];
        int cardIndex = GetCardIndex(card);

        // 카드 이미지 설정
        if (cardIndex >= 0 && cardIndex < cardSprites.Length && cardSprites[cardIndex] != null)
        {
            imageSlot.sprite = cardSprites[cardIndex];
            imageSlot.color = Color.white;
            Debug.Log($"[CardManager] 슬롯 {slotIndex}에 '{card.Name}' 이미지 설정 완료");
        }
        else
        {
            SetDefaultCardImage(imageSlot, card, cardIndex);
        }

        // 카드 이름 텍스트 설정 (있는 경우)
        if (slotIndex < cardNameTexts.Length && cardNameTexts[slotIndex] != null)
        {
            cardNameTexts[slotIndex].text = card.Name;
        }
    }

    private int GetCardIndex(Card card)
    {
        // CardLibrary에서 카드 인덱스 찾기
        for (int i = 0; i < CardLibrary.AllCards.Count; i++)
        {
            if (CardLibrary.AllCards[i].Name == card.Name)
            {
                return i;
            }
        }
        return -1; // 찾지 못한 경우
    }

    private void SetDefaultCardImage(Image imageSlot, Card card, int cardIndex)
    {
        Debug.LogWarning($"[CardManager] '{card.Name}' 스프라이트 없음 (인덱스: {cardIndex})");

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

    private Color GetCardTypeColor(Card card)
    {
        // 카드 종류별 색상 구분
        switch (card.Name)
        {
            case "죽음":
                return Color.black;
            case "바보":
                return new Color(1f, 0.5f, 0f); // 주황색
            case "전차":
            case "매달린 남자":
                return Color.cyan; // 특수 효과 카드
            case "세계":
                return Color.magenta; // 최종 카드
            default:
                if (card.HpChange > 0)
                    return Color.green; // 체력 증가
                else if (card.HpChange < 0)
                    return Color.red; // 체력 감소
                else if (card.CurseChange > 0)
                    return new Color(0.5f, 0f, 0.5f); // 보라색 (저주 증가)
                else if (card.CurseChange < 0)
                    return Color.blue; // 저주 감소
                else
                    return Color.yellow; // 특수 효과
        }
    }

    private void SetEmptySlot(int slotIndex)
    {
        if (slotIndex >= cardImageSlots.Length) return;

        var imageSlot = cardImageSlots[slotIndex];

        if (emptySlotSprite != null)
        {
            imageSlot.sprite = emptySlotSprite;
            imageSlot.color = new Color(1, 1, 1, 0.3f);
        }
        else
        {
            imageSlot.sprite = null;
            imageSlot.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);
        }

        // 빈 슬롯 텍스트 처리
        if (slotIndex < cardNameTexts.Length && cardNameTexts[slotIndex] != null)
        {
            cardNameTexts[slotIndex].text = "";
        }
    }

    private void SetCardButtonActive(int slotIndex, bool active)
    {
        if (slotIndex < cardButtons.Length && cardButtons[slotIndex] != null)
        {
            cardButtons[slotIndex].interactable = active;
        }
    }

    // Inspector 테스트 메서드들
    [ContextMenu("Check Sprite Setup")]
    public void CheckSpriteSetup()
    {
        Debug.Log("[CardManager] 스프라이트 설정 검사:");

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

        Debug.Log($"[CardManager] 결과: {validSprites}/{totalSprites} 스프라이트 설정됨");

        if (validSprites == 0)
        {
            Debug.LogError("[CardManager] 스프라이트가 하나도 설정되지 않았습니다!");
            Debug.LogError("PNG 파일들을 Sprite로 변환했는지 확인하세요:");
            Debug.LogError("1. PNG 파일 선택 → Inspector → Texture Type: Sprite (2D and UI) → Apply");
        }
    }

    [ContextMenu("Check UI Setup")]
    public void CheckUISetup()
    {
        Debug.Log("[CardManager] UI 설정 검사:");

        // Card Image Slots 확인
        Debug.Log($"Card Image Slots: {cardImageSlots.Length}개");
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            if (cardImageSlots[i] != null)
            {
                Debug.Log($"  ✓ 슬롯 {i}: {cardImageSlots[i].name} (Image 컴포넌트)");
            }
            else
            {
                Debug.LogError($"  ✗ 슬롯 {i}: NULL - Image 컴포넌트를 연결하세요");
            }
        }

        // Card Buttons 확인
        Debug.Log($"Card Buttons: {cardButtons.Length}개");
        for (int i = 0; i < cardButtons.Length; i++)
        {
            if (cardButtons[i] != null)
            {
                var handler = cardButtons[i].GetComponent<CardButtonHandler>();
                if (handler != null)
                {
                    Debug.Log($"  ✓ 버튼 {i}: {cardButtons[i].name} (CardButtonHandler: {handler.cardIndex})");
                }
                else
                {
                    Debug.LogWarning($"  △ 버튼 {i}: {cardButtons[i].name} (CardButtonHandler 없음)");
                }
            }
            else
            {
                Debug.LogError($"  ✗ 버튼 {i}: NULL - Button 컴포넌트를 연결하세요");
            }
        }
    }

    [ContextMenu("Test Display Random Cards")]
    public void TestDisplayRandomCards()
    {
        List<Card> testCards = new List<Card>();

        // 랜덤하게 3장 선택
        for (int i = 0; i < 3; i++)
        {
            int randomIndex = Random.Range(0, CardLibrary.AllCards.Count);
            testCards.Add(CardLibrary.AllCards[randomIndex]);
        }

        Debug.Log("[CardManager] 테스트 카드 표시");
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

        Debug.Log("[CardManager] 특수 카드 테스트 표시");
        DisplayCards(testCards);
    }

    // 외부에서 특정 카드 스프라이트 설정 가능
    public void SetCardSprite(string cardName, Sprite sprite)
    {
        int index = GetCardIndexByName(cardName);
        if (index >= 0 && index < cardSprites.Length)
        {
            cardSprites[index] = sprite;
            Debug.Log($"[CardManager] '{cardName}' 스프라이트 업데이트됨 (인덱스: {index})");
        }
        else
        {
            Debug.LogError($"[CardManager] '{cardName}' 카드를 찾을 수 없습니다");
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
}