using UnityEngine;
using TMPro;

public class RemainingPicksUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI remainingPicksText;
    public GameObject remainingPicksPanel;

    private void Start()
    {
        Debug.Log("[RemainingPicksUI] Start() - 초기화");
        Hide();
    }

    public void UpdateRemainingPicks(int remaining, int total)
    {
        Debug.Log($"[RemainingPicksUI] UpdateRemainingPicks 호출됨 - remaining: {remaining}, total: {total}");

        if (total > 1)
        {
            string newText = $"남은 선택: {remaining}/{total}";
            Debug.Log($"[RemainingPicksUI] 텍스트 변경 시도: '{newText}'");
            SetTextAndShow(newText);
        }
        else
        {
            Debug.Log("[RemainingPicksUI] total <= 1이므로 UI 숨김");
            Hide();
        }
    }

    public void UpdateDrawnCards(int drawnCount)
    {
        Debug.Log($"[RemainingPicksUI] UpdateDrawnCards 호출됨 - drawnCount: {drawnCount}");

        if (drawnCount != 3)
        {
            string newText = $"뽑힌 카드: {drawnCount}장";
            Debug.Log($"[RemainingPicksUI] 텍스트 변경 시도: '{newText}'");
            SetTextAndShow(newText);
        }
        else
        {
            Debug.Log("[RemainingPicksUI] drawnCount == 3이므로 UI 숨김");
            Hide();
        }
    }

    private void SetTextAndShow(string text)
    {
        if (remainingPicksText == null)
        {
            Debug.LogError("[RemainingPicksUI] remainingPicksText가 null입니다!");
            return;
        }

        try
        {
            remainingPicksText.text = text;
            Debug.Log($"[RemainingPicksUI] 텍스트 설정 완료: '{remainingPicksText.text}'");

            // 강제로 텍스트 업데이트
            remainingPicksText.ForceMeshUpdate();

            Show();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[RemainingPicksUI] 텍스트 설정 오류: {e.Message}");
        }
    }

    private void Show()
    {
        Debug.Log("[RemainingPicksUI] Show() 호출됨");

        if (remainingPicksPanel != null)
        {
            remainingPicksPanel.SetActive(true);
            Debug.Log($"[RemainingPicksUI] Panel 활성화됨: {remainingPicksPanel.activeInHierarchy}");
        }
        else if (remainingPicksText != null)
        {
            remainingPicksText.gameObject.SetActive(true);
            Debug.Log($"[RemainingPicksUI] Text GameObject 활성화됨: {remainingPicksText.gameObject.activeInHierarchy}");
        }
    }

    public void Hide()
    {
        Debug.Log("[RemainingPicksUI] Hide() 호출됨");

        if (remainingPicksPanel != null)
        {
            remainingPicksPanel.SetActive(false);
            Debug.Log("[RemainingPicksUI] Panel 비활성화됨");
        }
        else if (remainingPicksText != null)
        {
            remainingPicksText.gameObject.SetActive(false);
            Debug.Log("[RemainingPicksUI] Text GameObject 비활성화됨");
        }
    }

    // Inspector에서 테스트 가능
    [ContextMenu("Test - 남은 선택")]
    public void TestRemainingPicks()
    {
        Debug.Log("[RemainingPicksUI] 테스트 - 남은 선택 2/2");
        UpdateRemainingPicks(2, 2);
    }

    [ContextMenu("Test - 뽑힌 카드")]
    public void TestDrawnCards()
    {
        Debug.Log("[RemainingPicksUI] 테스트 - 뽑힌 카드 2장");
        UpdateDrawnCards(2);
    }

    [ContextMenu("Test - 숨기기")]
    public void TestHide()
    {
        Debug.Log("[RemainingPicksUI] 테스트 - 숨기기");
        Hide();
    }
}