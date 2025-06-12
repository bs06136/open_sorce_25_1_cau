 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using CardGame;

public class CardScrollManager : MonoBehaviour
{
    // 封装一张卡牌需要显示的数据
    public class CardInfo
    {
        public Sprite  cardImage;
        public string  cardName;
        public string  hpText;
        public string  curseText;
        public string  specialEffectText;
    }

    [Header("卡牌行 Prefab & 父容器")]
    public GameObject cardRowPrefab;
    public Transform  contentParent;

    [Header("卡牌图片 (Inspector 中按顺序填入，对应 CardLibrary.AllCards)")]
    public Sprite[] cardImages;

    void Start()
    {
        // 1. 从库里拿到所有卡
        var allCards = CardLibrary.AllCards;
        // 2. 计算要显示的张数：不超过图片数组长度，也不超过卡牌总数
        int displayCount = Mathf.Min(allCards.Count, cardImages.Length);

        // 3. 构造 CardInfo 数组
        var infos = new CardInfo[displayCount];
        for (int i = 0; i < displayCount; i++)
        {
            var card = allCards[i];
            infos[i] = new CardInfo
            {
                cardImage         = cardImages[i],
                cardName          = card.Name,
                hpText            = card.HpChange.ToString(),
                curseText         = card.CurseChange.ToString(),
                specialEffectText = card.Description
            };
        }

        // 4. 实例化每一行并填值
        foreach (var info in infos)
        {
            var row = Instantiate(cardRowPrefab, contentParent);

            // 查出行内的组件
            var img          = row.GetComponentsInChildren<Image>()
                                  .FirstOrDefault(x => x.gameObject.name == "CardImage");
            var hpLabel      = row.GetComponentsInChildren<TMP_Text>()
                                  .FirstOrDefault(t => t.name == "CardEffectText_HP");
            var curseLabel   = row.GetComponentsInChildren<TMP_Text>()
                                  .FirstOrDefault(t => t.name == "CardEffectText_Curse");
            var effectLabel  = row.GetComponentsInChildren<TMP_Text>()
                                  .FirstOrDefault(t => t.name == "CardEffectText_Effect");
            var storyLabel   = row.GetComponentsInChildren<TMP_Text>()
                                  .FirstOrDefault(t => t.name == "StoryText");

            // 填入数值
            if (img)         img.sprite = info.cardImage;
            if (hpLabel)     hpLabel.text = info.hpText;
            if (curseLabel)  curseLabel.text = info.curseText;
            if (effectLabel) effectLabel.text = info.specialEffectText;

            // 从 CardStoryMap 里根据名称取故事（英文）
            if (storyLabel != null)
            {
                string story = CardStoryMap.GetStory(info.cardName);
                storyLabel.text = string.IsNullOrEmpty(story) ? "No story." : story;
            }
            else
            {
                Debug.LogWarning("[CardScrollManager] 找不到名为 StoryText 的 TMP_Text");
            }
        }
    }
}
