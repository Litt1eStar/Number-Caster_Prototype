using System.Collections.Generic;
using UnityEngine;

public static class DeckHelper
{
    public static Vector3 CalculateTargetPosition(int index, List<Transform> deck, float cardSpacing, float xGap)
    {
        float totalWidth = (deck.Count - 1) * cardSpacing;
        float startZ = -totalWidth / 2;
        float xOffset = index * xGap;

        return new Vector3(-xOffset, 0f, startZ + index * cardSpacing);
    }

    public static bool IsOperatorCard(Card card) => card.cardData.CardType == CardType.Operator;
    public static bool IsNumberCard(Card card) => card.cardData.CardType == CardType.Number;
    public static bool ValidCardTypeOnBoard(Card card) => card.cardData.CardType == CardType.Number || card.cardData.CardType == CardType.Operator; 
}
