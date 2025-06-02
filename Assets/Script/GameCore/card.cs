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

        public Card(string name, int hpChange = 0, int curseChange = 0, string description = "", Action<Player, Game, List<Card>> special = null)
        {
            Name = name;
            HpChange = hpChange;
            CurseChange = curseChange;
            Description = description;
            Special = special;
        }

        public Card Copy(Dictionary<string, object> overrides = null)
        {
            var copy = new Card(Name, HpChange, CurseChange, Description, Special);
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
            // 세계 카드는 특별 처리
            if (Name == "세계" && Special != null)
            {
                Special(player, game, cards);
                return;
            }

            // 체력 변화 처리 및 UI 표시
            if (HpChange != 0)
            {
                string hpMessage = $"체력이 {(HpChange > 0 ? "증가" : "감소")}합니다: {player.Hp} → {player.Hp + HpChange}";
                Debug.Log(hpMessage);

                // UI에 체력 변화 표시
                var effectType = HpChange > 0 ? EffectType.Positive : EffectType.Negative;
                GameEvents.TriggerCardEffect(hpMessage, effectType);

                player.Hp += HpChange;
            }

            // 저주 변화 처리 및 UI 표시
            if (CurseChange != 0)
            {
                string curseMessage = $"저주가 {(CurseChange > 0 ? "증가" : "감소")}합니다: {player.Curse} → {player.Curse + CurseChange}";
                Debug.Log(curseMessage);

                // UI에 저주 변화 표시
                var effectType = CurseChange > 0 ? EffectType.Negative : EffectType.Positive;
                GameEvents.TriggerCardEffect(curseMessage, effectType);

                player.Curse += CurseChange;
            }

            // 특수 효과 실행
            Special?.Invoke(player, game, cards);
        }
    }
}