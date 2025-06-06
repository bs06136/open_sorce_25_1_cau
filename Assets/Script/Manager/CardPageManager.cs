using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame;

public class CardPageManager : MonoBehaviour
{
    [Header("카드 프리팹 & 부모 오브젝트")]
    public GameObject cardPrefab;
    public Transform cardContainer;

    [Header("페이지 이동 버튼")]
    public Button nextButton;
    public Button prevButton;

    private List<Card> cardDataList = new List<Card>();
    private List<GameObject> currentCards = new List<GameObject>();

    private int currentPage = 0;
    private int cardsPerPage = 1; // ✅ 한 장씩만 표시

    private void Start()
    {
        nextButton.onClick.AddListener(NextPage);
        prevButton.onClick.AddListener(PrevPage);

        LoadCardData();
        ShowPage(0);
    }

    // ✅ 카드 데이터 불러오기
    private void LoadCardData()
    {
        cardDataList = CardLibrary.AllCards;
        Debug.Log($"[CardPageManager] {cardDataList.Count}장의 카드 데이터 로드 완료.");
    }

    // ✅ 현재 카드들 삭제
    private void ClearCards()
    {
        foreach (var card in currentCards)
        {
            if (card != null)
                Destroy(card);
        }
        currentCards.Clear();
    }

    // ✅ 카드 1장 표시
    private void ShowPage(int pageIndex)
    {
        ClearCards();

        int startIndex = pageIndex * cardsPerPage;
        if (startIndex >= cardDataList.Count)
            return;

        Card card = cardDataList[startIndex];

        GameObject cardObj = Instantiate(cardPrefab, cardContainer);

        // ✅ CardImage 세팅
        Image cardImage = cardObj.transform.Find("CardImage").GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.sprite = GetCardSprite(card.Name);
        }
        else
        {
            Debug.LogError("[CardPageManager] ❌ CardImage 오브젝트를 찾을 수 없습니다!");
        }

        // ✅ 텍스트 세팅
        var hpText = cardObj.transform.Find("TextPanel/HpText").GetComponent<TextMeshProUGUI>();
        var curseText = cardObj.transform.Find("TextPanel/CurseText").GetComponent<TextMeshProUGUI>();
        var specialText = cardObj.transform.Find("TextPanel/SpecialText").GetComponent<TextMeshProUGUI>();

        if (hpText != null) hpText.text = GetHpText(card);
        if (curseText != null) curseText.text = GetCurseText(card);
        if (specialText != null) specialText.text = GetSpecialText(card);

        currentCards.Add(cardObj);

        currentPage = pageIndex;

        prevButton.gameObject.SetActive(currentPage > 0);
        nextButton.gameObject.SetActive((currentPage + 1) * cardsPerPage < cardDataList.Count);
    }

    // ✅ 카드 이름 기반 스프라이트 로드
    private Sprite GetCardSprite(string cardName)
    {
        int index = CardLibrary.AllCards.FindIndex(c => c.Name == cardName);
        if (index == -1)
        {
            Debug.LogError($"[CardPageManager] ❌ 카드 '{cardName}'의 인덱스를 찾을 수 없습니다!");
            return UnifiedCardManager.Instance.defaultCardSprite;
        }

        // 🔥 인덱스를 2자리 문자열(00, 01, 02, ...)로 포맷
        string resourcePath = $"Image/CardImage/{index:D2}";
        Debug.Log($"[CardPageManager] 📦 로딩 시도: {resourcePath}");

        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            Debug.Log($"[CardPageManager] ✅ 카드 스프라이트 로드 성공: {resourcePath}");
            return sprite;
        }
        else
        {
            Debug.LogError($"[CardPageManager] ❌ 카드 스프라이트 로드 실패: {resourcePath}");
            return UnifiedCardManager.Instance.defaultCardSprite;
        }
    }

    // ✅ 체력 변화 텍스트
    private string GetHpText(Card card)
    {
        if (card.HpChange > 0)
            return $"체력 +{card.HpChange}";
        else if (card.HpChange < 0)
            return $"체력 {card.HpChange}";
        else
            return "체력 변화 없음";
    }

    // ✅ 저주 변화 텍스트
    private string GetCurseText(Card card)
    {
        if (card.CurseChange > 0)
            return $"저주 +{card.CurseChange}";
        else if (card.CurseChange < 0)
            return $"저주 {card.CurseChange}";
        else
            return "저주 변화 없음";
    }

    // ✅ 특수 효과 텍스트
    private string GetSpecialText(Card card)
    {
        return string.IsNullOrEmpty(card.Description) ? "특수 효과 없음" : card.Description;
    }

    // ✅ 다음 카드
    public void NextPage()
    {
        if ((currentPage + 1) * cardsPerPage < cardDataList.Count)
        {
            ShowPage(currentPage + 1);
        }
    }

    // ✅ 이전 카드
    public void PrevPage()
    {
        if (currentPage > 0)
        {
            ShowPage(currentPage - 1);
        }
    }
}
