using UnityEngine;

public class Card : MonoBehaviour
{
    public CardSO cardData;
   
    private Turn owner;
    private bool isOnTop = false;
    public void SetOwner(Turn newOwner)
    {
        owner = newOwner;
        FlipCard();
    }

    public void FlipCard()
    {
        if (owner == Turn.Player1 && !isOnTop)
        {
            this.transform.localRotation = Quaternion.Euler(0, 180, 0);
            isOnTop = true;
            Debug.Log("Card flipped to Player 1's side.");
        }
    }
}
