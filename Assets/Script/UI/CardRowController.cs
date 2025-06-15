using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class CardRowController : MonoBehaviour, IPointerClickHandler
{
    [Header("面板绑定 (Drag & Drop)")]
    public GameObject FrontPanel;      // 整个前面UI的空物件
    public GameObject BackPanel;       // 整个背面UI的空物件（只放故事文本）

    public GameObject EffectSpace;

    [Header("Story 文本 (位于 BackPanel 里面)")]
    public TextMeshProUGUI StoryText;

    // 内部状态
    private bool isShowingFront = true;
    private string storyContent;      // 用于存储从外部传入的故事文本

    void Start()
    {
        // 启动时只显示前面
        FrontPanel.SetActive(true);
        BackPanel.SetActive(false);
    }

    /// <summary>
    /// 由 CardScrollManager 调用，注入每行对应的故事文本
    /// </summary>
    public void SetStory(string text)
    {
        storyContent = text;
        // 预先填入一次，确保翻过去也能立刻显示
        if (StoryText != null)
            StoryText.text = storyContent;
    }

    /// <summary>
    /// 点击整行时处理翻转动画并切换面板
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        // 防止重复点击打断动画
        if (LeanTween.isTweening(EffectSpace)) return;

        float halfDuration = 0.25f;
        // 先绕本地 Y 轴旋转 90°
        LeanTween.rotateY(EffectSpace, 90f, halfDuration)
            .setOnComplete(OnHalfFlipped)
            .setEase(LeanTweenType.easeInOutQuad);
    }

    private void OnHalfFlipped()
    {
        // 切换正反面
        isShowingFront = !isShowingFront;
        FrontPanel.SetActive(isShowingFront);
        BackPanel.SetActive(!isShowingFront);

        // 如果是背面，刷新一次文本（确保最新）
        if (!isShowingFront && StoryText != null)
            StoryText.text = storyContent;

        // 再反向旋转回 0°
        LeanTween.rotateY(EffectSpace, 0f, 0.25f)
            .setEase(LeanTweenType.easeInOutQuad);
    }
}
