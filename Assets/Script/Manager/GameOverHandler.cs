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

        // 캔버스 전환
        var gameOverCanvas = GameObject.Find("GameOverCanvas");
        if (gameOverCanvas == null)
        {
            Debug.LogError("GameOverCanvas 오브젝트를 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log("GameOverCanvas 오브젝트를 성공적으로 찾음");
        }
        var mainMenuCanvas = GameObject.Find("MainMenu");
        var ingameCanvas = GameObject.Find("InGame");

        if (ingameCanvas == null)
        {
            Debug.LogError("InGame 오브젝트를 찾을 수 없습니다!");
        }
        else
        {
            Debug.Log("InGame 오브젝트를 성공적으로 찾음");
            ingameCanvas.SetActive(false);
        }

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);
        Debug.Log($"GameOverCanvas 활성화 상태: {gameOverCanvas.activeSelf}");
        if (mainMenuCanvas != null)
            mainMenuCanvas.SetActive(false);
        if (ingameCanvas != null)
            ingameCanvas.SetActive(false);

        Debug.Log("게임 오버: 캔버스 전환");
    }
}
