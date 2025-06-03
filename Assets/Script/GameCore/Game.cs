using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame
{
    public class Game
    {
        public int Turn { get; set; } = 1;
        public Player Player { get; set; }
        public List<Card> Deck { get; set; }

        private static readonly Random rnd = new();

        // ✅ 외부에서 Player를 전달받도록 생성자 수정
        public Game(Player? player = null)
        {
            Player = player ?? new Player(); // 없으면 기본 Player
            Deck = GenerateDeck();
        }

        public List<Card> GenerateDeck()
        {
            var deck = CardLibrary.AllCards.Where(c => c.Name != "죽음").ToList();
            Shuffle(deck);
            return deck;
        }

        public void Shuffle(List<Card> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        public List<Card> DrawCards(int count)
        {
            if (Deck.Count < count)
                Deck.AddRange(GenerateDeck());

            var drawn = Deck.Take(count).ToList();
            Deck.RemoveRange(0, count);
            GameManager.Instance?.ShowRemainDeckNum();
            return drawn;
        }

        public void InsertDeathCards(int count)
        {
            var deathCard = CardLibrary.AllCards.First(c => c.Name == "죽음");
            for (int i = 0; i < count; i++)
            {
                int pos = rnd.Next(Deck.Count + 1);
                Deck.Insert(pos, deathCard);
            }
        }

        public void DisplayScore()
        {
            Console.WriteLine($"\n=== 최종 점수 ===");
            Console.WriteLine($"체력: {Player.Hp}");
            Console.WriteLine($"저주: {Player.Curse}");
            Console.WriteLine($"층수: {Turn - 1}");
            Environment.Exit(0);
        }
    }
}
