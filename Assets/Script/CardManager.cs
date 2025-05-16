using UnityEngine;
using UnityEngine.UI;

public class CardImageLoader : MonoBehaviour
{
    public Sprite[] cardSprites;       // ī�� ��������Ʈ ����
    public Image[] cardImageSlots;     // UI�� ��ġ�� Image

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