using UnityEngine;

[System.Serializable]
public class CardData
{
    public string cardName;
    public string description;
    public Sprite cardImage;

    public CardData()
    {
    }

    public CardData(string name, string desc, Sprite sprite)
    {
        cardName = name;
        description = desc;
        cardImage = sprite;
    }
}