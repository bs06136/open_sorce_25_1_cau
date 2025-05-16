using UnityEngine;
using UnityEngine.UI;

public class CardImageLoader : MonoBehaviour
{
    public Sprite[] cardSprites;       // 카드 스프라이트 모음
    public Image[] cardImageSlots;     // UI에 배치된 Image

    void Start()
    {
        ShowRandomCards();
    }

    void ShowRandomCards()
    {
        for (int i = 0; i < cardImageSlots.Length; i++)
        {
            int randomIndex = Random.Range(0, cardSprites.Length);
            cardImageSlots[i].sprite = cardSprites[randomIndex];
        }
    }
}