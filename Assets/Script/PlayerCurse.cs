using UnityEngine;
using TMPro;

public class PlayerCurse : MonoBehaviour
{
    public TextMeshProUGUI curseText;

    private int _currentCurse;
    public int CurrentCurse
    {
        get { return _currentCurse; }
        set
        {
            if (_currentCurse != value)
            {
                _currentCurse = value;
                curseText.text = $"{_currentCurse}";
            }
        }
    }

    void Start()
    {
        CurrentCurse = 0;
    }
}