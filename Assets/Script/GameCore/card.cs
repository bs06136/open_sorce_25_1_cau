#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public class Card
    {
        public string Name { get; set; }
        public int HpChange { get; set; }
        public int CurseChange { get; set; }
        public string Description { get; set; }
        public Action<Player, Game, List<Card>> Special { get; set; }

        // ✅ 추가된 카드 스토리 필드
        public string Story { get; set; }

        public Card(string name, int hpChange = 0, int curseChange = 0, string description = "", Action<Player, Game, List<Card>> special = null, string story = "")
        {
            Name = name;
            HpChange = hpChange;
            CurseChange = curseChange;
            Description = description;
            Special = special;
            Story = story;
        }

        public Card Copy(Dictionary<string, object> overrides = null)
        {
            var copy = new Card(Name, HpChange, CurseChange, Description, Special, Story);
            if (overrides != null)
            {
                foreach (var entry in overrides)
                {
                    typeof(Card).GetProperty(entry.Key)?.SetValue(copy, entry.Value);
                }
            }
            return copy;
        }

        public void Apply(Player player, Game game, List<Card> cards)
        {
            if (Name == "세계" && Special != null)
            {
                var tempHpChange = HpChange;
                var tempCurseChange = CurseChange;
                foreach (var card in cards)
                {
                    tempHpChange += card.HpChange;
                    tempCurseChange += card.CurseChange;
                }
                player.Hp += tempHpChange;
                player.Curse += tempCurseChange;

                Special(player, game, cards);
                return;
            }

            if (HpChange != 0)
            {
                string hpMessage = $"체력이 {(HpChange > 0 ? "증가" : "감소")}합니다: {player.Hp} → {player.Hp + HpChange}";
                Debug.Log(hpMessage);
                GameEvents.TriggerCardEffect(hpMessage, HpChange > 0 ? EffectType.Positive : EffectType.Negative);
                player.Hp += HpChange;
            }

            if (CurseChange != 0)
            {
                string curseMessage = $"저주가 {(CurseChange > 0 ? "증가" : "감소")}합니다: {player.Curse} → {player.Curse + CurseChange}";
                Debug.Log(curseMessage);
                GameEvents.TriggerCardEffect(curseMessage, CurseChange > 0 ? EffectType.Negative : EffectType.Positive);
                player.Curse += CurseChange;
            }

            Special?.Invoke(player, game, cards);
        }
    }
}
