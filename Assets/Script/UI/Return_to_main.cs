using UnityEngine;
using UnityEngine.SceneManagement;

public class Return_to_main : MonoBehaviour
{
    public GameObject MainMenuCanvas;
    public GameObject GameOverCanvas;

    public void backToMainMenu()
    {
        //MainMenuCanvas.SetActive(true);
        //GameOverCanvas.SetActive(false);
        // }

        // public void RetryGame()
        // {
        SceneManager.LoadScene("SampleScene");
    }
}
