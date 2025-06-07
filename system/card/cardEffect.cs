using System;
using System.Collections.Generic;

namespace CardGame
{
    public static class CardEffect
    {
        public static void DeathEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[죽음 효과] 사망합니다...");
            game.DisplayScore();
        }

        public static void FoolEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[바보 효과] 체력을 10으로 초기화합니다!");
            player.Hp = 10;
        }

        public static void TowerEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[탑 효과] 다음 턴 스킵!");
            player.SkipNextTurn = true;
        }

        public static void LoversEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[연인 효과] 리롤 기회 +1");
            player.RerollAvailable++;
        }

        public static void ReviveEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[부활 효과] 덱에서 죽음 카드 제거");
            game.Deck.RemoveAll(c => c.Name == "죽음");
        }

        public static void LifeEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[생명 효과] 덱에 무작위 카드 20장 추가");
            var newCards = game.GenerateDeck().GetRange(0, 20);
            var rnd = new Random();
            foreach (var card in newCards)
            {
                var pos = rnd.Next(0, game.Deck.Count + 1);
                game.Deck.Insert(pos, card);
            }
        }

        public static void FortuneWheelEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine($"[운명의 수레바퀴 효과] 체력과 저주 교환: {player.Hp}, {player.Curse} -> ", false);
            int newHp = player.Curse * 5;
            int newCurse = player.Hp / 5;
            player.Hp = newHp;
            player.Curse = newCurse;
            Console.WriteLine($"{player.Hp}, {player.Curse}");
        }

        public static void HangedManEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[매달린 남자 효과] 다음 턴 2장만 뽑음");
            player.NextDrawNum = 2;
        }

        public static void JudgementEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[심판 효과] 덱에서 무작위 카드 5장 제거");
            var rnd = new Random();
            for (int i = 0; i < 5 && game.Deck.Count > 0; i++)
            {
                int index = rnd.Next(game.Deck.Count);
                game.Deck.RemoveAt(index);
            }
        }

        public static void TemperanceEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[절제 효과] 3턴간 저주 피해 면역");
            player.NonCurseDamageTurn = 3;
        }

        public static void ScampEffect(Player player, Game game, List<Card> cards)
        {
            var rnd = new Random();
            int heal = rnd.Next(1, 11);
            Console.WriteLine($"[악동 효과] 체력 {heal} 증가");
            player.Hp += heal;
        }

        public static void HierophantEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[교황 효과] 3턴간 저주 감소 불가");
            player.NonCurseDecreaseTurn = 3;
        }

        public static void HermitEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[은둔자 효과] 5턴 후 체력 7 증가 예약");
            player.DelayedEffects.Add((5, () => player.Hp += 7));
        }

        public static void MagicianEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[마법사 효과] 3턴 후 저주 3 감소 예약");
            player.DelayedEffects.Add((3, () => player.Curse = Math.Max(0, player.Curse - 3)));
        }

        public static void HighPriestessEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[여교황 효과] 3턴간 체력 증가 불가");
            player.NonHpIncreaseTurn = 3;
        }

        public static void EmpressEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[여제 효과] 덱에서 죽음 카드 5장 제거");
            int count = 0;
            for (int i = game.Deck.Count - 1; i >= 0 && count < 5; i--)
            {
                if (game.Deck[i].Name == "죽음")
                {
                    game.Deck.RemoveAt(i);
                    count++;
                }
            }
        }

        public static void EmperorEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[황제 효과] 체력 & 저주 두 배 증가");
            player.Hp *= 2;
            player.Curse *= 2;
        }

        public static void ChariotEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[전차 효과] 다음 턴 2장 선택");
            player.NextPickNum = 2;
        }

        public static void JusticeEffect(Player player, Game game, List<Card> cards)
        {
            int total = player.Hp + player.Curse;
            player.Curse = total / 6;
            player.Hp = total - player.Curse;
            Console.WriteLine($"[정의 효과] 체력 & 저주 재분배: {player.Hp}, {player.Curse}");
        }

        public static void MirrorEffect(Player player, Game game, List<Card> cards)
        {
            if (player.LastCard != null)
            {
                Console.WriteLine($"[거울 효과] '{player.LastCard.Name}' 효과 재발동");
                player.LastCard.Apply(player, game, cards);
            }
            else
            {
                Console.WriteLine("[거울 효과] 마지막 카드 없음");
            }
        }

        public static void EclipseEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[일식 효과] 2턴 후 저주 +2 예약");
            player.DelayedEffects.Add((2, () => player.Curse += 2));
        }

        public static void BlackMarketEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[암거래 효과] 5턴간 죽음 카드 추가 금지");
            player.NotAddDeath = 5;
        }

        public static void EmberEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[불씨 효과] 체력 1 이하 시 회복 효과 활성화");
            player.Ember = true;
        }

        public static void ProphetEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[예언자 효과] 다음 턴 체력 감소 & 저주 증가 무효");
            player.NonHpDecreaseTurn = 1;
            player.NonCurseIncreaseTurn = 1;
        }

        public static void ApocalypseScriptureEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[종말의 경전 효과] 덱 리셋");
            game.Deck = game.GenerateDeck();
        }

        public static void ArchangelEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[대천사 효과] 죽음 카드 뽑기 시 교체 활성화");
            player.Archangel = true;
        }

        public static void SoulCandleEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[영혼의 초 효과] 3턴 후 저주 2 감소 예약");
            player.DelayedEffects.Add((3, () => player.Curse = Math.Max(0, player.Curse - 2)));
        }

        public static void SoulWeddingEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[영혼 결혼식 효과] 리롤 기회 +1");
            player.RerollAvailable++;
        }

        public static void BloodPactEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[피의 서약 효과] 2턴간 저주 증가 무효");
            player.NonCurseIncreaseTurn = 2;
        }

        public static void GambleOfFateEffect(Player player, Game game, List<Card> cards)
        {
            Console.WriteLine("[운명의 유희 효과] 다음 턴 무작위 선택 활성화");
            player.RandomChoice = true;
        }
    }
}
