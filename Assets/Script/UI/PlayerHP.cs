using UnityEngine;
using TMPro;

public class PlayerHP : MonoBehaviour
{
    public TextMeshProUGUI hpText;
    private int _currentHP;

    public void SetHP(int value)
    {
        _currentHP = value;
        hpText.text = _currentHP.ToString();
    }
}
