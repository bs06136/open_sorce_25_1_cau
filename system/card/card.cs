using System;
using System.Collections.Generic;

namespace CardGame
{
    public class Card
    {
        public string Name { get; set; }
        public int HpChange { get; set; }
        public int CurseChange { get; set; }
        public string Description { get; set; }
        public Action<Player, Game, List<Card>>? Special { get; set; }

        public Card(string name, int hpChange = 0, int curseChange = 0, string description = "", Action<Player, Game, List<Card>>? special = null)
        {
            Name = name;
            HpChange = hpChange;
            CurseChange = curseChange;
            Description = description;
            Special = special;
        }

        public Card Copy(Dictionary<string, object>? overrides = null)
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
            if (Name == "세계" && Special != null)
            {
                Special(player, game, cards);
                return;
            }

            if (HpChange != 0)
            {
                Console.WriteLine($"체력이 {(HpChange > 0 ? "증가" : "감소")}합니다. : {player.Hp} -> {player.Hp + HpChange}");
                player.Hp += HpChange;
            }

            if (CurseChange != 0)
            {
                Console.WriteLine($"저주가 {(CurseChange > 0 ? "증가" : "감소")}합니다. : {player.Curse} -> {player.Curse + CurseChange}");
                player.Curse += CurseChange;
            }

            Special?.Invoke(player, game, cards);
        }
    }
}
