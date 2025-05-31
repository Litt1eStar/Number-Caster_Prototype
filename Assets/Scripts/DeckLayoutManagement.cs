using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class DeckLayoutManagement : MonoBehaviour
{
    public float cardSpacing = 0.5f;    
    public float animationSpeed = 5.0f;
    public float yGap = 0.05f;

    public PlayerSide side; //for test

    public Transform player1_deckPosition;
    public Transform player2_deckPosition;

    private List<Transform> cards_player01 = new List<Transform>();
    private List<Transform> cards_player02 = new List<Transform>();

    private List<Transform> working_cards = new List<Transform>();
    public GameObject newCard;
    void UpdateCardPositions()
    {
        float totalWidth = (working_cards.Count - 1) * cardSpacing;
        float startX = -totalWidth / 2;

        for (int i = 0; i < working_cards.Count; i++)
        {
            float yOffset = i * yGap;
            Vector3 targetPos = new Vector3(-yOffset, 0f, startX + i * cardSpacing);
            working_cards[i].localPosition = Vector3.Lerp(working_cards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
            working_cards[i].SetAsLastSibling();
        }
    }

    public void AddCard(GameObject card)
    {
        Transform deckTransform = side == PlayerSide.Player1 ? player1_deckPosition : player2_deckPosition;
        working_cards = side == PlayerSide.Player1 ? cards_player01 : cards_player02;
        card.transform.SetParent(deckTransform);
        card.transform.localRotation = Quaternion.identity;
        working_cards.Add(card.transform);
        UpdateCardPositions();
    }

    public void RemoveCard(GameObject card)
    {
        if (working_cards.Contains(card.transform))
        {
            working_cards.Remove(card.transform);
            Destroy(card);
            UpdateCardPositions();
        }
    }

    private void Start()
    {
        side = PlayerSide.Player1;
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
