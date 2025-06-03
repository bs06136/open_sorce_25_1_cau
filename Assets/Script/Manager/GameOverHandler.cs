using UnityEngine;
using CardGame;

public static class GameOverHandler
{
    public static void GameOver(Game game)
    {
        GameEvents.TriggerNegativeEffect("[게임 오버] 플레이어 사망");
        Debug.Log("[게임 오버] 플레이어 사망");

        if (GameManager.Instance != null)
        {
            UnityEngine.Object.Destroy(GameManager.Instance.gameObject);
        }


        GameManager.Instance.ShowGameOver();
        Debug.Log("게임 오버: 캔버스 전환");
    }

}
