using UnityEngine;

public enum CardType
{
    Number,
    Operator,
    Skill
}

[CreateAssetMenu(fileName = "CardSO", menuName = "Scriptable Objects/CardSO")]
public class CardSO : ScriptableObject
{
    public CardType CardType;
    public Sprite cardImage;
    public string cardName;
    public int cardLevel;
    public char cardValue;
    public string cardDescription;
}
