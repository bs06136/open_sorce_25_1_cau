using UnityEngine;
using System.Collections.Generic;
using CardGame;

public class CardButtonHandler : MonoBehaviour
{
    public int slotIndex; // 인스펙터에서 설정: 0(Left), 1(Middle), 2(Right)

    public void OnCardButtonClicked()
    {
        Debug.Log($"[CardButtonHandler] 슬롯 {slotIndex} 클릭됨");
        Debug.Log($"[CardButtonHandler] GameManager 처리 상태: {GameManager.Instance?.IsCardProcessing() ?? false}");
        Debug.Log($"[CardButtonHandler] 전차 상태: active={GameManager.Instance?.IsChariotActive() ?? false}, firstPick={GameManager.Instance?.IsChariotFirstPick() ?? false}");

        // 🔥 GameManager가 처리 중이면 클릭 무시
        if (GameManager.Instance?.IsCardProcessing() == true)
        {
            Debug.Log($"[CardButtonHandler] GameManager가 처리 중이므로 클릭 무시");
            return;
        }

        // 🔥 전차 효과 중인지 확인하고 다른 매핑 적용
        int actualCardIndex = IsChariotSecondPick() ?
            GetChariotActualCardIndex(slotIndex) :
            GetActualCardIndex(slotIndex);

        if (actualCardIndex >= 0)
        {
            Debug.Log($"[CardButtonHandler] 슬롯 {slotIndex} 클릭 → 카드 인덱스 {actualCardIndex}");

            // 🔥 카드 적용 전에 스토리 표시 (애니메이션에서 중복 표시 방지)
            ShowCardStoryBeforeSelection(actualCardIndex);

            // 🔥 약간의 딜레이 후 카드 선택 이벤트 발동 (스토리가 먼저 표시되도록)
            StartCoroutine(DelayedCardSelection(actualCardIndex));
        }
        else
        {
            Debug.LogWarning($"[CardButtonHandler] 슬롯 {slotIndex} 클릭했지만 유효하지 않은 카드 인덱스");
        }
    }

    /// <summary>
    /// 전차 효과의 두 번째 선택인지 확인 (리플렉션 대신 public 메서드 사용)
    /// </summary>
    private bool IsChariotSecondPick()
    {
        if (GameManager.Instance == null) return false;

        // 🔥 리플렉션 대신 GameManager의 public 메서드 사용
        bool result = GameManager.Instance.IsChariotSecondPick();
        Debug.Log($"[CardButtonHandler] 전차 두 번째 선택 여부: {result}");
        return result;
    }

    /// <summary>
    /// 전차 효과 두 번째 선택 시 슬롯 인덱스를 카드 인덱스로 변환 (원래 위치 기반)
    /// </summary>
    private int GetChariotActualCardIndex(int buttonSlotIndex)
    {
        var currentCards = GetCurrentDrawnCards();
        if (currentCards == null || currentCards.Count != 2)
        {
            Debug.LogError($"[CardButtonHandler] 전차 효과 오류 - currentCards가 null이거나 2장이 아님: {currentCards?.Count ?? 0}");
            return -1;
        }

        Debug.Log($"[CardButtonHandler] 전차 효과 - 슬롯 {buttonSlotIndex}, 카드 수: {currentCards.Count}");

        // 🔥 활성화된 슬롯들 가져오기 (원래 위치 그대로)
        var activeSlots = GetActiveChariotSlots();
        Debug.Log($"[CardButtonHandler] 활성 슬롯: [{string.Join(", ", activeSlots)}]");

        // 🔥 클릭된 슬롯이 활성 슬롯 중 몇 번째인지 찾기
        int cardIndex = -1;
        for (int i = 0; i < activeSlots.Length; i++)
        {
            if (activeSlots[i] == buttonSlotIndex)
            {
                cardIndex = i;
                Debug.Log($"[CardButtonHandler] 슬롯 {buttonSlotIndex}는 활성 슬롯 중 {i}번째 → 카드 인덱스 {cardIndex}");
                break;
            }
        }

        if (cardIndex >= 0 && cardIndex < currentCards.Count)
        {
            Debug.Log($"[CardButtonHandler] 최종 매핑: 슬롯 {buttonSlotIndex} → 카드 인덱스 {cardIndex} ('{currentCards[cardIndex].Name}')");
            return cardIndex;
        }
        else
        {
            Debug.LogError($"[CardButtonHandler] 전차 효과에서 잘못된 슬롯: {buttonSlotIndex} (활성 슬롯: [{string.Join(", ", activeSlots)}])");
            return -1;
        }
    }

    /// <summary>
    /// 현재 활성화된 전차 슬롯들 가져오기 (실제 상태 기반)
    /// </summary>
    private int[] GetActiveChariotSlots()
    {
        List<int> activeSlots = new List<int>();

        if (UnifiedCardManager.Instance != null)
        {
            // 실제로 카드가 앞면으로 표시되고 버튼이 활성화된 슬롯들 확인
            for (int i = 0; i < UnifiedCardManager.Instance.isCardFront.Length; i++)
            {
                bool isCardFront = UnifiedCardManager.Instance.isCardFront[i];
                bool isButtonActive = i < UnifiedCardManager.Instance.cardButtons.Length &&
                                    UnifiedCardManager.Instance.cardButtons[i] != null &&
                                    UnifiedCardManager.Instance.cardButtons[i].interactable;

                Debug.Log($"[CardButtonHandler] 슬롯 {i}: 앞면={isCardFront}, 버튼활성={isButtonActive}");

                if (isCardFront && isButtonActive)
                {
                    activeSlots.Add(i);
                }
            }
        }

        // 슬롯 번호 순으로 정렬 (0, 1, 2 순서)
        activeSlots.Sort();

        var result = activeSlots.ToArray();
        Debug.Log($"[CardButtonHandler] 감지된 활성 슬롯 (정렬됨): [{string.Join(", ", result)}]");
        return result;
    }

    /// <summary>
    /// 딜레이된 카드 선택 (스토리 표시 후 진행)
    /// </summary>
    private System.Collections.IEnumerator DelayedCardSelection(int cardIndex)
    {
        // 스토리가 표시될 시간을 줌
        yield return new WaitForSeconds(0.2f);

        // 카드 선택 이벤트 발동 (이후 GameManager에서 강조 애니메이션과 카드 적용)
        Debug.Log($"[CardButtonHandler] GameEvents.OnCardChosen 호출: 카드 인덱스 {cardIndex}");
        GameEvents.OnCardChosen?.Invoke(cardIndex);
    }

    /// <summary>
    /// 카드 선택 전에 미리 스토리 표시 (현재 턴의 카드 정보 사용)
    /// </summary>
    private void ShowCardStoryBeforeSelection(int cardIndex)
    {
        // 현재 턴의 뽑힌 카드들 가져오기 (아직 제거되지 않은 상태)
        var currentCards = GetCurrentDrawnCards();
        if (currentCards != null && cardIndex >= 0 && cardIndex < currentCards.Count)
        {
            var selectedCard = currentCards[cardIndex];

            Debug.Log($"[CardButtonHandler] 선택된 카드 (적용 전): 인덱스 {cardIndex}, 이름 '{selectedCard.Name}'");

            // CardStoryDisplay가 있으면 스토리 표시
            if (CardStoryDisplay.Instance != null)
            {
                // 🔥 전차 효과 두 번째 선택인 경우 표시 이름 변경
                if (IsChariotSecondPick())
                {
                    string displayName = $"[전차] {selectedCard.Name}";
                    CardStoryDisplay.Instance.ShowCardStoryWithCustomName(selectedCard.Name, displayName);
                    Debug.Log($"[CardButtonHandler] 전차 효과 '{selectedCard.Name}' 카드 스토리 표시");
                }
                else
                {
                    CardStoryDisplay.Instance.ShowCardStory(selectedCard.Name);
                    Debug.Log($"[CardButtonHandler] '{selectedCard.Name}' 카드 스토리 표시 (선택 전)");
                }
            }
            else
            {
                Debug.LogWarning("[CardButtonHandler] CardStoryDisplay.Instance가 null입니다.");
            }
        }
        else
        {
            Debug.LogError($"[CardButtonHandler] 잘못된 카드 인덱스: {cardIndex}, 전체 카드 수: {currentCards?.Count ?? 0}");
        }
    }

    /// <summary>
    /// 현재 뽑힌 카드들 가져오기 (리플렉션 대신 GameManager의 public 필드/메서드 사용 권장)
    /// </summary>
    private List<Card> GetCurrentDrawnCards()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CardButtonHandler] GameManager.Instance가 null입니다.");
            return null;
        }

        // 🔥 리플렉션 사용 (GameManager에 public getter 추가를 권장)
        var cards = GameManager.Instance.GetType()
            .GetField("currentDrawnCards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.GetValue(GameManager.Instance) as List<Card>;

        if (cards == null)
        {
            Debug.LogError("[CardButtonHandler] currentDrawnCards를 가져올 수 없습니다.");
        }
        else
        {
            Debug.Log($"[CardButtonHandler] 현재 뽑힌 카드들: {string.Join(", ", cards.ConvertAll(c => c.Name))}");
        }

        return cards;
    }

    /// <summary>
    /// 슬롯 인덱스를 실제 카드 인덱스로 변환 (일반 상황) - 디버깅 강화
    /// </summary>
    private int GetActualCardIndex(int buttonSlotIndex)
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CardButtonHandler] GameManager.Instance가 null");
            return -1;
        }

        // 현재 뽑힌 카드들 가져오기
        var currentCards = GetCurrentDrawnCards();
        if (currentCards == null)
        {
            Debug.LogError("[CardButtonHandler] currentCards가 null");
            return -1;
        }

        int cardCount = currentCards.Count;

        Debug.Log($"[CardButtonHandler] 일반 상황 - 카드 수: {cardCount}, 클릭된 슬롯: {buttonSlotIndex}");

        // 카드 수에 따른 슬롯 매핑
        int result = -1;
        switch (cardCount)
        {
            case 1:
                // 가운데 슬롯(1번)만 유효
                result = buttonSlotIndex == 1 ? 0 : -1;
                break;
            case 2:
                // Left(0번) → 첫 번째 카드(0), Right(2번) → 두 번째 카드(1)
                if (buttonSlotIndex == 0) result = 0;
                else if (buttonSlotIndex == 2) result = 1;
                else result = -1;
                break;
            case 3:
            default:
                // 슬롯 인덱스 = 카드 인덱스
                result = buttonSlotIndex < cardCount ? buttonSlotIndex : -1;
                break;
        }

        Debug.Log($"[CardButtonHandler] 일반 매핑: 슬롯 {buttonSlotIndex} → 카드 인덱스 {result}");
        return result;
    }
}