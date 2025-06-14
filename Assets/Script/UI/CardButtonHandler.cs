using UnityEngine;
using System.Collections.Generic;
using CardGame;

public class CardButtonHandler : MonoBehaviour
{
    public int slotIndex; // 인스펙터에서 설정: 0(Left), 1(Middle), 2(Right)

    public void OnCardButtonClicked()
    {
        // 실제 카드 인덱스로 변환
        int actualCardIndex = GetActualCardIndex(slotIndex);
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
    /// 딜레이된 카드 선택 (스토리 표시 후 진행)
    /// </summary>
    private System.Collections.IEnumerator DelayedCardSelection(int cardIndex)
    {
        // 스토리가 표시될 시간을 줌
        yield return new WaitForSeconds(0.2f);

        // 카드 선택 이벤트 발동 (이후 GameManager에서 강조 애니메이션과 카드 적용)
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
                CardStoryDisplay.Instance.ShowCardStory(selectedCard.Name);
                Debug.Log($"[CardButtonHandler] '{selectedCard.Name}' 카드 스토리 표시 (선택 전)");
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
    /// 현재 뽑힌 카드들 가져오기
    /// </summary>
    private List<Card> GetCurrentDrawnCards()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[CardButtonHandler] GameManager.Instance가 null입니다.");
            return null;
        }

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
    /// 슬롯 인덱스를 실제 카드 인덱스로 변환
    /// </summary>
    private int GetActualCardIndex(int buttonSlotIndex)
    {
        if (GameManager.Instance == null) return -1;

        // 현재 뽑힌 카드들 가져오기
        var currentCards = GetCurrentDrawnCards();
        if (currentCards == null) return -1;

        int cardCount = currentCards.Count;

        Debug.Log($"[CardButtonHandler] 현재 카드 수: {cardCount}, 클릭된 슬롯: {buttonSlotIndex}");

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

        Debug.Log($"[CardButtonHandler] 슬롯 {buttonSlotIndex} → 카드 인덱스 {result}");
        return result;
    }
}