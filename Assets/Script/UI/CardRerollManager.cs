using UnityEngine;
using TMPro;  // 用于显示剩余抽卡次数
using System.Collections.Generic;  // 用于 List

public class CardRerollManager : MonoBehaviour
{
    [Header("卡牌显示组件")]
    public CardDisplay cardLeftDisplay;
    public CardDisplay cardMiddleDisplay;
    public CardDisplay cardRightDisplay;

    [Header("卡牌数据")]
    public List<CardData> allCards;  // 所有可用的卡牌

    [Header("Reroll 控制")]
    public TextMeshProUGUI rerollText;  // 显示剩余抽卡次数的文本
    private int rerollCount = 3;        // 初始重抽次数

    void Start()
    {
        UpdateRerollUI();  // 初始化显示剩余抽卡次数
        DrawInitialCards(); // 游戏开始时初始化抽卡
    }

    /// <summary>
    /// 点击“重新抽卡”按钮时调用
    /// </summary>
    public void OnRerollClicked()
    {
        if (rerollCount > 0)
        {
            rerollCount--;   // 每点击一次，抽卡次数减少
            DrawNewCards();  // 重新抽卡
            UpdateRerollUI(); // 更新抽卡次数
        }
        else
        {
            rerollText.text = "No Rerolls Left";  // 如果没有剩余抽卡次数，显示提示
        }
    }

    /// <summary>
    /// 抽取 3 张新卡，并更新卡牌显示
    /// </summary>
    private void DrawNewCards()
    {
        List<CardData> selectedCards = GetRandomCards(3);  // 随机抽取 3 张卡

        // 更新卡牌显示
        cardLeftDisplay.SetCard(selectedCards[0]);
        cardMiddleDisplay.SetCard(selectedCards[1]);
        cardRightDisplay.SetCard(selectedCards[2]);
    }

    /// <summary>
    /// 从所有卡牌中随机抽取指定数量的卡牌
    /// </summary>
    private List<CardData> GetRandomCards(int count)
    {
        List<CardData> result = new List<CardData>();
        List<CardData> tempList = new List<CardData>(allCards);  // 复制一份卡牌列表，用于随机抽取

        for (int i = 0; i < count; i++)
        {
            int randIndex = Random.Range(0, tempList.Count);  // 随机选择卡牌
            result.Add(tempList[randIndex]);
            tempList.RemoveAt(randIndex);  // 移除已经选中的卡牌，保证不重复
        }

        return result;
    }

    /// <summary>
    /// 更新 UI 上显示的剩余抽卡次数
    /// </summary>
    private void UpdateRerollUI()
    {
        rerollText.text = $"Rerolls Left: {rerollCount}";  // 更新剩余抽卡次数
    }

    /// <summary>
    /// 初始化卡牌显示（开始时调用）
    /// </summary>
    private void DrawInitialCards()
    {
        List<CardData> initialCards = GetRandomCards(3);  // 随机抽取 3 张卡作为初始卡牌

        // 设置初始卡牌
        cardLeftDisplay.SetCard(initialCards[0]);
        cardMiddleDisplay.SetCard(initialCards[1]);
        cardRightDisplay.SetCard(initialCards[2]);
    }
}


    