using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Image cardImage;  // 卡牌图像显示
    public Text cardName;  // 卡牌名称显示
    public Text description;  // 卡牌描述显示

    public void SetCard(CardData data)
    {
        cardImage.sprite = data.cardImage;
        cardName.text = data.cardName;
        description.text = data.description;
    }
}

