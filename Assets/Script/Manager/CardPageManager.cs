using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPageManager : MonoBehaviour
{
    [Header("ī�� ������ & �θ� ������Ʈ")]
    public GameObject cardPrefab;        // ī�� ������
    public Transform cardContainer;      // ī����� ��ġ�� �θ� (Grid Layout Group)

    [Header("������ �̵� ��ư")]
    public Button nextButton;            // ���� ������ ��ư
    public Button prevButton;            // ���� ������ ��ư

    private List<Sprite> cardSprites = new List<Sprite>(); // ��ü ī�� ��������Ʈ ����Ʈ
    private List<GameObject> currentCards = new List<GameObject>(); // ���� ȭ�鿡 ������ ī�� ������Ʈ��

    private int currentPage = 0;
    private int cardsPerPage = 6;

    private void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ ����
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);

        // ī�� �̹��� �ε�
        LoadCardSprites();

        // ù ������ ǥ��
        ShowPage(0);
    }

    private void LoadCardSprites()
    {
        // Resources ���� ���� ���
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Image/CardImage");

        foreach (var sprite in loadedSprites)
        {
            cardSprites.Add(sprite);
        }

        Debug.Log($"[CardPageManager] {cardSprites.Count}���� ī�� ��������Ʈ�� �ε�Ǿ����ϴ�.");
    }

    private void ClearCards()
    {
        // ���� ȭ�鿡 �ִ� ī��� ����
        foreach (var card in currentCards)
        {
            Destroy(card);
        }
        currentCards.Clear();
    }

    private void ShowPage(int pageIndex)
    {
        ClearCards();

        int startIndex = pageIndex * cardsPerPage;
        int endIndex = Mathf.Min(startIndex + cardsPerPage, cardSprites.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject cardObj = Instantiate(cardPrefab, cardContainer);
            Image cardImage = cardObj.GetComponent<Image>();
            cardImage.sprite = cardSprites[i]; // ��������Ʈ �Ҵ�

            currentCards.Add(cardObj);
        }

        currentPage = pageIndex;

        // ��ư Ȱ��ȭ/��Ȱ��ȭ ����
        prevButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive(endIndex < cardSprites.Count);
    }

    public void NextPage()
    {
        if ((currentPage + 1) * cardsPerPage < cardSprites.Count)
        {
            ShowPage(currentPage + 1);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }
}
