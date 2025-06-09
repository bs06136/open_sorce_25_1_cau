using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CardGame;


public class CardScrollManager : MonoBehaviour
{
    public class CardInfo
    {
        public Sprite cardImage;
        public string hpText;
        public string curseText;
        public string specialEffectText;
    }

    public GameObject cardRowPrefab;      // 프리팹
    public Transform contentParent;       // 카드 정보가 들어갈 부모 오브젝트
    public CardInfo[] cardList = new CardInfo[41]; // 카드 정보 리스트

    [Header("카드 이미지")]
    public Sprite[] cardImages = new Sprite[41];

    void Start()
    {
        var allCards = CardLibrary.AllCards.ToList();

        for (int i = 0; i < cardList.Length; i++)
        {
            cardList[i] = new CardInfo();
            var card = allCards[i];
            cardList[i].cardImage = cardImages[i];
            cardList[i].hpText = card.HpChange.ToString();
            cardList[i].curseText = card.CurseChange.ToString();
            cardList[i].specialEffectText = card.Description;
        }


        foreach (CardInfo card in cardList)
        {
            GameObject newRow = Instantiate(cardRowPrefab, contentParent);

            // CardImage를 하위 전체에서 찾음
            var imgObj = newRow.GetComponentsInChildren<Image>()
                .FirstOrDefault(img => img.gameObject.name == "CardImage")?.gameObject;
            var hpText = newRow.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(t => t.name == "CardEffectText_HP");
            var curseText = newRow.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(t => t.name == "CardEffectText_Curse");
            var effectText = newRow.GetComponentsInChildren<TMP_Text>()
                .FirstOrDefault(t => t.name == "CardEffectText_Effect");

            if (imgObj == null) Debug.LogError("CardImage 오브젝트를 찾을 수 없습니다!");
            if (hpText == null) Debug.LogError("CardEffectText_HP 오브젝트를 찾을 수 없습니다!");
            if (curseText == null) Debug.LogError("CardEffectText_Curse 오브젝트를 찾을 수 없습니다!");
            if (effectText == null) Debug.LogError("CardEffectText_Effect 오브젝트를 찾을 수 없습니다!");

            if (imgObj != null)
                imgObj.GetComponent<Image>().sprite = card.cardImage;
            if (hpText != null)
                hpText.text = card.hpText;
            if (curseText != null)
                curseText.text = card.curseText;
            if (effectText != null)
                effectText.text = card.specialEffectText;
        }
    }
}
