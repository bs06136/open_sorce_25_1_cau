// CharacterEffects.cs
// 한 파일에 모든 캐릭터 효과 인터페이스, 팩토리, 구현체를 모아두었습니다.

using UnityEngine;
using CardGame;
using System;
using System.Collections.Generic;
using System.Linq; // Deck 접근 시 LINQ 사용

namespace CardGame.Effects
{
    // 1. 캐릭터 효과 인터페이스
    public interface ICharacterEffect
    {
        void OnStartGame(GameManager gm);
        void OnTurnStart(GameManager gm);
        void OnAfterCardApply(GameManager gm, Card selectedCard);
        void OnReroll(GameManager gm);
    }

    // 2. 팩토리 클래스
    public static class CharacterEffectFactory
    {
        public static ICharacterEffect Create(CharacterType type)
        {
            return type switch
            {
                CharacterType.Explorer    => new ExplorerEffect(),
                CharacterType.Gravekeeper => new GravekeeperEffect(),
                CharacterType.Necromancer => new NecromancerEffect(),
                CharacterType.Cleric      => new ClericEffect(),
                CharacterType.Gambler     => new GamblerEffect(),
                CharacterType.Avenger     => new AvengerEffect(),
                CharacterType.Merchant    => new MerchantEffect(),
                CharacterType.DemonBinder => new DemonBinderEffect(),
                _ => new ExplorerEffect()
            };
        }
    }

    // 3. 개별 캐릭터 효과 구현체

    public class ExplorerEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm) { }
        public void OnTurnStart(GameManager gm) { }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm) { }
    }

    public class GravekeeperEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm)
        {
            gm.UnityGame.InsertDeathCards(10);
        }
        public void OnTurnStart(GameManager gm)
        {
            if (gm.GetCurrentTurn() % 2 == 0)
            {
                var deck = gm.UnityGame.Deck;
                var deathCard = deck.FirstOrDefault(c => c.Name == "죽음");
                if (deathCard != null)
                    deck.Remove(deathCard);
            }
        }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm) { }
    }

    public class NecromancerEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm)
        {
            gm.UnityPlayer.Curse = 3;
        }
        public void OnTurnStart(GameManager gm)
        {
            if (gm.GetCurrentTurn() % 3 == 0)
            {
                var reviveCard = CardLibrary.AllCards.FirstOrDefault(c => c.Name == "부활");
                if (reviveCard != null)
                    gm.UnityGame.Deck.Add(reviveCard);
            }
        }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm) { }
    }

    public class ClericEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm) { }
        public void OnTurnStart(GameManager gm)
        {
            int t = gm.GetCurrentTurn();
            if (t % 6 == 0)
                gm.UnityPlayer.Curse = Math.Max(0, gm.UnityPlayer.Curse - 1);
            if (t % 5 == 0)
                gm.UnityPlayer.Hp += 3;
        }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm) { }
    }

    public class GamblerEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm) { }
        public void OnTurnStart(GameManager gm)
        {
            float r1 = UnityEngine.Random.value;
            if (r1 < 0.2f)
                gm.UnityPlayer.Curse++;
            else
                gm.UnityPlayer.Hp++;

            float r2 = UnityEngine.Random.value;
            if (r2 < 0.01f)
            {
                var deck = gm.UnityGame.Deck;
                deck.Clear();
                foreach (var c in CardLibrary.AllCards)
                    deck.Add(c);
                int n = deck.Count;
                for (int i = 0; i < n - 1; i++)
                {
                    int j = UnityEngine.Random.Range(i, n);
                    var tmp = deck[i];
                    deck[i] = deck[j];
                    deck[j] = tmp;
                }
            }
        }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm) { }
    }

    public class AvengerEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm)
        {
            gm.UnityPlayer.Hp = 5;
            gm.UnityPlayer.Curse = 1;
        }
        public void OnTurnStart(GameManager gm) { }
        public void OnAfterCardApply(GameManager gm, Card selectedCard)
        {
            if (gm.UnityPlayer.Hp <= 5 && selectedCard.HpChange > 0)
                gm.UnityPlayer.Hp += selectedCard.HpChange;
        }
        public void OnReroll(GameManager gm) { }
    }

    public class MerchantEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm)
        {
            gm.UnityPlayer.RerollAvailable = 1;
        }
        public void OnTurnStart(GameManager gm) { }
        public void OnAfterCardApply(GameManager gm, Card selectedCard) { }
        public void OnReroll(GameManager gm)
        {
            for (int i = 0; i < 3; i++)
            {
                var bookCard = CardLibrary.AllCards.FirstOrDefault(c => c.Name == "저주받은 책");
                if (bookCard != null)
                    gm.UnityGame.Deck.Add(bookCard);
            }
        }
    }

    public class DemonBinderEffect : ICharacterEffect
    {
        public void OnStartGame(GameManager gm)
        {
            gm.UnityPlayer.Hp = 100;
        }
        public void OnTurnStart(GameManager gm)
        {
            gm.UnityPlayer.Curse++;
        }
        public void OnAfterCardApply(GameManager gm, Card selectedCard)
        {
            if (selectedCard.Name.Contains("악마"))
                gm.UnityPlayer.Curse = Math.Max(0, gm.UnityPlayer.Curse - 1);
        }
        public void OnReroll(GameManager gm) { }
    }
}
