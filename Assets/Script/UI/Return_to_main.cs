using UnityEngine;
using UnityEngine.SceneManagement;

public class Return_to_main : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameOverCanvas;

    public void backToMainMenu()
    {
        // 씬 로드 대신 GameManager의 메서드 호출
        GameManager.Instance.ResetGameToMainMenu();
    }
}
