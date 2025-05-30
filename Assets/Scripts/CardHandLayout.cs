using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CardHandLayout : MonoBehaviour
{
    public float cardSpacing = 0.5f;    
    public float animationSpeed = 5.0f;
    public float yGap = 0.05f;

    private List<Transform> cards = new List<Transform>();
    public GameObject newCard;
    void UpdateCardPositions()
    {
        float totalWidth = (cards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < cards.Count; i++)
        {
            float yOffset = i * yGap;
            Vector3 targetPos = new Vector3(-yOffset, 0f, startX + i * cardSpacing);
            cards[i].localPosition = Vector3.Lerp(cards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            cards[i].SetAsLastSibling();
        }
    }

    public void AddCard(GameObject card)
    {
        card.transform.SetParent(transform);
        card.transform.localRotation = Quaternion.identity;
        cards.Add(card.transform);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject card)
    {
        if (cards.Contains(card.transform))
        {
            cards.Remove(card.transform);
            Destroy(card);
            UpdateCardPositions();
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject m_newCard = Instantiate(newCard);
            AddCard(m_newCard);
        }
        UpdateCardPositions();
    }
}
