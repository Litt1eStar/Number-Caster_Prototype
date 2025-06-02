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
    public char cardValue;
    public string cardName;
}
