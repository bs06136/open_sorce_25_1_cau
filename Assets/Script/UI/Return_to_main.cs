using UnityEngine;

public class Return_to_main : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameOverCanvas;

    public void ShowMainMenu()
    {
        MainMenuCanvas.SetActive(true);
        GameOverCanvas.SetActive(false);
    }
}
