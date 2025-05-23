using UnityEngine;
using TMPro; // 用于 TextMeshPro

public class DifficultySelector : MonoBehaviour
{
    [Header("UI 显示组件")]
    public TextMeshProUGUI difficultyDisplay;

    private string[] difficultyLevels = { "Easy", "Normal", "Hard" };
    private int currentIndex = 1; // 初始为 Normal

    public static string SelectedDifficulty { get; private set; } = "Normal";

    void Start()
    {
        UpdateDifficultyUI();
    }

    public void OnDifficultyClick()
    {
        currentIndex = (currentIndex + 1) % difficultyLevels.Length;
        SelectedDifficulty = difficultyLevels[currentIndex];
        UpdateDifficultyUI();
    }

    private void UpdateDifficultyUI()
    {
        if (difficultyDisplay != null)
        {
            difficultyDisplay.text = SelectedDifficulty;
        }
    }
}
