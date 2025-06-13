using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame;
using System.Collections;

public class CardStoryDisplay : MonoBehaviour
{
    public static CardStoryDisplay Instance { get; private set; }

    [Header("=== 스토리 UI 컴포넌트 ===")]
    [Tooltip("스토리가 표시될 패널")]
    public GameObject storyPanel;

    [Tooltip("스토리 텍스트를 표시할 TextMeshPro")]
    public TextMeshProUGUI storyText;

    [Tooltip("카드 이름을 표시할 텍스트 (선택사항)")]
    public TextMeshProUGUI cardNameText;

    [Header("=== 애니메이션 설정 ===")]
    [Tooltip("스토리 표시 애니메이션 사용 여부")]
    public bool useAnimation = true;

    [Tooltip("페이드 인 지속 시간")]
    public float fadeInDuration = 0.5f;

    [Tooltip("스토리 자동 숨김 시간 (0 = 자동 숨김 안함)")]
    public float autoHideDelay = 5f;

    [Header("=== 스타일 설정 ===")]
    [Tooltip("스토리 텍스트 색상")]
    public Color storyTextColor = Color.white;

    [Tooltip("카드 이름 텍스트 색상")]
    public Color cardNameColor = new Color(1f, 0.8f, 0.2f, 1f); // 황금색

    // 내부 변수들
    private Coroutine currentStoryCoroutine;
    private CanvasGroup storyPanelCanvasGroup;
    private string currentCardName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // CanvasGroup 컴포넌트 확인/추가
        if (storyPanel != null)
        {
            storyPanelCanvasGroup = storyPanel.GetComponent<CanvasGroup>();
            if (storyPanelCanvasGroup == null)
            {
                storyPanelCanvasGroup = storyPanel.AddComponent<CanvasGroup>();
            }
        }

        // 초기 상태: 스토리 패널 숨김
        HideStoryImmediate();
    }

    private void Start()
    {
        // 🔥 이벤트 구독 제거 - CardButtonHandler에서 직접 호출하도록 변경
        // GameEvents.OnCardChosen += OnCardChosen;

        // 초기 텍스트 색상 설정
        if (storyText != null)
            storyText.color = storyTextColor;
        if (cardNameText != null)
            cardNameText.color = cardNameColor;

        Debug.Log("[CardStoryDisplay] 초기화 완료 - 이벤트 구독 없이 직접 호출 방식 사용");
    }

    private void OnDestroy()
    {
        // 🔥 이벤트 구독 해제도 제거 (구독 안 했으므로)
        // GameEvents.OnCardChosen -= OnCardChosen;

        // 실행 중인 코루틴 정리
        if (currentStoryCoroutine != null)
        {
            StopCoroutine(currentStoryCoroutine);
        }
    }

    /// <summary>
    /// 특정 카드의 스토리 표시 (외부에서 직접 호출)
    /// </summary>
    /// <param name="cardName">카드 이름</param>
    public void ShowCardStory(string cardName)
    {
        if (string.IsNullOrEmpty(cardName))
        {
            Debug.LogWarning("[CardStoryDisplay] 카드 이름이 비어있습니다.");
            return;
        }

        // 기존 스토리 표시 중단
        if (currentStoryCoroutine != null)
        {
            StopCoroutine(currentStoryCoroutine);
        }

        currentCardName = cardName;

        // 스토리 가져오기
        string story = CardStoryLibrary.GetRandomStory(cardName);

        Debug.Log($"[CardStoryDisplay] '{cardName}' 카드 스토리 표시: {story}");

        // 스토리 표시 코루틴 시작
        currentStoryCoroutine = StartCoroutine(DisplayStoryCoroutine(cardName, story));
    }

    /// <summary>
    /// 스토리 표시 코루틴
    /// </summary>
    private IEnumerator DisplayStoryCoroutine(string cardName, string story)
    {
        // UI 텍스트 설정
        if (cardNameText != null)
        {
            cardNameText.text = cardName;
        }

        if (storyText != null)
        {
            storyText.text = story;
        }

        // 애니메이션 또는 즉시 표시
        if (useAnimation && storyPanelCanvasGroup != null)
        {
            yield return StartCoroutine(FadeInStory());
        }
        else
        {
            ShowStoryImmediate();
        }

        // 자동 숨김 설정된 경우 대기 후 숨김
        if (autoHideDelay > 0)
        {
            yield return new WaitForSeconds(autoHideDelay);

            if (useAnimation && storyPanelCanvasGroup != null)
            {
                yield return StartCoroutine(FadeOutStory());
            }
            else
            {
                HideStoryImmediate();
            }
        }

        currentStoryCoroutine = null;
    }

    /// <summary>
    /// 스토리 페이드 인 애니메이션
    /// </summary>
    private IEnumerator FadeInStory()
    {
        if (storyPanelCanvasGroup == null) yield break;

        storyPanel.SetActive(true);
        storyPanelCanvasGroup.alpha = 0f;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / fadeInDuration);
            storyPanelCanvasGroup.alpha = alpha;
            yield return null;
        }

        storyPanelCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 스토리 페이드 아웃 애니메이션
    /// </summary>
    private IEnumerator FadeOutStory()
    {
        if (storyPanelCanvasGroup == null) yield break;

        float elapsedTime = 0f;
        float startAlpha = storyPanelCanvasGroup.alpha;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / fadeInDuration);
            storyPanelCanvasGroup.alpha = alpha;
            yield return null;
        }

        storyPanelCanvasGroup.alpha = 0f;
        storyPanel.SetActive(false);
    }

    /// <summary>
    /// 스토리 즉시 표시
    /// </summary>
    private void ShowStoryImmediate()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(true);
        }

        if (storyPanelCanvasGroup != null)
        {
            storyPanelCanvasGroup.alpha = 1f;
        }
    }

    /// <summary>
    /// 스토리 즉시 숨김
    /// </summary>
    private void HideStoryImmediate()
    {
        if (storyPanel != null)
        {
            storyPanel.SetActive(false);
        }

        if (storyPanelCanvasGroup != null)
        {
            storyPanelCanvasGroup.alpha = 0f;
        }
    }

    /// <summary>
    /// 수동으로 스토리 숨기기
    /// </summary>
    public void HideStory()
    {
        if (currentStoryCoroutine != null)
        {
            StopCoroutine(currentStoryCoroutine);
            currentStoryCoroutine = null;
        }

        if (useAnimation && storyPanelCanvasGroup != null)
        {
            StartCoroutine(FadeOutStory());
        }
        else
        {
            HideStoryImmediate();
        }
    }

    /// <summary>
    /// 테스트용 메서드 - 특정 카드 스토리 강제 표시
    /// </summary>
    [ContextMenu("Test Story Display")]
    public void TestStoryDisplay()
    {
        ShowCardStory("바보");
    }

    /// <summary>
    /// 현재 표시 중인 카드 이름 반환
    /// </summary>
    public string GetCurrentCardName()
    {
        return currentCardName;
    }

    /// <summary>
    /// 스토리 표시 중인지 확인
    /// </summary>
    public bool IsShowingStory()
    {
        return storyPanel != null && storyPanel.activeSelf;
    }
}