using UnityEngine;
using UnityEngine.UI;

public class ExitPopupManager : MonoBehaviour
{
    public GameObject exitConfirmationPanel;  // 用来显示弹窗的面板
    public Button confirmExitButton;          // 确认退出按钮
    public Button cancelExitButton;           // 取消退出按钮

    void Start()
    {
        // 隐藏弹窗
        exitConfirmationPanel.SetActive(false);

        // 为按钮添加点击事件
        confirmExitButton.onClick.AddListener(OnConfirmExit);
        cancelExitButton.onClick.AddListener(OnCancelExit);
    }

    // 显示退出确认弹窗
    public void ShowExitConfirmation()
    {
        exitConfirmationPanel.SetActive(true);
    }

    // 点击确认退出
    private void OnConfirmExit()
    {
        Application.Quit();  // 退出游戏
        Debug.Log("Exit confirmed!");
    }

    // 点击取消退出
    private void OnCancelExit()
    {
        exitConfirmationPanel.SetActive(false);  // 关闭弹窗
    }
}
