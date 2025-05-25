using UnityEngine;
using TMPro;

public class RemainingPicksUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI remainingPicksText;
    public GameObject remainingPicksPanel;

    private void Start()
    {
        Debug.Log("[RemainingPicksUI] Start() - �ʱ�ȭ");
        Hide();
    }

    public void UpdateRemainingPicks(int remaining, int total)
    {
        Debug.Log($"[RemainingPicksUI] UpdateRemainingPicks ȣ��� - remaining: {remaining}, total: {total}");

        if (total > 1)
        {
            string newText = $"���� ����: {remaining}/{total}";
            Debug.Log($"[RemainingPicksUI] �ؽ�Ʈ ���� �õ�: '{newText}'");
            SetTextAndShow(newText);
        }
        else
        {
            Debug.Log("[RemainingPicksUI] total <= 1�̹Ƿ� UI ����");
            Hide();
        }
    }

    public void UpdateDrawnCards(int drawnCount)
    {
        Debug.Log($"[RemainingPicksUI] UpdateDrawnCards ȣ��� - drawnCount: {drawnCount}");

        if (drawnCount != 3)
        {
            string newText = $"���� ī��: {drawnCount}��";
            Debug.Log($"[RemainingPicksUI] �ؽ�Ʈ ���� �õ�: '{newText}'");
            SetTextAndShow(newText);
        }
        else
        {
            Debug.Log("[RemainingPicksUI] drawnCount == 3�̹Ƿ� UI ����");
            Hide();
        }
    }

    private void SetTextAndShow(string text)
    {
        if (remainingPicksText == null)
        {
            Debug.LogError("[RemainingPicksUI] remainingPicksText�� null�Դϴ�!");
            return;
        }

        try
        {
            remainingPicksText.text = text;
            Debug.Log($"[RemainingPicksUI] �ؽ�Ʈ ���� �Ϸ�: '{remainingPicksText.text}'");

            // ������ �ؽ�Ʈ ������Ʈ
            remainingPicksText.ForceMeshUpdate();

            Show();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[RemainingPicksUI] �ؽ�Ʈ ���� ����: {e.Message}");
        }
    }

    private void Show()
    {
        Debug.Log("[RemainingPicksUI] Show() ȣ���");

        if (remainingPicksPanel != null)
        {
            remainingPicksPanel.SetActive(true);
            Debug.Log($"[RemainingPicksUI] Panel Ȱ��ȭ��: {remainingPicksPanel.activeInHierarchy}");
        }
        else if (remainingPicksText != null)
        {
            remainingPicksText.gameObject.SetActive(true);
            Debug.Log($"[RemainingPicksUI] Text GameObject Ȱ��ȭ��: {remainingPicksText.gameObject.activeInHierarchy}");
        }
    }

    public void Hide()
    {
        Debug.Log("[RemainingPicksUI] Hide() ȣ���");

        if (remainingPicksPanel != null)
        {
            remainingPicksPanel.SetActive(false);
            Debug.Log("[RemainingPicksUI] Panel ��Ȱ��ȭ��");
        }
        else if (remainingPicksText != null)
        {
            remainingPicksText.gameObject.SetActive(false);
            Debug.Log("[RemainingPicksUI] Text GameObject ��Ȱ��ȭ��");
        }
    }

    // Inspector���� �׽�Ʈ ����
    [ContextMenu("Test - ���� ����")]
    public void TestRemainingPicks()
    {
        Debug.Log("[RemainingPicksUI] �׽�Ʈ - ���� ���� 2/2");
        UpdateRemainingPicks(2, 2);
    }

    [ContextMenu("Test - ���� ī��")]
    public void TestDrawnCards()
    {
        Debug.Log("[RemainingPicksUI] �׽�Ʈ - ���� ī�� 2��");
        UpdateDrawnCards(2);
    }

    [ContextMenu("Test - �����")]
    public void TestHide()
    {
        Debug.Log("[RemainingPicksUI] �׽�Ʈ - �����");
        Hide();
    }
}