 using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SoundSettingsManager : MonoBehaviour
{
    [Header("UI 显示组件")]
    public GameObject soundPanel;
    public Button muteButton;
    public Button volumeButton;
    public TextMeshProUGUI muteButtonText;
    public TextMeshProUGUI settingStatusText;

    private bool isMuted = false;

    void Start()
    {
        muteButton.onClick.AddListener(OnMuteButtonClicked);
        volumeButton.onClick.AddListener(OnVolumeButtonClicked);

        // 默认隐藏提示文本
        settingStatusText.gameObject.SetActive(false);

        UpdateButtonText();
    }

    private void OnMuteButtonClicked()
    {
        Debug.Log("Mute button clicked");

        isMuted = !isMuted;

        if (isMuted)
        {
            AudioListener.volume = 0f;
            ShowStatus("已设置为静音");
        }
        else
        {
            AudioListener.volume = 1f;
            ShowStatus("音量已恢复");
        }

        UpdateButtonText();
        HideSoundPanel();
    }

    private void OnVolumeButtonClicked()
    {
        Debug.Log("Volume button clicked");
        ShowStatus("音量已调节");
        HideSoundPanel();
    }

    private void UpdateButtonText()
    {
        muteButtonText.text = isMuted ? "Unmute" : "Mute";
    }

    public void ShowSoundPanel()
    {
        soundPanel.SetActive(true);
    }

    public void HideSoundPanel()
    {
        soundPanel.SetActive(false);
    }

    // 新增：显示提示并自动隐藏
    private void ShowStatus(string message)
    {
        settingStatusText.text = message;
        settingStatusText.gameObject.SetActive(true);
        StopAllCoroutines(); // 确保不会堆叠多个隐藏协程
        StartCoroutine(HideStatusTextAfterDelay());
    }

    private IEnumerator HideStatusTextAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        settingStatusText.gameObject.SetActive(false);
    }
}

   
    