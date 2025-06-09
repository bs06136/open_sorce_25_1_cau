using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TurnDisplay : MonoBehaviour
{
    [Header("�� ǥ�� UI")]
    [Tooltip("�� ���ڸ� ǥ���� Text ������Ʈ (UI > Text ���)")]
    public Text turnText;

    [Tooltip("�� ���ڸ� ǥ���� TextMeshPro ������Ʈ (�� ���� �ؽ�Ʈ ǰ��)")]
    public TextMeshProUGUI turnTextMeshPro;

    [Header("ǥ�� �ɼ�")]
    [Tooltip("�� �ؽ�Ʈ �տ� ǥ���� ���λ�")]
    public string turnSuffix;

    [Tooltip("���� ����� �� �ִϸ��̼� ȿ��")]
    public bool enableTurnAnimation = true;

    [Tooltip("�� ���� �ִϸ��̼� ���ӽð�")]
    public float animationDuration = 0.5f;

    private int currentDisplayedTurn = 0;
    private Vector3 originalScale;

    private void Start()
    {
        // �ʱ� ������ ���� (�ִϸ��̼ǿ�)
        originalScale = transform.localScale;

        // ���� ���� �� �� ǥ��
        UpdateTurnDisplay();
    }

    private void Update()
    {
        // �� �����Ӹ��� �� ��ȭ üũ (���ɻ� ���� ������ ������)
        if (GameManager.Instance != null && GameManager.Instance.UnityGame != null)
        {
            int currentTurn = GameManager.Instance.UnityGame.Turn;
            if (currentTurn != currentDisplayedTurn)
            {
                currentDisplayedTurn = currentTurn;
                UpdateTurnDisplay();

                if (enableTurnAnimation)
                {
                    PlayTurnChangeAnimation();
                }
            }
        }
    }

    /// <summary>
    /// �� ǥ�� ������Ʈ
    /// </summary>
    public void UpdateTurnDisplay()
    {
        if (GameManager.Instance == null || GameManager.Instance.UnityGame == null)
        {
            SetTurnText("���� �غ���...");
            return;
        }

        int currentTurn = GameManager.Instance.UnityGame.Turn;
        string turnDisplayText = $"{currentTurn}{turnSuffix}";

        SetTurnText(turnDisplayText);
        Debug.Log($"[TurnDisplay] �� ǥ�� ������Ʈ: {currentTurn}");
    }

    /// <summary>
    /// �ؽ�Ʈ ���� (Text �Ǵ� TextMeshPro �ڵ� ����)
    /// </summary>
    private void SetTurnText(string text)
    {
        if (turnTextMeshPro != null)
        {
            turnTextMeshPro.text = text;
        }
        else if (turnText != null)
        {
            turnText.text = text;
        }
        else
        {
            Debug.LogWarning("[TurnDisplay] �� ǥ�ÿ� Text ������Ʈ�� �������� �ʾҽ��ϴ�!");
        }
    }

    /// <summary>
    /// �� ���� �� �ִϸ��̼� ȿ�� (Unity �⺻ Coroutine ���)
    /// </summary>
    private void PlayTurnChangeAnimation()
    {
        if (!enableTurnAnimation) return;

        // ���� �ִϸ��̼� �ߴ�
        StopAllCoroutines();

        // �� �ִϸ��̼� ����
        StartCoroutine(TurnChangeAnimationCoroutine());
    }

    /// <summary>
    /// �� ���� �ִϸ��̼� �ڷ�ƾ
    /// </summary>
    private System.Collections.IEnumerator TurnChangeAnimationCoroutine()
    {
        float expandTime = animationDuration * 0.3f;
        float shrinkTime = animationDuration * 0.7f;
        Vector3 targetScale = originalScale * 1.2f;

        // Ȯ�� �ִϸ��̼�
        float elapsedTime = 0f;
        while (elapsedTime < expandTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / expandTime;

            // EaseOutBack ȿ�� (������ ����)
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, easeProgress);

            yield return null;
        }

        // ��� �ִϸ��̼�
        elapsedTime = 0f;
        while (elapsedTime < shrinkTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / shrinkTime;

            // EaseOutBack ȿ��
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, easeProgress);

            yield return null;
        }

        // ���� ������ ����
        transform.localScale = originalScale;
    }

    /// <summary>
    /// �ܺο��� �� ǥ�ø� ������ ������Ʈ�� �� ���
    /// </summary>
    public void ForceUpdateTurn()
    {
        UpdateTurnDisplay();
    }

    /// <summary>
    /// Ư�� ���� ���� ǥ�� (�׽�Ʈ��)
    /// </summary>
    public void SetTurn(int turn)
    {
        string turnDisplayText = $"{turn}{turnSuffix}";
        SetTurnText(turnDisplayText);
    }

    // �����Ϳ��� �׽�Ʈ�� �� �ִ� ���ؽ�Ʈ �޴�
    [ContextMenu("Test Turn Display")]
    private void TestTurnDisplay()
    {
        SetTurn(Random.Range(1, 100));
    }
}