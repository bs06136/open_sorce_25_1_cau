using UnityEngine;

public class CardButtonHandler : MonoBehaviour
{
    public int cardIndex; // 인스펙터에서 설정: 0, 1, 2 

    public void OnCardButtonClicked()
    {
        GameEvents.OnCardChosen?.Invoke(cardIndex);
    }
}
