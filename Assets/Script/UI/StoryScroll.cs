using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class StoryScroll : MonoBehaviour
{
    public TextMeshProUGUI storyText;
    public float scrollSpeed = 80.0f; // 스크롤 속도
    public float fadeInTime = 1.0f;

    private RectTransform textRectTransform;
    private bool isScrolling = false;

    void Start()
    {
        textRectTransform = storyText.GetComponent<RectTransform>();

        // 부모 RectTransform의 높이만큼 아래에서 시작
        RectTransform parentRect = storyText.transform.parent.GetComponent<RectTransform>();
        float parentHeight = parentRect != null ? parentRect.rect.height : Screen.height;
        textRectTransform.anchoredPosition = new Vector2(0, -parentHeight);

        Color textColor = storyText.color;
        storyText.color = new Color(textColor.r, textColor.g, textColor.b, 0);

    }

    public void StartScroll()
    {
        StartCoroutine(ScrollStory());
    }

    IEnumerator ScrollStory()
    {
        yield return StartCoroutine(FadeTextIn());

        isScrolling = true;

        RectTransform parentRect = storyText.transform.parent.GetComponent<RectTransform>();
        float parentHeight = parentRect != null ? parentRect.rect.height : Screen.height;

        while (isScrolling)
        {
            Vector2 position = textRectTransform.anchoredPosition;
            position.y += scrollSpeed * Time.deltaTime;
            textRectTransform.anchoredPosition = position;

            if (position.y >= 0)
            {
                isScrolling = false;
            }

            yield return null;
        }

        Debug.Log("스토리 소개 완료");
    }

    IEnumerator FadeTextIn()
    {
        float elapsedTime = 0;
        Color textColor = storyText.color;

        while (elapsedTime < fadeInTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInTime);
            storyText.color = new Color(textColor.r, textColor.g, textColor.b, alpha);
            yield return null;
        }
    }
}
