using UnityEngine;
using TMPro;

public class PlayerCurse : MonoBehaviour
{
    public TextMeshProUGUI curseText;
    private int _currentCurse;

    public void SetCurse(int value)
    {
        _currentCurse = value;
        curseText.text = _currentCurse.ToString();
    }
}
