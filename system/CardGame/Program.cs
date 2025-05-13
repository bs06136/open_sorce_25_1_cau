using System;
using System.Collections.Generic;
using System.Threading;

namespace CardGame
{
    public class Program
    {
        private const int REGULAR_CURSE_INCREASE_FREQ = 5;
        private const int REGULAR_CURSE_INCREASE_INIT = 1;

        static void Main()
        {
            var game = new Game();
            var player = game.Player;

            while (game.Turn <= 40 && player.Hp > 0)
            {
                Console.WriteLine($"\n=== Turn {game.Turn} ===");
                Console.WriteLine($"HP: {player.Hp} | Curse: {player.Curse} | Deck: {game.Deck.Count} | Ember: {player.Ember}");

                if (player.SkipNextTurn)
                {
                    Console.WriteLine("이번 턴 스킵!");
                    player.SkipNextTurn = false;
                }
                else
                {
                    var drawCount = player.NextDrawNum;
                    player.NextDrawNum = 3;

                    var cards = game.DrawCards(drawCount);

                    for (int i = 0; i < cards.Count; i++)
                    {
                        if (player.Archangel && cards[i].Name == "죽음")
                        {
                            cards[i] = CardLibrary.AllCards.Find(c => c.Name != "죽음")!;
                            Console.WriteLine($"[대천사 효과] 죽음 카드가 '{cards[i].Name}'로 교체됨");
                        }
                    }
                    player.Archangel = false;

                    for (int i = 0; i < cards.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {cards[i].Name} (HP {cards[i].HpChange}, Curse {cards[i].CurseChange}) - {cards[i].Description}");
                    }

                    if (player.RerollAvailable > 0)
                    {
                        Console.Write("리롤 하시겠습니까? (y/n): ");
                        string? rerollInput = Console.ReadLine();
                        if (rerollInput?.Trim().ToLower() == "y")
                        {
                            player.RerollAvailable--;
                            continue;
                        }
                    }

                    int pickCount = player.NextPickNum;
                    player.NextPickNum = 1;

                    List<Card> selectedCards = new();

                    if (player.RandomChoice)
                    {
                        selectedCards.Add(cards[new Random().Next(cards.Count)]);
                        Console.WriteLine($"[무작위 선택] '{selectedCards[0].Name}' 선택됨");
                        player.RandomChoice = false;
                    }
                    else
                    {
                        for (int p = 0; p < pickCount; p++)
                        {
                            int choice = -1;
                            while (true)
                            {
                                Console.Write($"카드를 선택하세요 (1~{cards.Count}): ");
                                string? input = Console.ReadLine();

                                if (int.TryParse(input, out choice) && choice >= 1 && choice <= cards.Count)
                                    break;

                                Console.WriteLine("잘못된 입력입니다. 다시 선택하세요.");
                            }

                            selectedCards.Add(cards[choice - 1]);
                            cards.RemoveAt(choice - 1);

                            if (cards.Count == 0)
                                break;
                        }
                    }

                    foreach (var selectedCard in selectedCards)
                    {
                        selectedCard.Apply(player, game, cards);
                        player.LastCard = selectedCard;
                    }
                }

                var newDelayed = new List<(int, Action)>();
                foreach (var (delay, effect) in player.DelayedEffects)
                {
                    if (delay > 0)
                        newDelayed.Add((delay - 1, effect));
                    else
                    {
                        Console.WriteLine("[지연 효과 발동]");
                        effect();
                    }
                }
                player.DelayedEffects = newDelayed;

                if (player.NonCurseDamageTurn > 0)
                {
                    player.NonCurseDamageTurn--;
                }
                else if (player.Curse > 0)
                {
                    Console.WriteLine($"저주 데미지: {player.Curse}");
                    player.Hp -= player.Curse;
                }

                if (player.NotAddDeath > 0)
                {
                    player.NotAddDeath--;
                }
                else if (player.Curse >= 6)
                {
                    int deathAdd = player.Curse - 5;
                    Console.WriteLine($"죽음 카드 {deathAdd}장 덱에 추가");
                    game.InsertDeathCards(deathAdd);
                }

                if (game.Turn % REGULAR_CURSE_INCREASE_FREQ == 0)
                {
                    int inc = REGULAR_CURSE_INCREASE_INIT + (game.Turn / REGULAR_CURSE_INCREASE_FREQ - 1);
                    player.Curse += inc;
                    Console.WriteLine($"정기 저주 증가: +{inc}");
                }

                if (player.Ember && player.Hp <= 1)
                {
                    Console.WriteLine("[불씨 효과] 체력 1, 저주 0으로 변경");
                    player.Hp = 1;
                    player.Curse = 0;
                    player.Ember = false;
                }

                game.Turn++;
                Thread.Sleep(1000);
            }

            if (player.Hp <= 0)
            {
                Console.WriteLine("사망");
            }
            else
            {
                Console.WriteLine("생존! 승리!");
            }

            game.DisplayScore();
        }
    }
}
