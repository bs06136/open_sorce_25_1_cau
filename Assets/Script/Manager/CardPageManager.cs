using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPageManager : MonoBehaviour
{
    [Header("카드 프리팹 & 부모 오브젝트")]
    public GameObject cardPrefab;        // 카드 프리팹
    public Transform cardContainer;      // 카드들을 배치할 부모 (Grid Layout Group)

    [Header("페이지 이동 버튼")]
    public Button nextButton;            // 다음 페이지 버튼
    public Button prevButton;            // 이전 페이지 버튼

    private List<Sprite> cardSprites = new List<Sprite>(); // 전체 카드 스프라이트 리스트
    private List<GameObject> currentCards = new List<GameObject>(); // 현재 화면에 생성된 카드 오브젝트들

    private int currentPage = 0;
    private int cardsPerPage = 6;

    private void Start()
    {
        // 버튼 클릭 이벤트 연결
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);

        // 카드 이미지 로드
        LoadCardSprites();

        // 첫 페이지 표시
        ShowPage(0);
    }

    private void LoadCardSprites()
    {
        // Resources 폴더 기준 경로
        Sprite[] loadedSprites = Resources.LoadAll<Sprite>("Image/CardImage");

        foreach (var sprite in loadedSprites)
        {
            cardSprites.Add(sprite);
        }

        Debug.Log($"[CardPageManager] {cardSprites.Count}장의 카드 스프라이트가 로드되었습니다.");
    }

    private void ClearCards()
    {
        // 현재 화면에 있는 카드들 삭제
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
            cardImage.sprite = cardSprites[i]; // 스프라이트 할당

            currentCards.Add(cardObj);
        }

        currentPage = pageIndex;

        // 버튼 활성화/비활성화 설정
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
