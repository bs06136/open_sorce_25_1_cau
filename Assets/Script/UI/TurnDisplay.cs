using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class TurnDisplay : MonoBehaviour
{
    [Header("턴 표시 UI")]
    [Tooltip("턴 숫자를 표시할 Text 컴포넌트 (UI > Text 사용)")]
    public Text turnText;

    [Tooltip("턴 숫자를 표시할 TextMeshPro 컴포넌트 (더 좋은 텍스트 품질)")]
    public TextMeshProUGUI turnTextMeshPro;

    [Header("표시 옵션")]
    [Tooltip("턴 텍스트 앞에 표시할 접두사")]
    public string turnPrefix = "턴 ";

    [Tooltip("턴이 변경될 때 애니메이션 효과")]
    public bool enableTurnAnimation = true;

    [Tooltip("턴 변경 애니메이션 지속시간")]
    public float animationDuration = 0.5f;

    private int currentDisplayedTurn = 0;
    private Vector3 originalScale;

    private void Start()
    {
        // 초기 스케일 저장 (애니메이션용)
        originalScale = transform.localScale;

        // 게임 시작 시 턴 표시
        UpdateTurnDisplay();
    }

    private void Update()
    {
        // 매 프레임마다 턴 변화 체크 (성능상 좋지 않지만 간단함)
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
    /// 턴 표시 업데이트
    /// </summary>
    public void UpdateTurnDisplay()
    {
        if (GameManager.Instance == null || GameManager.Instance.UnityGame == null)
        {
            SetTurnText("게임 준비중...");
            return;
        }

        int currentTurn = GameManager.Instance.UnityGame.Turn;
        string turnDisplayText = $"{turnPrefix}{currentTurn}";

        SetTurnText(turnDisplayText);
        Debug.Log($"[TurnDisplay] 턴 표시 업데이트: {currentTurn}");
    }

    /// <summary>
    /// 텍스트 설정 (Text 또는 TextMeshPro 자동 선택)
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
            Debug.LogWarning("[TurnDisplay] 턴 표시용 Text 컴포넌트가 설정되지 않았습니다!");
        }
    }

    /// <summary>
    /// 턴 변경 시 애니메이션 효과 (Unity 기본 Coroutine 사용)
    /// </summary>
    private void PlayTurnChangeAnimation()
    {
        if (!enableTurnAnimation) return;

        // 이전 애니메이션 중단
        StopAllCoroutines();

        // 새 애니메이션 시작
        StartCoroutine(TurnChangeAnimationCoroutine());
    }

    /// <summary>
    /// 턴 변경 애니메이션 코루틴
    /// </summary>
    private System.Collections.IEnumerator TurnChangeAnimationCoroutine()
    {
        float expandTime = animationDuration * 0.3f;
        float shrinkTime = animationDuration * 0.7f;
        Vector3 targetScale = originalScale * 1.2f;

        // 확대 애니메이션
        float elapsedTime = 0f;
        while (elapsedTime < expandTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / expandTime;

            // EaseOutBack 효과 (간단한 버전)
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, easeProgress);

            yield return null;
        }

        // 축소 애니메이션
        elapsedTime = 0f;
        while (elapsedTime < shrinkTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / shrinkTime;

            // EaseOutBack 효과
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            transform.localScale = Vector3.Lerp(targetScale, originalScale, easeProgress);

            yield return null;
        }

        // 최종 스케일 보정
        transform.localScale = originalScale;
    }

    /// <summary>
    /// 외부에서 턴 표시를 강제로 업데이트할 때 사용
    /// </summary>
    public void ForceUpdateTurn()
    {
        UpdateTurnDisplay();
    }

    /// <summary>
    /// 특정 턴을 직접 표시 (테스트용)
    /// </summary>
    public void SetTurn(int turn)
    {
        string turnDisplayText = $"{turnPrefix}{turn}";
        SetTurnText(turnDisplayText);
    }

    // 에디터에서 테스트할 수 있는 컨텍스트 메뉴
    [ContextMenu("Test Turn Display")]
    private void TestTurnDisplay()
    {
        SetTurn(Random.Range(1, 100));
    }
}