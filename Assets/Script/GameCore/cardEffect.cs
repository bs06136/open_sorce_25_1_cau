#nullable disable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CardGame
{
    public static class CardEffect
    {
        public static void DeathEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[죽음 효과] 사망합니다...");
            Debug.Log("[죽음 효과] 사망합니다...");
            game.DisplayScore();
        }

        public static void FoolEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[바보 효과] 체력을 10으로 초기화합니다!");
            Debug.Log("[바보 효과] 체력을 10으로 초기화합니다!");
            player.Hp = 10;
        }

        public static void TowerEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[탑 효과] 다음 턴 스킵!");
            Debug.Log("[탑 효과] 다음 턴 스킵!");
            player.SkipNextTurn = true;
        }

        public static void LoversEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[연인 효과] 리롤 기회 +1");
            Debug.Log("[연인 효과] 리롤 기회 +1");
            player.RerollAvailable++;
        }

        public static void ReviveEffect(Player player, Game game, List<Card> cards)
        {
            int removedCards = game.Deck.RemoveAll(c => c.Name == "죽음");
            GameEvents.TriggerPositiveEffect($"[부활 효과] 덱에서 죽음 카드 {removedCards}장 제거");
            Debug.Log($"[부활 효과] 덱에서 죽음 카드 {removedCards}장 제거");
        }

        public static void LifeEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[생명 효과] 덱에 무작위 카드 20장 추가");
            Debug.Log("[생명 효과] 덱에 무작위 카드 20장 추가");
            var newCards = game.GenerateDeck().GetRange(0, 20);
            var rnd = new System.Random();
            foreach (var card in newCards)
            {
                var pos = rnd.Next(0, game.Deck.Count + 1);
                game.Deck.Insert(pos, card);
            }
        }

        public static void FortuneWheelEffect(Player player, Game game, List<Card> cards)
        {
            int oldHp = player.Hp;
            int oldCurse = player.Curse;
            int newHp = player.Curse * 5;
            int newCurse = player.Hp / 5;

            GameEvents.TriggerSpecialEffect($"[운명의 수레바퀴 효과] 체력과 저주 교환: {oldHp}, {oldCurse} → {newHp}, {newCurse}");
            Debug.Log($"[운명의 수레바퀴 효과] 체력과 저주 교환: {oldHp}, {oldCurse} → {newHp}, {newCurse}");

            player.Hp = newHp;
            player.Curse = newCurse;
        }

        public static void HangedManEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[매달린 남자 효과] 다음 턴 2장만 뽑음");
            Debug.Log("[매달린 남자 효과] 다음 턴 2장만 뽑음");
            player.NextDrawNum = 2;
        }

        public static void JudgementEffect(Player player, Game game, List<Card> cards)
        {
            var rnd = new System.Random();
            int removedCount = 0;
            for (int i = 0; i < 5 && game.Deck.Count > 0; i++)
            {
                int index = rnd.Next(game.Deck.Count);
                game.Deck.RemoveAt(index);
                removedCount++;
            }
            GameEvents.TriggerSpecialEffect($"[심판 효과] 덱에서 무작위 카드 {removedCount}장 제거");
            Debug.Log($"[심판 효과] 덱에서 무작위 카드 {removedCount}장 제거");
        }

        public static void TemperanceEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[절제 효과] 3턴간 저주 피해 면역");
            Debug.Log("[절제 효과] 3턴간 저주 피해 면역");
            player.NonCurseDamageTurn = 3;
        }

        public static void ScampEffect(Player player, Game game, List<Card> cards)
        {
            var rnd = new System.Random();
            int heal = rnd.Next(1, 11);
            GameEvents.TriggerPositiveEffect($"[광대 효과] 체력 {heal} 증가");
            Debug.Log($"[광대 효과] 체력 {heal} 증가");
            player.Hp += heal;
        }

        public static void HierophantEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[교황 효과] 3턴간 저주 감소 불가");
            Debug.Log("[교황 효과] 3턴간 저주 감소 불가");
            player.NonCurseDecreaseTurn = 3;
        }

        public static void HermitEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[은둔자 효과] 5턴 후 체력 7 증가 예약");
            Debug.Log("[은둔자 효과] 5턴 후 체력 7 증가 예약");
            player.DelayedEffects.Add((5, () => {
                GameEvents.TriggerPositiveEffect("[지연 효과] 은둔자 - 체력 7 증가!");
                player.Hp += 7;
            }
            ));
        }

        public static void MagicianEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[마법사 효과] 3턴 후 저주 3 감소 예약");
            Debug.Log("[마법사 효과] 3턴 후 저주 3 감소 예약");
            player.DelayedEffects.Add((3, () => {
                GameEvents.TriggerPositiveEffect("[지연 효과] 마법사 - 저주 3 감소!");
                player.Curse = Math.Max(0, player.Curse - 3);
            }
            ));
        }

        public static void HighPriestessEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[여교황 효과] 3턴간 체력 증가 불가");
            Debug.Log("[여교황 효과] 3턴간 체력 증가 불가");
            player.NonHpIncreaseTurn = 3;
        }

        public static void EmpressEffect(Player player, Game game, List<Card> cards)
        {
            int count = 0;
            for (int i = game.Deck.Count - 1; i >= 0 && count < 5; i--)
            {
                if (game.Deck[i].Name == "죽음")
                {
                    game.Deck.RemoveAt(i);
                    count++;
                }
            }
            GameEvents.TriggerPositiveEffect($"[여제 효과] 덱에서 죽음 카드 {count}장 제거");
            Debug.Log($"[여제 효과] 덱에서 죽음 카드 {count}장 제거");
        }

        public static void EmperorEffect(Player player, Game game, List<Card> cards)
        {
            int oldHp = player.Hp;
            int oldCurse = player.Curse;
            player.Hp *= 2;
            player.Curse *= 2;
            GameEvents.TriggerSpecialEffect($"[황제 효과] 체력 & 저주 두 배: {oldHp}→{player.Hp}, {oldCurse}→{player.Curse}");
            Debug.Log($"[황제 효과] 체력 & 저주 두 배: {oldHp}→{player.Hp}, {oldCurse}→{player.Curse}");
        }

        public static void ChariotEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[전차 효과] 다음 턴 2장 선택");
            Debug.Log("[전차 효과] 다음 턴 2장 선택");
            player.NextPickNum = 2;
        }

        public static void JusticeEffect(Player player, Game game, List<Card> cards)
        {
            int total = player.Hp + player.Curse;
            int oldHp = player.Hp;
            int oldCurse = player.Curse;
            player.Curse = total / 6;
            player.Hp = total - player.Curse;
            GameEvents.TriggerSpecialEffect($"[정의 효과] 체력 & 저주 재분배: {oldHp},{oldCurse} → {player.Hp},{player.Curse}");
            Debug.Log($"[정의 효과] 체력 & 저주 재분배: {oldHp},{oldCurse} → {player.Hp},{player.Curse}");
        }

        public static void MirrorEffect(Player player, Game game, List<Card> cards)
        {
            if (player.LastCard != null)
            {
                GameEvents.TriggerSpecialEffect($"[거울 효과] '{player.LastCard.Name}' 효과 재발동");
                Debug.Log($"[거울 효과] '{player.LastCard.Name}' 효과 재발동");
                player.LastCard.Apply(player, game, cards);
            }
            else
            {
                GameEvents.TriggerCardEffect("[거울 효과] 마지막 카드 없음");
                Debug.Log("[거울 효과] 마지막 카드 없음");
            }
        }

        public static void EclipseEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerNegativeEffect("[일식 효과] 2턴 후 저주 +2 예약");
            Debug.Log("[일식 효과] 2턴 후 저주 +2 예약");
            player.DelayedEffects.Add((2, () => {
                GameEvents.TriggerNegativeEffect("[지연 효과] 일식 - 저주 2 증가!");
                player.Curse += 2;
            }
            ));
        }

        public static void BlackMarketEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[암거래 효과] 5턴간 죽음 카드 추가 금지");
            Debug.Log("[암거래 효과] 5턴간 죽음 카드 추가 금지");
            player.NotAddDeath = 5;
        }

        public static void EmberEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[불씨 효과] 체력 1 이하 시 회복 효과 활성화");
            Debug.Log("[불씨 효과] 체력 1 이하 시 회복 효과 활성화");
            player.Ember = true;
        }

        public static void ProphetEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[예언자 효과] 다음 턴 체력 감소 & 저주 증가 무효");
            Debug.Log("[예언자 효과] 다음 턴 체력 감소 & 저주 증가 무효");
            player.NonHpDecreaseTurn = 1;
            player.NonCurseIncreaseTurn = 1;
        }

        public static void ApocalypseScriptureEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[종말의 경전 효과] 덱 리셋");
            Debug.Log("[종말의 경전 효과] 덱 리셋");
            game.Deck = game.GenerateDeck();
        }

        public static void ArchangelEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[대천사 효과] 죽음 카드 뽑기 시 교체 활성화");
            Debug.Log("[대천사 효과] 죽음 카드 뽑기 시 교체 활성화");
            player.Archangel = true;
        }

        public static void SoulCandleEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[영혼의 초 효과] 3턴 후 저주 2 감소 예약");
            Debug.Log("[영혼의 초 효과] 3턴 후 저주 2 감소 예약");
            player.DelayedEffects.Add((3, () => {
                GameEvents.TriggerPositiveEffect("[지연 효과] 영혼의 초 - 저주 2 감소!");
                player.Curse = Math.Max(0, player.Curse - 2);
            }
            ));
        }

        public static void SoulWeddingEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[영혼 결혼식 효과] 리롤 기회 +1");
            Debug.Log("[영혼 결혼식 효과] 리롤 기회 +1");
            player.RerollAvailable++;
        }

        public static void BloodPactEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerPositiveEffect("[피의 서약 효과] 2턴간 저주 증가 무효");
            Debug.Log("[피의 서약 효과] 2턴간 저주 증가 무효");
            player.NonCurseIncreaseTurn = 2;
        }

        public static void GambleOfFateEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect("[운명의 유희 효과] 다음 턴 무작위 선택 활성화");
            Debug.Log("[운명의 유희 효과] 다음 턴 무작위 선택 활성화");
            player.RandomChoice = true;
        }

        public static void WorldEffect(Player player, Game game, List<Card> cards)
        {
            GameEvents.TriggerSpecialEffect($"[세계 효과] {cards.Count}개 카드의 특수효과 실행");
            Debug.Log($"[세계 효과] {cards.Count}개 카드의 특수효과 실행");

            foreach (var card in cards)
            {
                if (card.Special != null)
                {
                    try
                    {
                        GameEvents.TriggerCardEffect($"[세계] {card.Name} 특수효과 실행", EffectType.Special);
                        Debug.Log($"[세계] {card.Name} 특수효과 실행");
                        card.Special(player, game, cards);
                    }
                    catch (Exception e)
                    {
                        GameEvents.TriggerNegativeEffect($"[세계] {card.Name} 실행 중 오류!");
                        Debug.Log($"[세계] {card.Name} 실행 중 오류: {e.Message}");
                    }
                }
            }
        }
    }
}