using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<GameObject> cards;
    [SerializeField] private Transform deckParent;
    [SerializeField] private float cardSpacing = 0.04f;

    private Stack<GameObject> deckStack = new Stack<GameObject>();   
    private Vector3 pos;

    private void Start()
    {
        pos = deckParent.position;

        //InitDeck();
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
            cardObj.transform.position = pos + new Vector3(0, cardSpacing, 0);
            cardObj.transform.localPosition = new Vector3(1, 1, 1);
            pos = cardObj.transform.position;

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
