using UnityEngine;
using UnityEngine.UI;
using CardGame;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    public Sprite[] cardSprites;
    public Image[] cardImageSlots;

    public void ShowRandomCards()
    {
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            int randomIndex = Random.Range(0, cardSprites.Length);
            cardImageSlots[i].sprite = cardSprites[randomIndex];
        }
    }

    public void DisplayCards(List<Card> cards)
    {
        foreach (var card in cards)
        {
            Debug.Log($"카드: {card.Name} - {card.Description}");
        }

        // TODO: 버튼이나 UI에 텍스트/이미지로 반영하는 작업 추가
    }
}
