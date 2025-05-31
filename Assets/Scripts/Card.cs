using UnityEngine;

public class Card : MonoBehaviour
{
    [SerializeField] private PlayerSide owner;

    private bool isOnTop = false;
    public void SetOwner(PlayerSide newOwner)
    {
        owner = newOwner;
        FlipCard();
    }

    public void FlipCard()
    {
        if (owner == PlayerSide.Player1 && !isOnTop)
        {
            this.transform.localRotation = Quaternion.Euler(0, 180, 0);
            isOnTop = true;
            Debug.Log("Card flipped to Player 1's side.");
        }
    }
}
