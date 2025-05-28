using UnityEngine;
using CardGame;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PlayerHP playerHpUI;
    public PlayerCurse playerCurseUI;
    public CardManager cardManager;

    public Game UnityGame { get; private set; }
    public PlayerBridge UnityPlayer { get; private set; }

    private List<Card> currentDrawnCards = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        GameEvents.OnCardStatusRequested = GetCardStatus;
        GameEvents.OnCardChosen = ApplyCardByIndex;
    }

    public void StartGame()
    {
        Debug.Log("✅ StartGame 실행됨");  
        UnityPlayer = new PlayerBridge(playerHpUI, playerCurseUI);
        UnityGame = new Game(UnityPlayer);

        UnityPlayer.Hp = 10;
        UnityPlayer.Curse = 0;

        Debug.Log("GameManager: 게임 시작");
        StartTurn();
    }

    public void StartTurn()
    {
        if (UnityPlayer.SkipNextTurn)
        {
            Debug.Log("이번 턴은 스킵됩니다.");
            UnityPlayer.SkipNextTurn = false;
            UnityGame.Turn++;
            StartTurn();
            return;
        }

        int drawCount = UnityPlayer.NextDrawNum;
        UnityPlayer.NextDrawNum = 3;

        var cards = UnityGame.DrawCards(drawCount);
        currentDrawnCards = cards;

        if (UnityPlayer.RandomChoice)
        {
            int randomIndex = UnityEngine.Random.Range(0, currentDrawnCards.Count);
            Debug.Log($"[무작의 선택] {randomIndex}번 카드 선택");
            UnityPlayer.RandomChoice = false;
            ApplyCardByIndex(randomIndex);
            return;
        }

        cardManager.DisplayCards(cards);
    }

    private (List<int>, int) GetCardStatus()
    {
        var cardIndices = currentDrawnCards
            .Select(c => CardLibrary.AllCards.IndexOf(c))
            .ToList();

        return (cardIndices, UnityPlayer.RerollAvailable);
    }

    private void ApplyCardByIndex(int index)
    {
        if (index < 0 || index >= currentDrawnCards.Count) return;

        var selected = currentDrawnCards[index];
        currentDrawnCards.RemoveAt(index);

        if (UnityPlayer.Archangel && selected.Name == "죽음")
        {
            selected = CardLibrary.AllCards.Find(c => c.Name != "죽음");
            Debug.Log($"[대천사 효과] 죽음 카드가 '{selected.Name}'로 교체됨");
        }
        UnityPlayer.Archangel = false;

        ApplyCard(selected, currentDrawnCards);
    }

    private void ApplyCard(Card selectedCard, List<Card> remainingCards)
    {
        selectedCard.Apply(UnityPlayer, UnityGame, remainingCards);
        UnityPlayer.LastCard = selectedCard;

        Debug.Log($"[지연 효과 개수] {UnityPlayer.DelayedEffects.Count}");
        HandleDelayedEffects();
        HandleCurseDamage();
        HandleDeathCardInjection();
        HandleCurseIncrease();
        HandleEmberEffect();

        if (UnityPlayer.Hp <= 0)
        {
            Debug.Log("사망");
            return;
        }

        UnityGame.Turn++;
        StartTurn();
    }

    private void HandleDelayedEffects()
    {
        var newList = new List<(int, Action)>();
        foreach (var (delay, effect) in UnityPlayer.DelayedEffects)
        {
            Debug.Log($"[지연 효과 검사] delay: {delay}");
            if (delay > 0)
                newList.Add((delay - 1, effect));
            else
            {
                Debug.Log("[지연 효과 발동]");
                effect();
            }
        }
        UnityPlayer.DelayedEffects = newList;
    }

    private void HandleCurseDamage()
    {
        if (UnityPlayer.NonCurseDamageTurn > 0)
        {
            UnityPlayer.NonCurseDamageTurn--;
        }
        else if (UnityPlayer.Curse > 0)
        {
            Debug.Log($"저주 데미지: {UnityPlayer.Curse}");
            UnityPlayer.Hp -= UnityPlayer.Curse;
            UnityPlayer.Hp = UnityPlayer.Hp; // UI 갱신
        }
    }

    private void HandleDeathCardInjection()
    {
        if (UnityPlayer.NotAddDeath > 0)
        {
            UnityPlayer.NotAddDeath--;
        }
        else if (UnityPlayer.Curse >= 6)
        {
            int deathAdd = UnityPlayer.Curse - 5;
            Debug.Log($"죽음 카드 {deathAdd}장 데크에 추가");
            UnityGame.InsertDeathCards(deathAdd);
        }
    }

    private void HandleCurseIncrease()
    {
        if (UnityGame.Turn % 5 == 0)
        {
            int inc = 1 + (UnityGame.Turn / 5 - 1);
            UnityPlayer.Curse += inc;
            UnityPlayer.Curse = UnityPlayer.Curse; // UI 갱신
            Debug.Log($"정기 저주 증가: +{inc}");
        }
    }

    private void HandleEmberEffect()
    {
        if (UnityPlayer.Ember && UnityPlayer.Hp <= 1)
        {
            Debug.Log("[불씨 효과] 체력 1, 저주 0으로 변경");
            UnityPlayer.Hp = 1;
            UnityPlayer.Curse = 0;
        }
    }
}
