using UnityEngine;
using UnityEngine.UI;
using CardGame;
using System.Collections.Generic;

public class CareerSelectionHandler : MonoBehaviour
{
    [System.Serializable]
    public class CareerImage
    {
        public CharacterType type;
        public GameObject obj; // 캐릭터 오브젝트(UI 오브젝트)
    }

    [Header("캐릭터 오브젝트와 타입 매핑")]
    public List<CareerImage> careerImages; // 인스펙터에서 각 캐릭터별로 할당

    [Header("흑백 머티리얼")]
    public Material grayScaleMaterial; // 인스펙터에서 할당

    public void SelectExplorer() => SelectCareer(CharacterType.Explorer);
    public void SelectGravekeeper() => SelectCareer(CharacterType.Gravekeeper);
    public void SelectNecromancer() => SelectCareer(CharacterType.Necromancer);
    public void SelectCleric() => SelectCareer(CharacterType.Cleric);
    public void SelectGambler() => SelectCareer(CharacterType.Gambler);
    public void SelectAvenger() => SelectCareer(CharacterType.Avenger);
    public void SelectMerchant() => SelectCareer(CharacterType.Merchant);
    public void SelectDemonBinder() => SelectCareer(CharacterType.DemonBinder);

    private void SelectCareer(CharacterType type)
    {
        // 1) 저장
        PlayerPrefs.SetInt("SelectedCareer", (int)type);
        PlayerPrefs.Save();

        // 2) GameManager에 반영
        if (GameManager.Instance != null)
            GameManager.Instance.selectedCharacter = type;

        Debug.Log("[CareerSelectionHandler] 선택된 직업: " + type);

        // 3) 흑백 처리
        ApplyGrayScale(type);
    }

    private void ApplyGrayScale(CharacterType selected)
    {
        foreach (var c in careerImages)
        {
            if (c.obj != null)
            {
                var img = c.obj.GetComponent<Image>();
                if (img != null)
                {
                    // 선택된 캐릭터만 컬러, 나머지는 흑백
                    img.material = (c.type == selected) ? null : grayScaleMaterial;
                }
            }
        }
    }

    private void Start()
    {
        int saved = PlayerPrefs.GetInt("SelectedCareer", (int)CharacterType.Explorer);
        ApplyGrayScale((CharacterType)saved);
    }
}
