using UnityEngine;
using TMPro; // 用于 TextMeshPro

public class LanguageSelector : MonoBehaviour
{
    [Header("UI 显示组件")]
    public TextMeshProUGUI languageDisplay;  // 显示当前语言的文本

    private string[] languages = { "English", "Chinese", "Spanish", "French", "German" }; // 语言列表
    private int currentIndex = 0; // 初始为 English

    public static string SelectedLanguage { get; private set; } = "English";  // 当前选择的语言，默认为英语

    void Start()
    {
        UpdateLanguageUI();  // 初始化时更新文本
    }

    // 点击按钮时切换语言
    public void OnLanguageClick()
    {
        currentIndex = (currentIndex + 1) % languages.Length;  // 切换语言
        SelectedLanguage = languages[currentIndex];  // 更新当前语言
        UpdateLanguageUI();  // 更新语言显示
    }

    // 更新语言显示文本
    private void UpdateLanguageUI()
    {
        if (languageDisplay != null)
        {
            languageDisplay.text = SelectedLanguage;  // 更新文本为当前选中的语言
        }
    }
}
