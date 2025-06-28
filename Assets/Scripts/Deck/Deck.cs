using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private Transform deckParent;
    [SerializeField] private float cardSpacing = 0.04f;

    private Stack<GameObject> deckStack = new Stack<GameObject>();   
    private List<GameObject> cards;
    private Vector3 pos;

    private void Start()
    {
        pos = deckParent.position;
    }

    public void SetDeckParent(Transform _deckParent)
    {
        deckParent = _deckParent;
    }
    public void InitDeck(DeckSO deckData)
    {
        if(deckData == null) return;
        cards = new List<GameObject>(deckData.cards);

        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardObj = Instantiate(cards[i], deckParent);
            cardObj.transform.rotation = Quaternion.Euler(0, 0, 90);
            cardObj.transform.localPosition = new Vector3(0, 1, 0);
            //pos = cardObj.transform.position;

            deckStack.Push(cardObj);
        }
    }

    public GameObject DrawCard()
    {
        if(deckStack.Count <= 0) return null;

        GameObject drawnCard = deckStack.Pop();
        return drawnCard;
    }
}
