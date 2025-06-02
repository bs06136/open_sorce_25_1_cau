#nullable disable
using UnityEngine;
using CardGame;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI 컴포넌트")]
    public PlayerHP playerHpUI;
    public PlayerCurse playerCurseUI;
    public UnifiedCardManager unifiedCardManager;
    public TurnDisplay turnDisplay;

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
        if (playerHpUI == null)
            playerHpUI = FindFirstObjectByType<PlayerHP>();
        if (playerCurseUI == null)
            playerCurseUI = FindFirstObjectByType<PlayerCurse>();
        if (unifiedCardManager == null)
            unifiedCardManager = FindFirstObjectByType<UnifiedCardManager>();
        if (turnDisplay == null)
            turnDisplay = FindFirstObjectByType<TurnDisplay>();

        Debug.Log("✅ StartGame 실행됨");
        UnityPlayer = new PlayerBridge(playerHpUI, playerCurseUI);
        UnityGame = new Game(UnityPlayer);

        UnityPlayer.Hp = 10;
        UnityPlayer.Curse = 0;

        Debug.Log("GameManager: 게임 시작");
        GameEvents.TriggerPositiveEffect("🎮 게임이 시작되었습니다!");
        UpdateTurnDisplay();
        StartTurn();
    }

    public void StartTurn()
    {
        // 무한 재귀 방지를 위한 개선
        int skipCount = 0;
        while (UnityPlayer.SkipNextTurn && skipCount < 5)
        {
            GameEvents.TriggerNegativeEffect($"⏭️ 턴 {UnityGame.Turn} 스킵!");
            Debug.Log($"턴 {UnityGame.Turn} 스킵!");
            UnityPlayer.SkipNextTurn = false;
            UnityGame.Turn++;
            skipCount++;
        }

        if (skipCount >= 5)
        {
            GameEvents.TriggerNegativeEffect("⚠️ 턴 스킵이 너무 많이 발생했습니다!");
            Debug.LogError("턴 스킵이 너무 많이 발생했습니다!");
            return;
        }

        UpdateTurnDisplay();

        int drawCount = UnityPlayer.NextDrawNum;
        UnityPlayer.NextDrawNum = 3;

        var cards = UnityGame.DrawCards(drawCount);
        currentDrawnCards = cards;

        if (UnityPlayer.RandomChoice)
        {
            int randomIndex = UnityEngine.Random.Range(0, currentDrawnCards.Count);
            var selectedCard = currentDrawnCards[randomIndex];
            GameEvents.TriggerSpecialEffect($"🎲 [무작위 선택] '{selectedCard.Name}' 카드 선택!");
            Debug.Log($"[무작위 선택] {randomIndex}번 카드 선택");
            UnityPlayer.RandomChoice = false;
            ApplyCardByIndex(randomIndex);
            return;
        }

        unifiedCardManager.DisplayCards(cards);
        GameEvents.TriggerCardEffect($"🃏 카드 {cards.Count}장을 뽑았습니다");
    }

    private (List<int> drawnCards, List<int> HP, List<int> curse, List<string> text, int rerollCount) GetCardStatus()
    {
        if (currentDrawnCards == null || currentDrawnCards.Count == 0)
        {
            return (new List<int>(), new List<int>(), new List<int>(), new List<string>(), 0);
        }

        var cardIndices = currentDrawnCards
            .Select(c => {
                if (c == null) return -1;
                int index = CardLibrary.AllCards.IndexOf(c);
                return index >= 0 ? index : -1;
            })
            .ToList();

        var hpChanges = currentDrawnCards
            .Select(c => c.HpChange)
            .ToList();

        var curseChanges = currentDrawnCards
            .Select(c => c.CurseChange)
            .ToList();

        var descriptions = currentDrawnCards
            .Select(c => c.Description)
            .ToList();

        int rerollCount = UnityPlayer?.RerollAvailable ?? 0;

        return (cardIndices, hpChanges, curseChanges, descriptions, rerollCount);
    }

    private void ApplyCardByIndex(int index)
    {
        if (currentDrawnCards == null || index < 0 || index >= currentDrawnCards.Count)
        {
            GameEvents.TriggerNegativeEffect("❌ 잘못된 카드 선택!");
            return;
        }

        var selected = currentDrawnCards[index];
        currentDrawnCards.RemoveAt(index);

        // 대천사 효과 처리
        if (UnityPlayer.Archangel && selected.Name == "죽음")
        {
            var replacement = CardLibrary.AllCards.Find(c => c.Name != "죽음");
            if (replacement != null)
            {
                GameEvents.TriggerPositiveEffect($"👼 [대천사 효과] 죽음 카드가 '{replacement.Name}'로 교체됨!");
                Debug.Log($"[대천사 효과] 죽음 카드가 '{replacement.Name}'로 교체됨");
                selected = replacement;
            }
        }
        UnityPlayer.Archangel = false;

        GameEvents.TriggerCardEffect($"✨ '{selected.Name}' 카드를 선택했습니다!", EffectType.Special);
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
            GameEvents.TriggerNegativeEffect("💀 사망하였습니다...");
            Debug.Log("사망");
            return;
        }

        UnityGame.Turn++;
        UpdateTurnDisplay();
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
            if (UnityPlayer.Curse > 0)
            {
                GameEvents.TriggerPositiveEffect($"🛡️ 저주 피해 면역! (남은 턴: {UnityPlayer.NonCurseDamageTurn})");
            }
        }
        else if (UnityPlayer.Curse > 0)
        {
            int curseDamage = UnityPlayer.Curse;
            GameEvents.TriggerNegativeEffect($"💜 저주 데미지 {curseDamage} 받음!");
            Debug.Log($"저주 데미지: {curseDamage}");
            UnityPlayer.Hp -= curseDamage;
        }
    }

    private void HandleDeathCardInjection()
    {
        if (UnityPlayer.NotAddDeath > 0)
        {
            UnityPlayer.NotAddDeath--;
            if (UnityPlayer.Curse >= 6)
            {
                GameEvents.TriggerPositiveEffect($"🔒 죽음 카드 추가 금지! (남은 턴: {UnityPlayer.NotAddDeath})");
            }
        }
        else if (UnityPlayer.Curse >= 6)
        {
            int deathAdd = UnityPlayer.Curse - 5;
            GameEvents.TriggerNegativeEffect($"💀 죽음 카드 {deathAdd}장이 덱에 추가됨!");
            Debug.Log($"죽음 카드 {deathAdd}장 덱에 추가");
            UnityGame.InsertDeathCards(deathAdd);
        }
    }

    private void HandleCurseIncrease()
    {
        if (UnityGame.Turn % 5 == 0)
        {
            int inc = 1 + (UnityGame.Turn / 5 - 1);
            GameEvents.TriggerNegativeEffect($"⏰ 정기 저주 증가: +{inc}");
            Debug.Log($"정기 저주 증가: +{inc}");
            UnityPlayer.Curse += inc;
        }
    }

    private void HandleEmberEffect()
    {
        if (UnityPlayer.Ember && UnityPlayer.Hp <= 1)
        {
            GameEvents.TriggerSpecialEffect("🔥 [불씨 효과] 체력 1, 저주 0으로 회복!");
            Debug.Log("[불씨 효과] 체력 1, 저주 0으로 변경");
            UnityPlayer.Hp = 1;
            UnityPlayer.Curse = 0;
            UnityPlayer.Ember = false; // 불씨 효과는 한 번만 발동
        }
    }

    /// <summary>
    /// 턴 표시 UI 업데이트
    /// </summary>
    private void UpdateTurnDisplay()
    {
        if (turnDisplay != null)
        {
            turnDisplay.ForceUpdateTurn();
        }
    }

    /// <summary>
    /// 현재 턴 정보 가져오기 (외부에서 사용 가능)
    /// </summary>
    public int GetCurrentTurn()
    {
        return UnityGame?.Turn ?? 0;
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        GameEvents.TriggerCardEffect("🔄 게임을 재시작합니다!");
        StartGame();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void EndGame()
    {
        GameEvents.TriggerNegativeEffect("🏁 게임이 종료되었습니다!");
        Debug.Log("게임 종료");
    }
}