using UnityEngine;
using CardGame;

public static class GameOverHandler
{
    public static void GameOver(Game game)
    {
        GameEvents.TriggerNegativeEffect("[게임 오버] 플레이어 사망");
        Debug.Log("[게임 오버] 플레이어 사망");

        // ✅ GameManager 통해 캔버스 전환
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowGameOver();
            Debug.Log("[GameOverHandler] GameManager 통해 게임 오버 화면 활성화");
        }
        else
        {
            Debug.LogError("[GameOverHandler] GameManager 인스턴스 없음!");
        }

        Debug.Log("게임 오버: 캔버스 전환 완료");
    }
}
