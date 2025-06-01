using System;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> cards;
    public Transform deckParent;
    [SerializeField] private float cardSpacing = 0.04f;

    private Queue<GameObject> deckQueue = new Queue<GameObject>();   

    Vector3 pos;

    private void Start()
    {
        pos = deckParent.position;

        InitDeck();
    }

    private void InitDeck()
    {
        for (int i = 0; i < cards.Count; i++)
        {
            GameObject cardObj = Instantiate(cards[i], deckParent);
            cardObj.transform.rotation = Quaternion.Euler(0, 0, 90);
            cardObj.transform.position = pos + new Vector3(0, cardSpacing, 0);
            pos = cardObj.transform.position;

            deckQueue.Enqueue(cardObj);
        }
    }

    public GameObject DrawCard()
    {
        if(deckQueue.Count <= 0) return null;

        GameObject drawnCard = deckQueue.Dequeue();
        return drawnCard;
    }
}
