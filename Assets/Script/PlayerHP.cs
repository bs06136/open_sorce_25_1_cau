using UnityEngine;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    public TextMeshProUGUI hpText;

    private int _currentHP;
    public int CurrentHP
    {
        get { return _currentHP; }
        set
        {
            if (_currentHP != value)
            {
                _currentHP = value;
                hpText.text = $"{_currentHP}";
            }
        }
    }

    void Start()
    {
        CurrentHP = 10;
    }
}