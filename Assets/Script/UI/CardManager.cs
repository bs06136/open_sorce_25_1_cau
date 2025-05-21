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
            Debug.Log($"ī��: {card.Name} - {card.Description}");
        }

        // TODO: ��ư�̳� UI�� �ؽ�Ʈ/�̹����� �ݿ��ϴ� �۾� �߰�
    }
}
