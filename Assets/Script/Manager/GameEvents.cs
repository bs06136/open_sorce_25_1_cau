#nullable disable
using System;
using System.Collections.Generic;

/// <summary>
/// 카드 효과 타입 열거형
/// </summary>
public enum EffectType
{
    Normal,
    Positive,
    Negative,
    Special
}

public static class GameEvents
{
    // 카드 상태 요청 (UI → 게임 로직)
    public static System.Func<(List<int> drawnCards, List<int> HP, List<int> curse, List<string> text, int rerollCount)> OnCardStatusRequested;

    // 카드 선택 (UI → 게임 로직)
    public static Action<int> OnCardChosen;

    // 카드 효과 표시 이벤트 추가
    public static Action<string, EffectType> OnCardEffectTriggered;

    /// <summary>
    /// 카드 효과 트리거 (간편 메서드)
    /// </summary>
    public static void TriggerCardEffect(string effectText, EffectType effectType = EffectType.Normal)
    {
        OnCardEffectTriggered?.Invoke(effectText, effectType);
    }

    /// <summary>
    /// 긍정적 효과 트리거
    /// </summary>
    public static void TriggerPositiveEffect(string effectText)
    {
        TriggerCardEffect(effectText, EffectType.Positive);
    }

    /// <summary>
    /// 부정적 효과 트리거
    /// </summary>
    public static void TriggerNegativeEffect(string effectText)
    {
        TriggerCardEffect(effectText, EffectType.Negative);
    }

    /// <summary>
    /// 특수 효과 트리거
    /// </summary>
    public static void TriggerSpecialEffect(string effectText)
    {
        TriggerCardEffect(effectText, EffectType.Special);
    }
}