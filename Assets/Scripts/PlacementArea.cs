using System.Collections.Generic;
using System;
using UnityEngine;

public class PlacementArea : MonoBehaviour
{
    private bool isEnter = false;
    public float cardSpacing = 0.5f;
    public float animationSpeed = 5.0f;
    public float rotateSpeed = 10f;
    public float xGap = 0.05f;
    public Transform placementParent;

    private List<Transform> cardOnBoards = new List<Transform>();

    private void Update()
    {
        UpdateCardPositions();
    }
    public void AddCard(Transform newCard)
    {
        if (newCard != null) 
        { 
            cardOnBoards.Add(newCard);
            newCard.SetParent(placementParent);

            Debug.Log("Add new Card to Placement Area");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Mouse"))
        {
            ActionManager.Instance.EnterPlacementArea();
            isEnter = true;
            Debug.Log("Enter Placement Area");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isEnter)
        {
            ActionManager.Instance.ExitPlacementArea();
            isEnter = false;
            Debug.Log("Exit Placement Area");
        }
    }

    void UpdateCardPositions()
    {
        for (int i = 0; i < cardOnBoards.Count; i++)
        {
            Vector3 targetPos = CalculateTargetPosition(i);
            cardOnBoards[i].localPosition = Vector3.Lerp(cardOnBoards[i].localPosition, targetPos, Time.deltaTime * animationSpeed);
        }
    }

    Vector3 CalculateTargetPosition(int index)
    {
        float totalWidth = (cardOnBoards.Count - 1) * cardSpacing;
        float startZ = -totalWidth / 2;
        float xOffset = index * xGap;

        return new Vector3(-xOffset, 0f, startZ + index * cardSpacing);
    }
}
