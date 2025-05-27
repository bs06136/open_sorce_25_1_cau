using System;
using System.Collections.Generic;

public static class GameEvents
{
    // 카드 상태 요청 (UI → 게임 로직)
    public static Func<(List<int> drawnCards, int rerollCount)>? OnCardStatusRequested;
    /*
    void Start()
    {
        var result = GameEvents.OnCardStatusRequested?.Invoke();
        if (result != null)
        {
            var (indices, reroll) = result.Value;
            Debug.Log($"현재 뽑은 카드: {string.Join(", ", indices)} / 리롤: {reroll}");
        }
    }*/
    //사용법


    // 카드 선택 (UI → 게임 로직)
    public static Action<int>? OnCardChosen;
}