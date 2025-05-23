using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 主菜单界面
    public GameObject mainMenuPanel;

    // 卡牌分类面板
    public GameObject cardTypePanel;

    // 各分类按钮
    public Button attackButton;
    public Button defenseButton;
    public Button healButton;
    public Button curseButton;

    // 卡牌显示的容器
    public Transform cardGroupContainer;

    // 各类卡牌数据
    public List<CardData> attackCards;
    public List<CardData> defenseCards;
    public List<CardData> healCards;
    public List<CardData> curseCards;

    void Start()
    {
        // 为每个按钮添加事件监听
        attackButton.onClick.AddListener(() => ShowCards(attackCards));
        defenseButton.onClick.AddListener(() => ShowCards(defenseCards));
        healButton.onClick.AddListener(() => ShowCards(healCards));
        curseButton.onClick.AddListener(() => ShowCards(curseCards));

        // 默认隐藏卡牌分类面板
        cardTypePanel.SetActive(false);
    }

    // 切换到卡牌类型面板
    public void OnCardTypeClicked()
    {
        mainMenuPanel.SetActive(false);  // 隐藏主菜单
        cardTypePanel.SetActive(true);   // 显示卡牌类型面板
    }

    // 根据点击的按钮展示对应类型的卡牌
    private void ShowCards(List<CardData> cards)
    {
        // 清空现有的卡牌显示
        foreach (Transform child in cardGroupContainer)
        {
            Destroy(child.gameObject);
        }

        // 根据选中的卡牌类型生成卡牌
        foreach (var card in cards)
        {
            // 创建一个新的UI对象来显示卡牌
            GameObject cardObject = new GameObject(card.cardName);
            cardObject.transform.SetParent(cardGroupContainer);
            
            // 创建Image组件显示卡牌图像
            Image cardImage = cardObject.AddComponent<Image>();
            cardImage.sprite = card.cardImage;  // 设置卡牌图片

            // 创建Text组件显示卡牌描述
            Text cardText = cardObject.AddComponent<Text>();
            cardText.text = card.cardName;  // 设置卡牌名称（可以添加其他信息）
        }
    }
}

       