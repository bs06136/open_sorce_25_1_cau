using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardEffectDisplay : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [Tooltip("효과 텍스트를 표시할 Text 컴포넌트")]
    public TextMeshProUGUI effectText;

    [Tooltip("효과 패널 (애니메이션용)")]
    public GameObject effectPanel;

    [Tooltip("배경 이미지 (선택사항)")]
    public Image backgroundImage;

    [Header("애니메이션 설정")]
    [Tooltip("효과 텍스트 표시 시간")]
    public float displayDuration = 2.0f;

    [Tooltip("페이드 인/아웃 시간")]
    public float fadeTime = 0.5f;

    [Tooltip("텍스트 애니메이션 사용")]
    public bool useTextAnimation = true;

    [Header("스타일 설정")]
    [Tooltip("일반 효과 색상")]
    public Color normalEffectColor = Color.white;

    [Tooltip("긍정적 효과 색상")]
    public Color positiveEffectColor = Color.green;

    [Tooltip("부정적 효과 색상")]
    public Color negativeEffectColor = Color.red;

    [Tooltip("특수 효과 색상")]
    public Color specialEffectColor = Color.yellow;

    private Queue<EffectInfo> effectQueue = new Queue<EffectInfo>();
    private bool isDisplaying = false;
    private CanvasGroup canvasGroup;

    [System.Serializable]
    public class EffectInfo
    {
        public string effectText;
        public EffectType effectType;
        public float duration;

        public EffectInfo(string text, EffectType type, float duration = 2.0f)
        {
            this.effectText = text;
            this.effectType = type;
            this.duration = duration;
        }
    }

    private void Start()
    {
        // CanvasGroup 컴포넌트 확인/추가
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 초기에는 숨김
        SetVisible(false);

        // GameEvents에 연결
        if (GameEvents.OnCardEffectTriggered == null)
            GameEvents.OnCardEffectTriggered = ShowCardEffect;
    }

    /// <summary>
    /// 카드 효과를 화면에 표시
    /// </summary>
    public void ShowCardEffect(string effectDescription, EffectType type = EffectType.Normal)
    {
        var effectInfo = new EffectInfo(effectDescription, type, displayDuration);
        effectQueue.Enqueue(effectInfo);

        if (!isDisplaying)
        {
            StartCoroutine(DisplayEffectCoroutine());
        }
    }

    /// <summary>
    /// 카드 효과 표시 코루틴
    /// </summary>
    private IEnumerator DisplayEffectCoroutine()
    {
        isDisplaying = true;

        while (effectQueue.Count > 0)
        {
            var effectInfo = effectQueue.Dequeue();
            yield return StartCoroutine(ShowSingleEffect(effectInfo));
        }

        isDisplaying = false;
    }

    /// <summary>
    /// 단일 효과 표시
    /// </summary>
    private IEnumerator ShowSingleEffect(EffectInfo effectInfo)
    {
        // 텍스트 설정
        if (effectText != null)
        {
            effectText.text = effectInfo.effectText;
            effectText.color = GetColorByType(effectInfo.effectType);
        }

        // 패널 활성화
        if (effectPanel != null)
            effectPanel.SetActive(true);

        // 페이드 인
        yield return StartCoroutine(FadeIn());

        // 텍스트 애니메이션 (선택사항)
        if (useTextAnimation)
        {
            yield return StartCoroutine(TextPulseAnimation());
        }

        // 표시 시간 대기
        yield return new WaitForSeconds(effectInfo.duration);

        // 페이드 아웃
        yield return StartCoroutine(FadeOut());

        // 패널 비활성화
        if (effectPanel != null)
            effectPanel.SetActive(false);
    }

    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeTime);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(1f);
    }

    /// <summary>
    /// 페이드 아웃 애니메이션
    /// </summary>
    private IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeTime);
            SetAlpha(alpha);
            yield return null;
        }
        SetAlpha(0f);
    }

    /// <summary>
    /// 텍스트 펄스 애니메이션
    /// </summary>
    private IEnumerator TextPulseAnimation()
    {
        if (effectText == null) yield break;

        Vector3 originalScale = effectText.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        float duration = 0.3f;
        float elapsedTime = 0f;

        // 확대
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.1f, elapsedTime / duration);
            effectText.transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsedTime = 0f;

        // 축소
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1.1f, 1f, elapsedTime / duration);
            effectText.transform.localScale = originalScale * scale;
            yield return null;
        }

        effectText.transform.localScale = originalScale;
    }

    /// <summary>
    /// 효과 타입에 따른 색상 반환
    /// </summary>
    private Color GetColorByType(EffectType type)
    {
        switch (type)
        {
            case EffectType.Positive: return positiveEffectColor;
            case EffectType.Negative: return negativeEffectColor;
            case EffectType.Special: return specialEffectColor;
            default: return normalEffectColor;
        }
    }

    /// <summary>
    /// 투명도 설정
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }

    /// <summary>
    /// 표시/숨김 설정
    /// </summary>
    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }

        if (effectPanel != null)
            effectPanel.SetActive(visible);
    }

    /// <summary>
    /// 즉시 모든 효과 표시 중단
    /// </summary>
    public void ClearAllEffects()
    {
        StopAllCoroutines();
        effectQueue.Clear();
        isDisplaying = false;
        SetVisible(false);
    }

    /// <summary>
    /// 테스트용 메서드들
    /// </summary>
    [ContextMenu("Test Positive Effect")]
    public void TestPositiveEffect()
    {
        ShowCardEffect("[생명 효과] 체력이 5 증가합니다!", EffectType.Positive);
    }

    [ContextMenu("Test Negative Effect")]
    public void TestNegativeEffect()
    {
        ShowCardEffect("[저주 효과] 저주가 2 증가합니다!", EffectType.Negative);
    }

    [ContextMenu("Test Special Effect")]
    public void TestSpecialEffect()
    {
        ShowCardEffect("[마법사 효과] 3턴 후 저주 3 감소 예약!", EffectType.Special);
    }
}