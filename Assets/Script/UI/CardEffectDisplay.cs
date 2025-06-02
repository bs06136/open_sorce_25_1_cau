using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class CardEffectDisplay : MonoBehaviour
{
    [Header("UI ������Ʈ")]
    [Tooltip("ȿ�� �ؽ�Ʈ�� ǥ���� Text ������Ʈ")]
    public TextMeshProUGUI effectText;

    [Tooltip("ȿ�� �г� (�ִϸ��̼ǿ�)")]
    public GameObject effectPanel;

    [Tooltip("��� �̹��� (���û���)")]
    public Image backgroundImage;

    [Header("�ִϸ��̼� ����")]
    [Tooltip("ȿ�� �ؽ�Ʈ ǥ�� �ð�")]
    public float displayDuration = 2.0f;

    [Tooltip("���̵� ��/�ƿ� �ð�")]
    public float fadeTime = 0.5f;

    [Tooltip("�ؽ�Ʈ �ִϸ��̼� ���")]
    public bool useTextAnimation = true;

    [Header("��Ÿ�� ����")]
    [Tooltip("�Ϲ� ȿ�� ����")]
    public Color normalEffectColor = Color.white;

    [Tooltip("������ ȿ�� ����")]
    public Color positiveEffectColor = Color.green;

    [Tooltip("������ ȿ�� ����")]
    public Color negativeEffectColor = Color.red;

    [Tooltip("Ư�� ȿ�� ����")]
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
        // CanvasGroup ������Ʈ Ȯ��/�߰�
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // �ʱ⿡�� ����
        SetVisible(false);

        // GameEvents�� ����
        if (GameEvents.OnCardEffectTriggered == null)
            GameEvents.OnCardEffectTriggered = ShowCardEffect;
    }

    /// <summary>
    /// ī�� ȿ���� ȭ�鿡 ǥ��
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
    /// ī�� ȿ�� ǥ�� �ڷ�ƾ
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
    /// ���� ȿ�� ǥ��
    /// </summary>
    private IEnumerator ShowSingleEffect(EffectInfo effectInfo)
    {
        // �ؽ�Ʈ ����
        if (effectText != null)
        {
            effectText.text = effectInfo.effectText;
            effectText.color = GetColorByType(effectInfo.effectType);
        }

        // �г� Ȱ��ȭ
        if (effectPanel != null)
            effectPanel.SetActive(true);

        // ���̵� ��
        yield return StartCoroutine(FadeIn());

        // �ؽ�Ʈ �ִϸ��̼� (���û���)
        if (useTextAnimation)
        {
            yield return StartCoroutine(TextPulseAnimation());
        }

        // ǥ�� �ð� ���
        yield return new WaitForSeconds(effectInfo.duration);

        // ���̵� �ƿ�
        yield return StartCoroutine(FadeOut());

        // �г� ��Ȱ��ȭ
        if (effectPanel != null)
            effectPanel.SetActive(false);
    }

    /// <summary>
    /// ���̵� �� �ִϸ��̼�
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
    /// ���̵� �ƿ� �ִϸ��̼�
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
    /// �ؽ�Ʈ �޽� �ִϸ��̼�
    /// </summary>
    private IEnumerator TextPulseAnimation()
    {
        if (effectText == null) yield break;

        Vector3 originalScale = effectText.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        float duration = 0.3f;
        float elapsedTime = 0f;

        // Ȯ��
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 1.1f, elapsedTime / duration);
            effectText.transform.localScale = originalScale * scale;
            yield return null;
        }

        elapsedTime = 0f;

        // ���
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
    /// ȿ�� Ÿ�Կ� ���� ���� ��ȯ
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
    /// ���� ����
    /// </summary>
    private void SetAlpha(float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }

    /// <summary>
    /// ǥ��/���� ����
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
    /// ��� ��� ȿ�� ǥ�� �ߴ�
    /// </summary>
    public void ClearAllEffects()
    {
        StopAllCoroutines();
        effectQueue.Clear();
        isDisplaying = false;
        SetVisible(false);
    }

    /// <summary>
    /// �׽�Ʈ�� �޼����
    /// </summary>
    [ContextMenu("Test Positive Effect")]
    public void TestPositiveEffect()
    {
        ShowCardEffect("[���� ȿ��] ü���� 5 �����մϴ�!", EffectType.Positive);
    }

    [ContextMenu("Test Negative Effect")]
    public void TestNegativeEffect()
    {
        ShowCardEffect("[���� ȿ��] ���ְ� 2 �����մϴ�!", EffectType.Negative);
    }

    [ContextMenu("Test Special Effect")]
    public void TestSpecialEffect()
    {
        ShowCardEffect("[������ ȿ��] 3�� �� ���� 3 ���� ����!", EffectType.Special);
    }
}