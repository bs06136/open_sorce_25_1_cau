using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;
    public Text cardName;
    public Text description;

    /// <summary>
    /// UnifiedCardData를 사용한 카드 설정 (주요 메서드)
    /// </summary>
    public void SetCard(UnifiedCardData data)
    {
        if (data == null)
        {
            ClearCard();
            return;
        }

        if (cardImage != null && data.cardImage != null)
            cardImage.sprite = data.cardImage;

        if (cardName != null)
            cardName.text = data.cardName;

        if (description != null)
        {
            // 카드 정보를 포함한 상세 설명 생성
            string detailedDescription = data.description;

            if (data.hpChange != 0 || data.curseChange != 0)
            {
                detailedDescription += $"\n(HP: {data.hpChange:+#;-#;0}, 저주: {data.curseChange:+#;-#;0})";
            }

            description.text = detailedDescription;
        }

        // 카드 활성화
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 기존 CardData와의 호환성 유지 (하위 호환)
    /// </summary>
    public void SetCard(CardData data)
    {
        if (data == null)
        {
            ClearCard();
            return;
        }

        if (cardImage != null && data.cardImage != null)
            cardImage.sprite = data.cardImage;

        if (cardName != null)
            cardName.text = data.cardName;

        if (description != null)
            description.text = data.description;

        gameObject.SetActive(true);
    }

    /// <summary>
    /// 카드 정보 지우기
    /// </summary>
    public void ClearCard()
    {
        if (cardImage != null)
            cardImage.sprite = null;

        if (cardName != null)
            cardName.text = "";

        if (description != null)
            description.text = "";

        // 카드 비활성화는 선택사항
        // gameObject.SetActive(false);
    }
}