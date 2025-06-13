using UnityEngine;
using CardGame;

public class CareerSelectionHandler : MonoBehaviour
{
    // 각 버튼의 OnClick()에 이 메서드를 연결하세요.
    public void SelectExplorer()      => SelectCareer(CharacterType.Explorer);
    public void SelectGravekeeper()   => SelectCareer(CharacterType.Gravekeeper);
    public void SelectNecromancer()   => SelectCareer(CharacterType.Necromancer);
    public void SelectCleric()        => SelectCareer(CharacterType.Cleric);
    public void SelectGambler()       => SelectCareer(CharacterType.Gambler);
    public void SelectAvenger()       => SelectCareer(CharacterType.Avenger);
    public void SelectMerchant()      => SelectCareer(CharacterType.Merchant);
    public void SelectDemonBinder()   => SelectCareer(CharacterType.DemonBinder);

    private void SelectCareer(CharacterType type)
    {
        // 1) 저장
        PlayerPrefs.SetInt("SelectedCareer", (int)type);
        PlayerPrefs.Save();

        // 2) GameManager에 반영
        if (GameManager.Instance != null)
            GameManager.Instance.selectedCharacter = type;

        Debug.Log("[CareerSelectionHandler] 선택된 직업: " + type);

        // 3) (선택 강조용) 필요하다면 여기서 하이라이트 처리 호출
        // e.g. HighlightManager.Instance.Highlight(type);
    }
}
